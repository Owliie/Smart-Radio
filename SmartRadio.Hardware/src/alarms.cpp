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

void AlarmManager::add_alarm(Alarm a)
{
    if (!contains(a))
    {
        alarms.push_back(a);
        sort();
        update_alarms_file();
    }
}

void AlarmManager::delete_alarm_at(int index)
{
    alarms.erase(alarms.begin() + index);
    sort();
    update_alarms_file();
}

std::vector<Alarm> AlarmManager::get_alarms()
{
    return alarms;
}

bool AlarmManager::contains(Alarm el)
{
    for (Alarm a : alarms)
    {
        if (a.get_hours() == el.get_hours() && a.get_minutes() == el.get_minutes())
        {
            return true;
        }
    }

    return false;
}

void AlarmManager::sort()
{
    std::sort(
        alarms.begin(), alarms.end(), [](Alarm a, Alarm b) {
            if (a.get_hours() == b.get_hours())
            {
                return a.get_minutes() < b.get_minutes();
            }

            return a.get_hours() < b.get_hours();
        });
}

int AlarmManager::count()
{
    return alarms.size();
}

void AlarmManager::update_alarms_file()
{
    std::ofstream out;

    out.open(MOUNT_POINT_FAT "/alarms.txt");

    if (!out.fail())
    {
        for (int i = 0; i < count(); i++)
        {
            Alarm a = alarms[i];
            out << a.get_hours() << " " << a.get_minutes() << std::endl;
        }
    }
    else
    {
        log_e("Error opening file (ofstream)");
    }

    out.close();
}