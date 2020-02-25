#include <Arduino.h>
#include <string>
#include <vector>

#include "utilities.h"

void ledcAnalogWrite(uint8_t channel, uint32_t value, uint32_t valueMax = 255) {
  // calculate duty, 8191 from 2 ^ 13 - 1
  uint32_t duty = (8191 / valueMax) * min(value, valueMax);

  // write duty to LEDC
  ledcWrite(channel, duty);
}

void split(std::string &str, const char delim,
           std::vector<std::string> &out)
{
    size_t start;
    size_t end = 0;

    while ((start = str.find_first_not_of(delim, end)) != std::string::npos)
    {
        end = str.find(delim, start);
        out.push_back(str.substr(start, end - start));
    }
}

const std::string get_extension(char * url)
{
    std::string cppStr(url);
    std::vector<std::string> tokens;
    split(cppStr, '.', tokens);
    return tokens.back();
}