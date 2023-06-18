using Bloodstone.API;
using ProjectM;

namespace QuickBrazier.Server;

public class DayNightCycleTracker
{
    public delegate void Event(TimeOfDay timeOfDay);

    public event Event OnTimeOfDayChanged;

    private TimeOfDay _previousTimeOfDay;

    public void Update(DayNightCycle dayNightCycle)
    {
        if (!VWorld.Server.IsCreated) return;
        if (dayNightCycle.TimeOfDay == _previousTimeOfDay) return;
        _previousTimeOfDay = dayNightCycle.TimeOfDay;
        OnTimeOfDayChanged?.Invoke(dayNightCycle.TimeOfDay);
    }
}