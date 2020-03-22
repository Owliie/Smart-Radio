#ifndef __ALARMS_H__
#define __ALARMS_H__

#include <fstream>
#include <sstream>
#include <string>
#include <vector>

#include <Arduino.h>

#include "definitions.h"
#include "utilities.h"

class Alarm
{
public:
    Alarm(int, int);
    int get_hours();
    int get_minutes();
    std::string to_string();

private:
    int hours;
    int minutes;
};

class AlarmManager
{
public:
    void add_alarm(int, int);
    void delete_alarm_at(int);
    std::vector<Alarm> get_alarms();
    void sort();
    int count();
private:
    std::vector<Alarm> alarms;
    void update_alarms_file();
};

#endif