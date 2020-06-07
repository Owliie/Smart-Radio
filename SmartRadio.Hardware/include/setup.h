#ifndef __SETUP_H__
#define __SETUP_H__

#include <fstream>
#include <regex>
#include <string>

#include "definitions.h"
#include "stream_callback.h"
#include "variables.h"

void setup_external_fat();
void setup_gpio();
void setup_oled();
void setup_wifi();
void setup_ntp();
void setup_audio_transmission();
void identify_owner();

void load_alarms_from_file();

#endif