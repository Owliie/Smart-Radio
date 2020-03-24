#ifndef __UTILITIES_H__
#define __UTILITIES_H__

#include <iomanip>
#include <sstream>

#include <Arduino.h>

#include "definitions.h"

void tone(uint8_t, unsigned int, unsigned long duration = 0, uint8_t channel = SOUND_PWM_CHANNEL);
void noTone(uint8_t, uint8_t channel = SOUND_PWM_CHANNEL);

std::string itos(int);
std::string concat_2(char *, char *);
std::string pad_left(char, int, int);
#endif