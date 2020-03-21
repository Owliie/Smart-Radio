#ifndef __TASK_FUNCTIONS_H__
#define __TASK_FUNCTIONS_H__

#include <string>

#include "base64_encode_psram.h"
#include "definitions.h"
#include "setup.h"
#include "stream_callback.h"
#include "utilities.h"


void drive_oled(void *);
void drive_piezo(void *);
void record_snippet(void *);

#endif