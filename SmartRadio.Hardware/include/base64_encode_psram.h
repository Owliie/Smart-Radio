#ifndef __BASE64_ENCODE_PSRAM_H__
#define __BASE64_ENCODE_PSRAM_H__

#include <math.h>
#include <stdint.h>
#include <stdlib.h>
#include "esp32-hal-psram.h"

size_t get_output_length(size_t);
char *base64_encode(const unsigned char *, size_t, size_t *);
#endif