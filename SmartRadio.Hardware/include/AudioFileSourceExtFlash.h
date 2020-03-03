#ifndef __AUDIOFILESOURCEEXTFLASH_H__
#define __AUDIOFILESOURCEEXTFLASH_H__

#include "AudioFileSource.h"
class AudioFileSourceExtFlash : public AudioFileSource
{
public:
    AudioFileSourceExtFlash();
    AudioFileSourceExtFlash(const char *filename);
    virtual ~AudioFileSourceExtFlash() override;

    virtual bool open(const char *filename) override;
    virtual uint32_t read(void *data, uint32_t len) override;
    virtual bool seek(int32_t pos, int dir) override;
    virtual bool close() override;
    virtual bool isOpen() override;
    virtual uint32_t getSize() override;
    virtual uint32_t getPos() override;

private:
    FILE *f;
};
#endif