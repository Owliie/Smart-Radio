#include "AudioOutputExtFlash.h"

void AudioOutputExtFlash::SetFilename(const char *name)
{
    free(filename);
    filename = strdup(name);
}

bool AudioOutputExtFlash::begin()
{
    if (f)
    {
        return false;
    }

    unlink(filename);
    f = fopen(filename, "wb+");

    if (!f)
    {
        return false;
    }

    return true;
}

bool AudioOutputExtFlash::ConsumeSample(int16_t sample[2])
{
    log_d("Sample consumed for ExtFlash\n");

    for (int i = 0; i < channels; i++)
    {
        if (bps == 8)
        {
            uint8_t l = sample[i] & 0xff;
            fwrite(&l, sizeof(l), 1, f);
        }
        else
        {
            uint8_t l = sample[i] & 0xff;
            uint8_t h = (sample[i] >> 8) & 0xff;
            fwrite(&l, sizeof(l), 1, f);
            fwrite(&h, sizeof(h), 1, f);
        }
    }
    return true;
}

bool AudioOutputExtFlash::stop()
{
    fclose(f);
    return true;
}