#ifndef __TASK_FUNCTIONS_H__
#define __TASK_FUNCTIONS_H__

#include <string>

#include "alarms.h"
#include "base64_encode_psram.h"
#include "definitions.h"
#include "setup.h"
#include "stream_callback.h"
#include "utilities.h"

void create_new_alarm(void *);
void drive_alarm_manager(void *);
void drive_oled(void *);
void record_snippet(void *);

#endif