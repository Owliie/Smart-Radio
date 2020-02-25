#ifndef __UTILITIES_H__
#define __UTILITIES_H__
#include <string>
#include <vector>

void ledcAnalogWrite(uint8_t, uint32_t, uint32_t);
void split(std::string &, const char, std::vector<std::string> &);
const std::string get_extension(char *);
#endif