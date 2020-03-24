#include "utilities.h"

void tone(uint8_t pin, unsigned int frequency, unsigned long duration, uint8_t channel)
{
    if (ledcRead(channel))
    {
        log_e("Tone channel %d is already in use", ledcRead(channel));
        return;
    }

    ledcAttachPin(pin, channel);
    ledcWriteTone(channel, frequency);
    if (duration)
    {
        delay(duration);
        noTone(pin, channel);
    }
}

void noTone(uint8_t pin, uint8_t channel)
{
    ledcDetachPin(pin);
    ledcWrite(channel, 0);
}

std::string itos(int i) // convert int to string
{
    std::stringstream s;
    s << i;
    return s.str();
}

std::string concat_2(char *s1, char *s2)
{
    std::stringstream s;
    s << s1 << s2;
    return s.str();
}

std::string pad_left(char pad, int length, int c)
{
    std::stringstream s;
    s << std::setfill(pad) << std::setw(length) << c;
    return s.str();
}