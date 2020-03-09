#include "stream_callback.h"

void metadata_callback(void *cbData, const char *type, bool isUnicode, const char *string)
{
    const char *ptr = reinterpret_cast<const char *>(cbData);
    char s1[32], s2[64];
    strncpy_P(s1, type, sizeof(s1));
    s1[sizeof(s1) - 1] = 0;
    strncpy_P(s2, string, sizeof(s2));
    s2[sizeof(s2) - 1] = 0;
    log_i("METADATA(%s) '%s' = '%s'\n", ptr, s1, s2);
    message = std::string(s2);
    log_d("Text length on OLED: %d\n", oled->getStringWidth(s2));
    Serial.flush();
}

void status_callback(void *cbData, int code, const char *string)
{
    const char *ptr = reinterpret_cast<const char *>(cbData);
    char s1[64];
    strncpy_P(s1, string, sizeof(s1));
    s1[sizeof(s1) - 1] = 0;
    log_w("STATUS(%s) '%d' = '%s'\n", ptr, code, s1);
    Serial.flush();
}