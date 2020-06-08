#include "setup.h"

String identityCookie;
String ipAddress;

std::string message;
int transfer_progress = 0;

bool is_setting_time = false;

bool has_set_hours = false;
bool has_set_minutes = false;

int set_hours = 0;
int set_minutes = 0;

AudioGeneratorMP3 *mp3;
AudioFileSourceICYStream *file;

AudioFileSourceBuffer *buff;
AudioOutputI2S *out;

ExtFlash extflash;
FatFlash fatflash;

WiFiUDP ntpUDP;
NTPClient *timeClient;

SSD1306Wire *oled;

void setup_gpio()
{
    pinMode(PIN_PROMPT_RECORD_BUTTON, INPUT);
    pinMode(PIN_UPDATE_VALUE_BUTTON, INPUT);
    pinMode(PIN_CONFIRM_VALUE_BUTTON, INPUT);
}

void setup_oled()
{
    oled = new SSD1306Wire(0x3c, 33, 32, GEOMETRY_128_32);

    if (!oled->init())
    {
        log_e("OLED initialization failed.");
        for (;;)
            ; // Stop setup, force user to reset
    }

    oled->flipScreenVertically();
    oled->setFont(ArialMT_Plain_10);
}

void setup_external_fat()
{
    ext_flash_config_t cfg =
        {
            .vspi = true,
            .sck_io_num = PIN_SPI_SCK,
            .miso_io_num = PIN_SPI_MISO,
            .mosi_io_num = PIN_SPI_MOSI,
            .ss_io_num = PIN_SPI_SS,
            .hd_io_num = PIN_SPI_HD,
            .wp_io_num = PIN_SPI_WP,
            .speed_mhz = (int8_t)40,
            .dma_channel = 1,
            .queue_size = (int8_t)4,
            .max_dma_size = 8192,
            .sector_size = 0,
            .capacity = 0};

    esp_err_t err = extflash.init(&cfg);

    if (err != ESP_OK)
    {
        log_e("Flash initialization failed. (err: %d)\n", err);
        for (;;)
            ; // Stop setup, force user to reset
    }

    fat_flash_config_t fat_cfg =
        {
            .flash = &extflash,
            .base_path = MOUNT_POINT_FAT,
            .open_files = 4,
            .auto_format = true};

    err = fatflash.init(&fat_cfg);

    if (err != ESP_OK)
    {
        log_e("FAT mounting failed.");
        for (;;)
            ; // Stop setup, force user to reset
    }

    log_d("Flash initalization successful:\n - Sector size:%d\n - Capacity: %d\n", extflash.sector_size(), extflash.chip_size());
}

void setup_wifi()
{
    log_d("Connecting to WiFi");

    WiFi.disconnect();
    WiFi.softAPdisconnect(true);
    WiFi.mode(WIFI_STA);
    WiFi.setSleep(false);

    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

    // Try forever
    while (WiFi.status() != WL_CONNECTED)
    {
        log_d("...Connecting to WiFi");
        delay(1000);
    }

    ipAddress = WiFi.localIP().toString();

    log_d("IP address: ");
    log_d("%s", ipAddress.c_str());
    log_d("Subnet Mask: ");
    log_d("%s", WiFi.subnetMask().toString().c_str());
    log_d("Gateway IP: ");
    log_d("%s", WiFi.gatewayIP().toString().c_str());
    log_d("DNS: ");
    log_d("%s", WiFi.dnsIP().toString().c_str());
}

void setup_ntp()
{
    timeClient = new NTPClient(ntpUDP, "europe.pool.ntp.org", 10800, 60000);
    timeClient->begin();
}

void setup_audio_transmission()
{
    audioLogger = &Serial;
    file = new AudioFileSourceICYStream(STREAM_URL);
    file->RegisterMetadataCB(metadata_callback, (void *)"ICY");

    // AudioFileSourceExtFlash *flash = new AudioFileSourceExtFlash(MOUNT_POINT_FAT "/snippet.mp3");

    buff = new AudioFileSourceBuffer(file, 32768);
    buff->RegisterStatusCB(status_callback, (void *)"buffer");

    out = new AudioOutputI2S();
    out->SetPinout(GPIO_NUM_26, GPIO_NUM_25, GPIO_NUM_27);
    out->SetGain(0.05);

    mp3 = new AudioGeneratorMP3();
    mp3->RegisterStatusCB(status_callback, (void *)"mp3");

    mp3->begin(buff, out);
}

void load_alarms_from_file()
{
    std::ifstream in;
    in.open(MOUNT_POINT_FAT "/alarms.txt");

    if (!in.fail())
    {
        while (!in.eof())
        {
            int hours, minutes;
            in >> hours >> minutes;
            Alarm a(hours, minutes);
            alarm_manager.add_alarm(a);
        }
    }
    else
    {
        log_e("Error opening alarms file");
    }

    in.close();
}

void identify_owner()
{
    HTTPClient http;
    const char *headerKeys[] = {"Set-Cookie"};
    http.collectHeaders(headerKeys, 1);

    String token;
    String antiforgeryCookie;

    if (http.begin(MUSIC_RECOGNITION_BASE_URL "/identity/account/login"))
    {
        int responseCode = http.GET();
        if (responseCode > 0 && responseCode == 200)
        {
            String payload = http.getString();
            std::string payloadCpp = payload.c_str();

            payloadCpp = payloadCpp.substr(5000, 1000);
            std::regex rgx("value\\=\"");

            std::smatch matches;

            log_d("%s", payloadCpp.c_str());
            std::sregex_iterator currentMatch(payloadCpp.begin(),
                                              payloadCpp.end(), rgx);

            std::smatch match = *currentMatch;
            int position = match.position();
            token = String(payloadCpp.substr(position + 7, 155).c_str());
            log_d("%s", token.c_str());

            std::string antiforgeryCookieCpp = http.header("Set-Cookie").c_str();
            antiforgeryCookie = String(antiforgeryCookieCpp.substr(0, antiforgeryCookieCpp.find(";")).c_str());
        }
    }

    http.end();

    HTTPClient http2;
    http2.collectHeaders(headerKeys, 1);
    if (http2.begin(MUSIC_RECOGNITION_BASE_URL "/identity/account/login"))
    {

        http2.addHeader("Content-Type", "application/x-www-form-urlencoded");
        http2.addHeader("Cookie", antiforgeryCookie);
        String httpRequestData = "Input.Username=misho&Input.Password=test12&__RequestVerificationToken=" + String(token.c_str());
        int responseCode = http2.POST(httpRequestData);
        if (responseCode > 0 && responseCode == 302)
        {
            std::string identityCookieCpp = http2.header("Set-Cookie").c_str();
            identityCookie = String(identityCookieCpp.substr(0, identityCookieCpp.find(";")).c_str());
        }
    }

    http2.end();
}

/*
void handle_volume()
{
    int new_volume = analogRead(VOLUME_PIN) / 40.96;
    if (last_read_volume != new_volume)
    {
        last_read_volume = new_volume;
        out->SetGain(new_volume / 100.0);
    }
}
*/