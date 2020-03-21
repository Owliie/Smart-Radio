#include "task_functions.h"
#define USE_SERIAL Serial

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
        else
        {
            oled->drawString(0, 10, "(no data available)");
        }

        if (transfer_progress != 0)
        {
            oled->drawProgressBar(0, 25, 126, 5, transfer_progress);
        }

        oled->display();

        vTaskDelay(250);
    }
}

void record_snippet(void *)
{
    message = "Recording...";
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
            while (http.connected() && (len > 0 || len == -1) && total < 250000)
            {
                transfer_progress = (total / 250000.0) * 50;
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