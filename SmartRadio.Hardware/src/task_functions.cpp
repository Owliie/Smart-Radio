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

        oled->display();

        vTaskDelay(500);
    }
}

void record_snippet(void *)
{
    remove(MOUNT_POINT_FAT "/snippet.mp3");
    HTTPClient http;

    USE_SERIAL.print("[HTTP] begin...\n");

    // configure server and url
    http.begin(STREAM_URL);
    //http.begin("192.168.1.12", 80, "/test.html");

    USE_SERIAL.print("[HTTP] GET...\n");
    // start connection and send HTTP header
    int httpCode = http.GET();
    // create buffer for read
    uint8_t buff[150] = {0};
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
                // get available data size
                size_t size = stream->available();

                log_d("Stream size: %d (total = %d)", size, total);

                if (size)
                {
                    // read up to 150 byte
                    int c = stream->readBytes(buff, ((size > 150) ? 150 : size));
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

    log_d("Header:\n%s", header.c_str());

    WiFiClientSecure client;
    client.setNoDelay(true);

    if (!client.connect("192.168.0.105", 443, INT32_MAX))
    {
        log_e("Connection failed");
    }

    int content_length = 0;

    for (int i = 0, temp = total, allocation_size; temp > 0; i += allocation_size, temp -= allocation_size)
    {
        size_t outToSend;
        allocation_size = temp > 9000 ? 9000 : temp;

        uint8_t *tempBuff = (uint8_t *)ps_malloc(allocation_size);
        memcpy(tempBuff, output + i, allocation_size);
        base64_encode(tempBuff, allocation_size, &outToSend);
        content_length += outToSend;
        free(tempBuff);
    }

    header += "Content-Length: ";
    header += String(content_length);
    header += "\r\n";
    header += "\r\n";

    client.print(header);

    for (int i = 0, allocation_size; total > 0; i += allocation_size, total -= allocation_size)
    {
        size_t outToSend;
        allocation_size = total > 9000 ? 9000 : total;
        log_d("allocation_size = %d", allocation_size);

        uint8_t *tempBuff = (uint8_t *)ps_malloc(allocation_size);
        memcpy(tempBuff, output + i, allocation_size);

        char *base64_output = base64_encode(tempBuff, allocation_size, &outToSend);
        log_d("outToSend = %d", outToSend);
        // log_d("%s", base64_output);
        client.print(base64_output);
        free(tempBuff);
        free(base64_output);
        delay(10);
    }

    // client.flush();
    client.stop();

    free(output);
    vTaskSuspend(NULL);
}