#include "definitions.h"
#include "setup.h"
#include "utilities.h"

void record_mp3_to_flash(void *)
{
    vTaskSuspend(NULL);
}

void record_single_snippet(void *)
{
    vTaskSuspend(NULL);
}

void setup()
{
    setup_external_fat();
    setup_gpio();
    setup_oled();
    setup_wifi();
    setup_ntp();
    setup_audio_transmission();
    /*
    HTTPClient client;
    FILE *f = fopen(MOUNT_POINT_FAT "/test.mp3", "w");
    client.begin("https://file-examples.com/wp-content/uploads/2017/11/file_example_MP3_5MG.mp3");
    client.GET();
    char buffer[20480];
    int size = 0;
    do
    {
        size = client.getStreamPtr()->readBytes(buffer, 20480);

        Serial.println(ESP.getFreeHeap());
        fwrite(buffer, sizeof(byte), size, f);
    } while (size != 0);
    fclose(f);
    client.end();

    AudioFileSourceExtFlash *ext = new AudioFileSourceExtFlash(MOUNT_POINT_FAT "/test.mp3");
    mp3->begin(ext, out);
    */

    // xTaskCreatePinnedToCore(drive_oled, "CLOCK_DRIVER", MAX_STACK_SIZE, NULL, 1, NULL, 1);
    xTaskCreatePinnedToCore(record_mp3_to_flash, "MP3_RECORDER", MAX_STACK_SIZE, NULL, 0, NULL, 1);
    // xTaskCreatePinnedToCore(record_single_snippet, "RECORD_SINGLE_SNIPPET", MAX_STACK_SIZE, NULL, 0, NULL, 1);
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
        log_i("MP3 done. Restarting...\n");
        clear_audio_transmission();
        setup_audio_transmission();
    }
}