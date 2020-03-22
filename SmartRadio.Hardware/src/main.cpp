#include "definitions.h"
#include "setup.h"
#include "task_functions.h"
#include "utilities.h"
#include "variables.h"

int last_time_of_button_press = 0;
bool is_pressed = false;

void clear_audio_transmission()
{
    delete mp3;
    delete file;
    delete buff;
    delete out;
}

void setup()
{
    disableCore0WDT();
    disableCore1WDT();
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

    if(digitalRead(PIN_CONFIRM_VALUE_BUTTON))
    {
        if(!is_pressed)
        {
            is_pressed = true;
            last_time_of_button_press = millis();
        }

        if(millis() - last_time_of_button_press > 3000 && !is_setting_time)
        {
            set_hours = timeClient->getHours();
            set_minutes = timeClient->getMinutes();
            is_setting_time = true;
            xTaskCreatePinnedToCore(create_new_alarm, "CREATE_NEW_ALARM", MAX_STACK_SIZE, NULL, 1, NULL, 0); 
            log_d("Is setting time.");
        }
    }
    else
    {
        is_pressed = false;
    }
    
}