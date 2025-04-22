namespace CarsHistory.Extentions;

public static class DateTimeExtentions
{
    public static DateTime ToUtcSafe(this DateTime date, bool needAddDay = false)
    {
        int day = date.Day;

        DateTime res = date.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(date.Date, DateTimeKind.Local).ToUniversalTime() : date.ToUniversalTime();
        
        if (!needAddDay)
            return res;
        
        return res.Day != day ? res.AddDays(1) : res;
    }
}