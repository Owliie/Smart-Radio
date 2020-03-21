#include "definitions.h"
#include "setup.h"
#include "task_functions.h"
#include "utilities.h"
#include "variables.h"

void setup()
{
    Serial.begin(115200);
    setup_external_fat();
    setup_gpio();
    setup_oled();
    setup_wifi();
    setup_ntp();
    setup_audio_transmission();

    xTaskCreatePinnedToCore(drive_oled, "CLOCK_DRIVER", MAX_STACK_SIZE, NULL, 1, NULL, 1);
    // delay(3000);
    xTaskCreatePinnedToCore(drive_piezo, "PIEZO_DRIVER", MAX_STACK_SIZE, NULL, 1, NULL, 1);
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

    if (digitalRead(PIN_PROMPT_RECORD_BUTTON) && (t_record_audio == NULL || eTaskGetState(t_record_audio) == eSuspended))
    {
            xTaskCreatePinnedToCore(record_snippet, "RECORD_IN_MEMORY", MAX_STACK_SIZE, NULL, 1, &t_record_audio, 1);
    }
}