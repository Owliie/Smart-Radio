#ifndef __AUDIOOUTPUTEXTFLASH_H__
#define __AUDIOOUTPUTEXTFLASH_H__

#include "AudioOutput.h"

class AudioOutputExtFlash : public AudioOutput
{
    public:
    AudioOutputExtFlash() { filename = NULL; f = NULL; };
    ~AudioOutputExtFlash() { free(filename); };
    virtual bool begin() override;
    virtual bool ConsumeSample(int16_t sample[2]) override;
    virtual bool stop() override;
    void SetFilename(const char *name);

  private:
    FILE *f;
    char *filename;
};

#endif