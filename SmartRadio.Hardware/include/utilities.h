#ifndef __UTILITIES_H__
#define __UTILITIES_H__

#include <sstream>
#include "setup.h"

void clear_audio_transmission();
void tone(uint8_t pin, unsigned int frequency, unsigned long duration = 0, uint8_t channel = SOUND_PWM_CHANNEL);
void noTone(uint8_t pin, uint8_t channel = SOUND_PWM_CHANNEL);
std::string itos(int);
#endif