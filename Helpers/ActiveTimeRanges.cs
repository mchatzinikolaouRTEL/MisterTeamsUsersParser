using RtelLibrary.DataBaseObjects;
using RtelLibrary.TableModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisterProtypoParser.Helpers
{
    public class ActiveTimeRanges
    {
        private readonly Dictionary<DayOfWeek, List<ActiveTimeRange>> activeTimeRanges = new();
        public ActiveTimeRanges()
        {
            activeTimeRanges.Add(DayOfWeek.Monday, new() { new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero } });
            activeTimeRanges.Add(DayOfWeek.Tuesday, new() { new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero } });
            activeTimeRanges.Add(DayOfWeek.Wednesday, new() { new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero } });
            activeTimeRanges.Add(DayOfWeek.Thursday, new() { new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero } });
            activeTimeRanges.Add(DayOfWeek.Friday, new() { new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero } });
            activeTimeRanges.Add(DayOfWeek.Saturday, new() { new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero } });
            activeTimeRanges.Add(DayOfWeek.Sunday, new() { new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero } });
        }

        public bool ICanRun(IEnumerable<SysParameters> parameters)
        {
            //TODO: also here we have to check if another proccess forbit the run. For example if phoneuser table is syncing CDR Details parser must not run
            DateTime DateTimeNow = DateTime.Now;

            SetConditions(parameters);

            foreach (ActiveTimeRange activeTimeRange in activeTimeRanges[DateTimeNow.DayOfWeek])
                if (TimeSpan.Compare(DateTimeNow.TimeOfDay,activeTimeRange.From) >= 0 && TimeSpan.Compare(DateTimeNow.TimeOfDay, activeTimeRange.To) <= 0)
                    return true;
            return false;
        }

        private void SetConditions(IEnumerable<SysParameters> parameters)
        {
            //TODO: also here we have to get data if another proccess forbit the run. For example if phoneuser table is syncing CDR Details parser must not run
            if (parameters is null || parameters.Count() == 0 || !(parameters.Where(x => x.ParamName.Contains(ParameterNames.ActiveTimeRange)).Any()))
            {
                foreach (KeyValuePair<DayOfWeek, List<ActiveTimeRange>> activeTimeRange in activeTimeRanges)
                {
                    List<ActiveTimeRange> activeTimeRanges = new();
                    activeTimeRanges.Add(new ActiveTimeRange() { From = TimeSpan.Zero, To = TimeSpan.Zero });
                }
                return;
            }

            foreach (SysParameters parameter in parameters.Where(x => x.ParamName.Contains(ParameterNames.ActiveTimeRange)))
            {
                List<string> fromToHours = parameter.ParamValue.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
                ActiveTimeRange activeTimeRange = new ()
                {
                    From = DateTime.ParseExact(fromToHours[0], "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay,
                    To = DateTime.ParseExact(fromToHours[1], "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay
                };
                switch (parameter.ParamName)
                {
                    case string a when a.Contains("Monday"):                        
                        activeTimeRanges[DayOfWeek.Monday].Add(activeTimeRange);
                        break;
                    case string a when a.Contains("Tuesday"):
                        activeTimeRanges[DayOfWeek.Tuesday].Add(activeTimeRange);
                        break;
                    case string a when a.Contains("Wednesday"):
                        activeTimeRanges[DayOfWeek.Wednesday].Add(activeTimeRange);
                        break;
                    case string a when a.Contains("Thursday"):
                        activeTimeRanges[DayOfWeek.Thursday].Add(activeTimeRange);
                        break;
                    case string a when a.Contains("Friday"):                        
                        activeTimeRanges[DayOfWeek.Friday].Add(activeTimeRange);
                        break;
                    case string a when a.Contains("Saturday"):
                        activeTimeRanges[DayOfWeek.Saturday].Add(activeTimeRange);
                        break;
                    case string a when a.Contains("Sunday"):
                        activeTimeRanges[DayOfWeek.Sunday].Add(activeTimeRange);
                        break;
                    default:
                        activeTimeRanges[DayOfWeek.Monday].Add(activeTimeRange);
                        activeTimeRanges[DayOfWeek.Tuesday].Add(activeTimeRange);
                        activeTimeRanges[DayOfWeek.Wednesday].Add(activeTimeRange);
                        activeTimeRanges[DayOfWeek.Thursday].Add(activeTimeRange);
                        activeTimeRanges[DayOfWeek.Friday].Add(activeTimeRange);
                        activeTimeRanges[DayOfWeek.Saturday].Add(activeTimeRange);
                        activeTimeRanges[DayOfWeek.Sunday].Add(activeTimeRange);
                        break;
                }
            }
        }
    }

    sealed internal class ActiveTimeRange
    {
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }
}
