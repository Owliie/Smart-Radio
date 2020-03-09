#include "definitions.h"
#include "setup.h"
#include "variables.h"

std::string message;

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
}

void setup_oled()
{
    oled = new SSD1306Wire(0x3c, 33, 32, GEOMETRY_128_32);

    oled->init();
    oled->flipScreenVertically();
    oled->setFont(ArialMT_Plain_10);
}

void setup_external_fat()
{
    // SPIFFS.begin();
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
            ;
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
        log_e("External FAT initalization failed.");
        for (;;)
            ;
    }

    log_d("Flash initalization successful:\n - Sector size:%d\n - Capacity: %d\n", extflash.sector_size(), extflash.chip_size());
}

void setup_wifi()
{
    log_d("Connecting to WiFi");

    WiFi.disconnect();
    WiFi.softAPdisconnect(true);
    WiFi.mode(WIFI_STA);

    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

    // Try forever
    while (WiFi.status() != WL_CONNECTED)
    {
        log_d("...Connecting to WiFi");
        delay(1000);
    }
    log_d("Connected\n", WiFi.localIP().toString().c_str());
}

void setup_ntp()
{
    timeClient = new NTPClient(ntpUDP, "europe.pool.ntp.org", 7200, 60000);
    timeClient->begin();
}

// Called when a metadata event occurs (i.e. an ID3 tag, an ICY block, etc.)
void MDCallback(void *cbData, const char *type, bool isUnicode, const char *string)
{
    const char *ptr = reinterpret_cast<const char *>(cbData);
    char s1[32], s2[64];
    strncpy_P(s1, type, sizeof(s1));
    s1[sizeof(s1) - 1] = 0;
    strncpy_P(s2, string, sizeof(s2));
    s2[sizeof(s2) - 1] = 0;
    log_i("METADATA(%s) '%s' = '%s'\n", ptr, s1, s2);
    message = std::string(s2);
    log_d("Text length on OLED: %d\n", oled->getStringWidth(s2));
    Serial.flush();
}

void StatusCallback(void *cbData, int code, const char *string)
{
    const char *ptr = reinterpret_cast<const char *>(cbData);
    char s1[64];
    strncpy_P(s1, string, sizeof(s1));
    s1[sizeof(s1) - 1] = 0;
    log_w("STATUS(%s) '%d' = '%s'\n", ptr, code, s1);
    Serial.flush();
}

void setup_audio_transmission()
{
    audioLogger = &Serial;
    file = new AudioFileSourceICYStream(STREAM_URL);
    file->RegisterMetadataCB(MDCallback, (void *)"ICY");

    buff = new AudioFileSourceBuffer(file, 32768);
    buff->RegisterStatusCB(StatusCallback, (void *)"buffer");

    out = new AudioOutputI2S();
    out->SetPinout(GPIO_NUM_26, GPIO_NUM_25, GPIO_NUM_27);
    out->SetGain(0.05);

    mp3 = new AudioGeneratorMP3();
    mp3->RegisterStatusCB(StatusCallback, (void *)"mp3");
    mp3->begin(buff, out);
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