#include "definitions.h"
#include "setup.h"
#include "task_functions.h"
#include "utilities.h"
#include "variables.h"

static int last_time_of_button_press = 0;
static bool is_confirm_button_pressed = false;

static TaskHandle_t t_record_audio = NULL;

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
    identify_owner();

    
    load_alarms_from_file();

    xTaskCreatePinnedToCore(drive_oled, "CLOCK_DRIVER", MAX_STACK_SIZE, NULL, 1, NULL, 1);
    xTaskCreatePinnedToCore(drive_alarm_manager, "DRIVE_ALARM_MANAGER", MAX_STACK_SIZE / 10, NULL, 3, NULL, 0);
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

    if (digitalRead(PIN_CONFIRM_VALUE_BUTTON))
    {
        if (!is_confirm_button_pressed)
        {
            is_confirm_button_pressed = true;
            last_time_of_button_press = millis();
        }

        if (millis() - last_time_of_button_press > 3000 && !is_setting_time)
        {
            set_hours = timeClient->getHours();
            set_minutes = timeClient->getMinutes();
            is_setting_time = true;
            xTaskCreatePinnedToCore(create_new_alarm, "CREATE_NEW_ALARM", MAX_STACK_SIZE, NULL, 1, NULL, 0);
            log_d("Is setting time.");
        }

        if(millis() - last_time_of_button_press > 10000)
        {
            unlink(MOUNT_POINT_FAT "/alarms.txt");
            log_i("Alarms file deleted.");
        }
    }
    else
    {
        is_confirm_button_pressed = false;
    }
}