#include "task_functions.h"

void drive_oled(void *)
{
    for (;;)
    {
        oled->clear();

        timeClient->update();
        oled->drawString(0, 0, timeClient->getFormattedTime());

        if (message != "")
        {
            oled->drawString(0, 10, message.c_str());
        }

        oled->display();

        vTaskDelay(500);
    }
}

void record_snippet(void *)
{
    AudioFileSourceICYStream *recordFile = new AudioFileSourceICYStream(STREAM_URL);
    recordFile->RegisterMetadataCB(metadata_callback, (void *)"ICY");

    AudioFileSourceBuffer *recordBuff = new AudioFileSourceBuffer(file, 32768);
    recordBuff->RegisterStatusCB(status_callback, (void *)"extflash_buffer");

    AudioGeneratorMP3 *recordMp3 = new AudioGeneratorMP3();
    recordMp3->RegisterStatusCB(status_callback, (void *)"extflash_mp3");

    AudioOutputExtFlash *recordOut = new AudioOutputExtFlash();
    recordOut->SetFilename(MOUNT_POINT_FAT "/snippet.mp3");

    recordMp3->begin(recordBuff, recordOut);
    
    for (;;);
    vTaskSuspend(NULL);
}