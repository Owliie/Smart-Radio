#ifndef __VARIABLES_H__
#define  __VARIABLES_H__

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
#include "AudioOutputExtFlash.h"
#include "AudioOutputI2S.h"
#include "extflash.h"
#include "fatflash.h"

#include "alarms.h"

extern String identityCookie;

extern std::string message;

extern int transfer_progress;

extern AlarmManager alarm_manager;
extern bool is_setting_time;
extern bool has_set_hours;
extern bool has_set_minutes;

extern int set_hours;
extern int set_minutes;

extern AudioGeneratorMP3 *mp3;
extern AudioFileSourceICYStream *file;

extern AudioFileSourceBuffer *buff;
extern AudioOutputI2S *out;

extern ExtFlash extflash;
extern FatFlash fatflash;

extern WiFiUDP ntpUDP;
extern NTPClient *timeClient;

extern SSD1306Wire *oled;
#endif