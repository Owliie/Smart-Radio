#include "utilities.h"
#include "variables.h"

void clear_audio_transmission()
{
    delete mp3;
    delete file;
    delete buff;
    delete out;
}
