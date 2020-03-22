#include "task_functions.h"
#define USE_SERIAL Serial

static const uint8_t clock_bits[] = {
    0xFC,
    0x00,
    0x02,
    0x01,
    0x21,
    0x02,
    0x21,
    0x02,
    0x21,
    0x02,
    0xE1,
    0x02,
    0x01,
    0x02,
    0x01,
    0x02,
    0x02,
    0x01,
    0xFC,
    0x00,
};
static const uint8_t note_bits[] = {
    0xC0,
    0x03,
    0xFC,
    0x03,
    0x3C,
    0x03,
    0x0C,
    0x03,
    0x0C,
    0x03,
    0x0C,
    0x03,
    0x8C,
    0x03,
    0xCE,
    0x03,
    0x8F,
    0x01,
    0x06,
    0x00,
};
static const uint8_t bell_bits[] = {
    0x78,
    0x00,
    0x84,
    0x00,
    0x84,
    0x00,
    0x84,
    0x00,
    0x84,
    0x00,
    0x84,
    0x00,
    0x02,
    0x01,
    0xFE,
    0x01,
    0x48,
    0x00,
    0x30,
    0x00,
};

void create_new_alarm(void *)
{
    while (digitalRead(PIN_CONFIRM_VALUE_BUTTON))
        ;

    int last_activity = millis();

    bool last_update_button_state = false, last_confirm_button_state = false;
    bool current_update_button_state = false, current_confirm_button_state = false;

    while (is_setting_time)
    {
        if (millis() - last_activity > 10000)
        {
            is_setting_time = false;
        }

        if (has_set_hours && has_set_minutes)
        {
            has_set_hours = false;
            has_set_minutes = false;
            is_setting_time = false;

            alarm_manager.add_alarm(set_hours, set_minutes);
        }

        current_update_button_state = digitalRead(PIN_UPDATE_VALUE_BUTTON);
        if (current_update_button_state != last_update_button_state)
        {
            log_d("asd");
            last_activity = millis();
            if (current_update_button_state)
            {
                log_d("test");
                if (!has_set_hours)
                {
                    log_d("test 2");
                    set_hours = set_hours >= 23 ? 0 : set_hours + 1;
                }
                else if (has_set_hours && !has_set_minutes)
                {
                    set_minutes = set_minutes >= 59 ? 0 : set_minutes + 1;
                }
            }
        }

        current_confirm_button_state = digitalRead(PIN_CONFIRM_VALUE_BUTTON);
        if (current_confirm_button_state != last_confirm_button_state)
        {
            last_activity = millis();

            if (current_confirm_button_state)
            {
                if (!has_set_hours)
                {
                    has_set_hours = true;
                }
                else if (has_set_hours && !has_set_minutes)
                {
                    has_set_minutes = true;
                }
            }
        }

        last_update_button_state = current_update_button_state;
        last_confirm_button_state = current_confirm_button_state;
    }
    vTaskSuspend(NULL);
}

void drive_oled(void *)
{
    for (;;)
    {
        oled->clear();

        if (!is_setting_time)
        {
            timeClient->update();
            oled->drawXbm(0, 2, OLED_ICON_WIDTH, OLED_ICON_HEIGHT, clock_bits);
            oled->drawString(12, 0, timeClient->getFormattedTime());

            if (alarm_manager.count() > 0)
            {
                oled->drawXbm(90, 2, OLED_ICON_WIDTH, OLED_ICON_HEIGHT, bell_bits);
                std::vector<Alarm> alarms = alarm_manager.get_alarms();
                if (alarm_manager.count() == 1)
                {
                    oled->drawString(100, 0, String(alarms[0].to_string().c_str()));
                }
            }

            oled->drawXbm(0, 14, OLED_ICON_WIDTH, OLED_ICON_HEIGHT, note_bits);
            if (message != "")
            {
                oled->drawString(12, 12, message.c_str());
            }
            else
            {
                oled->drawString(12, 12, "(no data available)");
            }

            if (transfer_progress != 0)
            {
                oled->drawProgressBar(0, 25, 126, 5, transfer_progress);
            }
        }
        else
        {
            oled->drawString(0, 0, "[New alarm]");
            if (!has_set_hours)
            {
                oled->drawString(0, 10, "Enter hours: ");
                oled->drawString(60, 10,
                                 String(pad_left('0', 2, set_hours).c_str()) + String(":") + String(pad_left('0', 2, set_minutes).c_str()));
            }
            else if (!has_set_minutes && has_set_hours)
            {
                oled->drawString(0, 10, "Enter minutes: ");
                oled->drawString(70, 10,
                                 String(pad_left('0', 2, set_hours).c_str()) + String(":") + String(pad_left('0', 2, set_minutes).c_str()));
            }
        }

        oled->display();

        vTaskDelay(200);
    }
}

void drive_piezo(void *)
{
    for (;;)
    {
        tone(PIN_PIEZO, 1000);
        delay(500);
        noTone(PIN_PIEZO);
        delay(500);
    }
    vTaskSuspend(NULL);
}

void record_snippet(void *)
{
    HTTPClient http;

    USE_SERIAL.print("[HTTP] begin...\n");

    http.begin(STREAM_URL);

    USE_SERIAL.print("[HTTP] GET...\n");
    // start connection and send HTTP header
    int httpCode = http.GET();
    // create buffer for read
    uint8_t buff[256] = {0};
    int total = 0;
    uint8_t *output = (uint8_t *)ps_malloc(256000);

    if (httpCode > 0)
    {
        // HTTP header has been send and Server response header has been handled
        USE_SERIAL.printf("[HTTP] GET... code: %d\n", httpCode);

        // file found at server
        if (httpCode == HTTP_CODE_OK)
        {
            // get lenght of document (is -1 when Server sends no Content-Length header)
            int len = http.getSize();

            // get tcp stream
            WiFiClient *stream = http.getStreamPtr();

            // read all data from server
            while (http.connected() && (len > 0 || len == -1) && total < SNIPPET_SIZE)
            {

                message = "Recording... (";
                message += itos(total / 1000);
                message += "k/250k)";
                transfer_progress = (total / (double)SNIPPET_SIZE) * 50;
                // get available data size
                size_t size = stream->available();

                log_d("Stream size: %d (total = %d)", size, total);

                if (size)
                {
                    // read up to 256 byte
                    int c = stream->readBytes(buff, ((size > 256) ? 256 : size));
                    memcpy(output + total, (void *)buff, c);
                    // write it to Serial
                    // USE_SERIAL.write(buff, c);

                    total += c;

                    if (len > 0)
                    {
                        len -= c;
                    }
                }
                delay(1);
            }

            USE_SERIAL.println();
            USE_SERIAL.print("[HTTP] connection closed or file end.\n");
        }
    }
    else
    {
        USE_SERIAL.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }

    http.end();

    log_d("total = %d", total);

    // Generate header
    String header;
    header = F("POST /api/values HTTP/1.1\r\n");
    header += F("Content-Type: application/octet-stream");
    header += "\r\n";
    header += F("Accept: */*\r\n");
    header += F("Host: 192.168.0.105:8080");
    header += F("\r\n");
    header += F("accept-encoding: gzip, deflate\r\n");
    header += F("Connection: keep-alive\r\n");
    int content_length = 0;

    message = "Encoding and sending...";

    for (int i = 0, temp = total, memory_to_allocate; temp > 0; i += memory_to_allocate, temp -= memory_to_allocate, transfer_progress += 3)
    {
        size_t base64_chunk_length;
        memory_to_allocate = temp > 9000 ? 9000 : temp;

        uint8_t *tempBuff = (uint8_t *)ps_malloc(memory_to_allocate);
        memcpy(tempBuff, output + i, memory_to_allocate);
        base64_encode(tempBuff, memory_to_allocate, &base64_chunk_length);
        content_length += base64_chunk_length;
        free(tempBuff);
    }

    header += "Content-Length: ";
    header += String(content_length);
    header += "\r\n";
    header += "\r\n";

    log_d("Header:\n%s", header.c_str());

    transfer_progress = 50;

    WiFiClientSecure client;
    client.setNoDelay(true);

    if (!client.connect("192.168.0.105", 443, INT32_MAX))
    {
        log_e("Connection failed");
    }

    client.print(header);

    transfer_progress = 75;

    for (int i = 0, memory_to_allocate; total > 0; i += memory_to_allocate, total -= memory_to_allocate)
    {
        size_t base64_chunk_length;
        memory_to_allocate = total > 9000 ? 9000 : total;
        log_v("memory_to_allocate = %d", memory_to_allocate);

        uint8_t *tempBuff = (uint8_t *)ps_malloc(memory_to_allocate);
        memcpy(tempBuff, output + i, memory_to_allocate);

        char *base64_output = base64_encode(tempBuff, memory_to_allocate, &base64_chunk_length);
        log_v("base64_chunk_length = %d", base64_chunk_length);
        // log_d("%s", base64_output);
        client.print(base64_output);
        free(tempBuff);
        free(base64_output);
        delay(10);
    }

    // client.flush();
    client.stop();

    message = "// Response";
    transfer_progress = 100;
    delay(2000);

    transfer_progress = 0;

    free(output);
    vTaskSuspend(NULL);
}