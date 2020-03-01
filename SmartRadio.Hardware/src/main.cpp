#include <Arduino.h>
#include <FS.h>
#include <SD.h>
#include <SPI.h>
#include <SPIFFS.h>
#include <time.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>

#include <vector>

#include <SSD1306Wire.h>
#include <NTPClient.h>
#include "AudioFileSourceSPIFFS.h"
#include "AudioFileSourceICYStream.h"
#include "AudioFileSourceBuffer.h"
#include "AudioGeneratorMP3.h"
#include "AudioOutputI2S.h"
#include "extflash.h"
#include "fatflash.h"

#include "utilities.h"

#define MAX_STACK_SIZE 10000

#define PIN_SPI_MOSI GPIO_NUM_23 // PIN 5 - IO0 - DI
#define PIN_SPI_MISO GPIO_NUM_19 // PIN 2 - IO1 - DO
#define PIN_SPI_WP GPIO_NUM_17   // PIN 3 - IO2 - /WP
#define PIN_SPI_HD GPIO_NUM_21   // PIN 7 - IO3 - /HOLD - /RESET
#define PIN_SPI_SCK GPIO_NUM_18  // PIN 6 - CLK - CLK
#define PIN_SPI_SS GPIO_NUM_5    // PIN 1 - /CS - /CS
#define MOUNT_POINT_FAT "/fatflash"

#define VOLUME_PIN 27

const char *SSID = "Gerginov";
const char *PASSWORD = "59199878";

int last_read_volume = 0;

std::string message;
// url for Z-Rock's radio stream
// char *url = "http://46.10.150.243/z-rock.mp3";
// char *url = "http://78.31.65.20:8080/dance.mp3";
char *url = "http://play.global.audio:80/nrj128";
// char *url="http://play.global.audio/veronika128";

AudioGeneratorMP3 *mp3;
AudioFileSourceICYStream *file;

AudioFileSourceBuffer *buff;
AudioOutputI2S *out;

ExtFlash extflash;
FatFlash fatflash;

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "europe.pool.ntp.org", 7200, 60000);

SSD1306Wire oled(0x3c, 33, 32, GEOMETRY_128_32);

// Called when a metadata event occurs (i.e. an ID3 tag, an ICY block, etc.)
void MDCallback(void *cbData, const char *type, bool isUnicode, const char *string)
{
    const char *ptr = reinterpret_cast<const char *>(cbData);
    char s1[32], s2[64];
    strncpy_P(s1, type, sizeof(s1));
    s1[sizeof(s1) - 1] = 0;
    strncpy_P(s2, string, sizeof(s2));
    s2[sizeof(s2) - 1] = 0;
    Serial.printf("METADATA(%s) '%s' = '%s'\n", ptr, s1, s2);
    message = std::string(s2);
    Serial.printf("Text length on OLED: %d\n", oled.getStringWidth(s2));
    Serial.flush();
}

void StatusCallback(void *cbData, int code, const char *string)
{
    const char *ptr = reinterpret_cast<const char *>(cbData);
    char s1[64];
    strncpy_P(s1, string, sizeof(s1));
    s1[sizeof(s1) - 1] = 0;
    Serial.printf("STATUS(%s) '%d' = '%s'\n", ptr, code, s1);
    Serial.flush();
}

void setup_serial()
{
    Serial.begin(9600);
}

void setup_gpio()
{
}

void setup_oled()
{
    oled.init();
    oled.flipScreenVertically();
    oled.setFont(ArialMT_Plain_10);
}

void setup_external_fat()
{
    SPIFFS.begin();
    ext_flash_config_t cfg =
            {
                .vspi = true,
                .sck_io_num = PIN_SPI_SCK,
                .miso_io_num = PIN_SPI_MISO,
                .mosi_io_num = PIN_SPI_MOSI,
                .ss_io_num = PIN_SPI_SS,
                .hd_io_num = PIN_SPI_HD,
                .wp_io_num = PIN_SPI_WP,
                .speed_mhz = (int8_t) 40,
                .dma_channel = 1,
                .queue_size = (int8_t) 4,
                .max_dma_size = 8192,
                .sector_size = 0,
                .capacity = 0
            };
    
    esp_err_t err = extflash.init(&cfg);

    if(err != ESP_OK) {
        Serial.println("Flash initialization failed.");
        for(;;);
    }

    fat_flash_config_t fat_cfg =
    {
        .flash = &extflash,
        .base_path = MOUNT_POINT_FAT,
        .open_files = 4,
        .auto_format = true
    };

    err = fatflash.init(&fat_cfg);

    if(err != ESP_OK) {
        Serial.println("External FAT initalization failed.");
        for(;;);
    }



    Serial.printf("Flash initalization successful:\n - Sector size:%d\n - Capacity: %d\n", extflash.sector_size(), extflash.chip_size());
}

void setup_wifi()
{
    Serial.println("Connecting to WiFi");

    WiFi.disconnect();
    WiFi.softAPdisconnect(true);
    WiFi.mode(WIFI_STA);

    WiFi.begin(SSID, PASSWORD);

    // Try forever
    while (WiFi.status() != WL_CONNECTED)
    {
        Serial.println("...Connecting to WiFi");
        delay(1000);
    }
    Serial.printf("Connected (%s)\n", WiFi.localIP().toString().c_str());
}

void setup_ntp()
{
    timeClient.begin();
}

void setup_audio_transmission()
{
    audioLogger = &Serial;
    file = new AudioFileSourceICYStream(url);
    file->RegisterMetadataCB(MDCallback, (void *)"ICY");

    buff = new AudioFileSourceBuffer(file, 2048);
    buff->RegisterStatusCB(StatusCallback, (void *)"buffer");

    out = new AudioOutputI2S();
    out->SetGain(0.05);

    mp3 = new AudioGeneratorMP3();
    mp3->RegisterStatusCB(StatusCallback, (void *)"mp3");
    // mp3->begin(buff, out);
}

void handle_volume()
{
    int new_volume = analogRead(VOLUME_PIN) / 40.96;
    if (last_read_volume != new_volume)
    {
        last_read_volume = new_volume;
        out->SetGain(new_volume / 100.0);
    }
}

void drive_oled(void *)
{
    for (;;)
    {
        oled.clear();

        timeClient.update();
        oled.drawString(0, 0, timeClient.getFormattedTime());

        if (message != "")
        {
            oled.drawString(0, 10, message.c_str());
        }

        oled.display();

        vTaskDelay(1000);
    }
}

void record_mp3_to_flash(void *) {
    

    vTaskSuspend(NULL);
}

void setup()
{
    setup_serial();
    setup_external_fat();
    setup_gpio();
    setup_oled();
    setup_wifi();
    setup_ntp();
    setup_audio_transmission();
    HTTPClient client;
    File f = SPIFFS.open("/fatflash/test.mp3", "w");
    client.begin("https://file-examples.com/wp-content/uploads/2017/11/file_example_MP3_700KB.mp3");
    client.GET();
    client.writeToStream(&f);
    Serial.println("Downloaded");
    f.close();
    client.end();

    AudioFileSourceSPIFFS *fs = new AudioFileSourceSPIFFS("/fatflash/test.mp3");
    mp3->begin(fs, out);

    xTaskCreatePinnedToCore(drive_oled, "CLOCK_DRIVER", MAX_STACK_SIZE, NULL, 1, NULL, 1);
    xTaskCreatePinnedToCore(record_mp3_to_flash, "MP3_RECORDER", MAX_STACK_SIZE, NULL, 1, NULL, 1);
}

void loop()
{
    if (mp3->isRunning())
    {
        if (!mp3->loop())
        {
            mp3->stop();
        }
    }
    else
    {
        Serial.printf("MP3 done\n");
        delay(1000);
    }
}