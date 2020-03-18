#include "base64_encode_psram.h"

static const unsigned char base64_table[65] =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

size_t get_output_length(size_t input_length)
{
    size_t olen = input_length * 4 / 3 + 4;
    olen += olen / 72;
    olen++;
    return olen;
}

char *base64_encode(const unsigned char *src, size_t len,
                    size_t *out_len)
{
    char *out, *pos;
    const unsigned char *end, *in;
    size_t olen;
    int line_len;

    olen = len * 4 / 3 + 4; /* 3-byte blocks to 4-byte */
    olen += olen / 72;      /* line feeds */
    olen++;                 /* nul termination */
    if (olen < len)
    {
        return NULL;
    }
    out = (char *)ps_malloc(olen);
    if (out == NULL)
    {
        return NULL;
    }

    end = src + len;
    in = src;
    pos = out;
    line_len = 0;
    while (end - in >= 3)
    {
        *pos++ = base64_table[in[0] >> 2];
        *pos++ = base64_table[((in[0] & 0x03) << 4) | (in[1] >> 4)];
        *pos++ = base64_table[((in[1] & 0x0f) << 2) | (in[2] >> 6)];
        *pos++ = base64_table[in[2] & 0x3f];
        in += 3;
        line_len += 4;
        if (line_len >= 72)
        {
            *pos++ = '\n';
            line_len = 0;
        }
    }

    if (end - in)
    {
        *pos++ = base64_table[in[0] >> 2];
        if (end - in == 1)
        {
            *pos++ = base64_table[(in[0] & 0x03) << 4];
            *pos++ = '=';
        }
        else
        {
            *pos++ = base64_table[((in[0] & 0x03) << 4) |
                                  (in[1] >> 4)];
            *pos++ = base64_table[(in[1] & 0x0f) << 2];
        }
        *pos++ = '=';
        line_len += 4;
    }

    if (line_len)
    {
        *pos++ = '\n';
    }

    *pos = '\0';
    if (out_len)
    {
        *out_len = pos - out;
    }
    return out;
}