#include "alarms.h"

AlarmManager alarm_manager;

Alarm::Alarm(int _hours, int _minutes)
{
    hours = _hours;
    minutes = _minutes;
}

int Alarm::get_hours()
{
    return hours;
}

int Alarm::get_minutes()
{
    return minutes;
}

std::string Alarm::to_string()
{
    std::stringstream s;
    s << pad_left('0', 2, hours) << ":" << pad_left('0', 2, minutes);
    return s.str();
}

void AlarmManager::add_alarm(int hours, int minutes)
{
    alarms.push_back(Alarm(hours, minutes));
    updateAlarmsFile();
}

void AlarmManager::delete_alarm_at(int index)
{
    alarms.erase(alarms.begin() + index);
    updateAlarmsFile();
}

std::vector<Alarm> AlarmManager::get_alarms()
{
    return alarms;
}

int AlarmManager::count()
{
    return alarms.size();
}

void AlarmManager::updateAlarmsFile()
{
    FILE *f = fopen(MOUNT_POINT_FAT "/alarms.txt", "w");

    for(int i = 0; i < count(); i++)
    {
        fprintf(f, "%d %d\n", alarms[i].get_hours(), alarms[i].get_minutes());
    }

}