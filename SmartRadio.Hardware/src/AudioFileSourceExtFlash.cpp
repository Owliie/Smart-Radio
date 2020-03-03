#include "AudioFileSourceExtFlash.h"

#include "AudioFileSourceExtFlash.h"

AudioFileSourceExtFlash::AudioFileSourceExtFlash(const char *filename)
{
    open(filename);
}

bool AudioFileSourceExtFlash::open(const char *filename)
{
    f = fopen(filename, "rb");
    return f;
}

AudioFileSourceExtFlash::~AudioFileSourceExtFlash()
{
    if (f)
        fclose(f);
    f = NULL;
}

uint32_t AudioFileSourceExtFlash::read(void *data, uint32_t len)
{
    int ret = fread(reinterpret_cast<uint8_t *>(data), 1, len, f);
    return ret;
}

bool AudioFileSourceExtFlash::seek(int32_t pos, int dir)
{
    return fseek(f, pos, dir);
}

bool AudioFileSourceExtFlash::close()
{
    fclose(f);
    f = NULL;
    return true;
}

bool AudioFileSourceExtFlash::isOpen()
{
    return f ? true : false;
}

uint32_t AudioFileSourceExtFlash::getSize()
{
    if (!f)
        return 0;
    uint32_t p = ftell(f);
    fseek(f, 0, SEEK_END);
    uint32_t len = ftell(f);
    fseek(f, p, SEEK_SET);
    return len;
}

uint32_t AudioFileSourceExtFlash::getPos()
{

    if (!f)
    {
        return 0;
    }
    else
    {
        return (uint32_t)ftell(f);
    }
}
