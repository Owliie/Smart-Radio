#include "task_functions.h"
#include "variables.h"

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

        vTaskDelay(1000);
    }
}

void record_mp3_to_flash(void *)
{
    vTaskSuspend(NULL);
}