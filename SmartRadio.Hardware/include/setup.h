#ifndef __SETUP_H__
#define __SETUP_H__

#include <Arduino.h>
#include <FS.h>
#include <SD.h>
#include <SPI.h>
#include <SPIFFS.h>
#include <time.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>

#include <string>

#include <SSD1306Wire.h>
#include <NTPClient.h>
#include "AudioFileSourceExtFlash.h"
#include "AudioFileSourceICYStream.h"
#include "AudioFileSourceBuffer.h"
#include "AudioGeneratorMP3.h"
#include "AudioOutputI2S.h"
#include "extflash.h"
#include "fatflash.h"

// int last_read_volume = 0;

extern std::string message;

extern AudioGeneratorMP3 *mp3;
extern AudioFileSourceICYStream *file;

extern AudioFileSourceBuffer *buff;
extern AudioOutputI2S *out;

extern ExtFlash extflash;
extern FatFlash fatflash;

extern WiFiUDP ntpUDP;
extern NTPClient *timeClient;

extern SSD1306Wire *oled;

void setup_external_fat();
void setup_gpio();
void setup_oled();
void setup_wifi();
void setup_ntp();
void setup_audio_transmission();

#endif