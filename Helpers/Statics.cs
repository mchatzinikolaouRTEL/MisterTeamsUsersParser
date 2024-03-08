using System;

namespace MisterProtypoParser.Helpers
{
    public static class Statics
    {
        public static TimeSpan GetProperDateTime(string SynchroniseValue)
        {
            DateTime DateTimeNow = DateTime.Now;
            string HourFormat = SynchroniseValue.Substring(0, 2);
            string MinuteFormat = SynchroniseValue.Substring(3, 2);
            string SecondFormat = SynchroniseValue.Substring(6, 2);
            string HourValue = SynchroniseValue.Substring(0, 2);
            string MinuteValue = SynchroniseValue.Substring(3, 2);
            string SecondValue = SynchroniseValue.Substring(6, 2);

            if (HourFormat == "##")
            {
                HourValue = DateTimeNow.Hour.ToString("00");
            }

            if (MinuteFormat == "##")
            {
                MinuteValue = DateTimeNow.Minute.ToString("00");
            }
            else if (MinuteFormat.StartsWith("#"))
            {
                MinuteValue = DateTimeNow.Minute.ToString("00").Substring(0, 1) + MinuteValue.Substring(1, 1);
            }

            if (SecondFormat == "##")
            {
                SecondValue = DateTimeNow.Second.ToString("00");
            }

            DateTime SynchroniseDateTimeValue = DateTime.ParseExact(DateTimeNow.Year + "-" + DateTimeNow.Month.ToString("00") + "-" + DateTimeNow.Day.ToString("00") + " " + HourValue + ":" + MinuteValue + ":" + SecondValue, "yyyy-MM-dd HH:mm:ss", null);
            if (SynchroniseDateTimeValue < DateTimeNow)
            {
                double MiliisecondOffset = SynchroniseDateTimeValue.Subtract(DateTimeNow).TotalMilliseconds;
                SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddMilliseconds(MiliisecondOffset);

                if (HourFormat != "##")
                {
                    SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddDays(1);
                    SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddMilliseconds(-MiliisecondOffset);
                }
                else if (MinuteFormat != "##")
                {
                    if (MinuteFormat.StartsWith("#"))
                    {
                        SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddMinutes(10);
                        SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddMilliseconds(-MiliisecondOffset);
                    }
                    else
                    {
                        SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddHours(1);
                        SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddMilliseconds(-MiliisecondOffset);
                    }
                }
                else if (SecondFormat != "##")
                {
                    SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddMinutes(1);
                    SynchroniseDateTimeValue = SynchroniseDateTimeValue.AddMilliseconds(-MiliisecondOffset);
                }
            }
            return SynchroniseDateTimeValue.Subtract(DateTimeNow);
        }
    }
}
