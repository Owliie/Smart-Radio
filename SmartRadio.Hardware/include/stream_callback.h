#ifndef __STREAM_CALLBACK_H__
#define __STREAM_CALLBACK_H__

#include "variables.h"

void metadata_callback(void *, const char *, bool, const char *);
void status_callback(void *, int, const char *);

#endif