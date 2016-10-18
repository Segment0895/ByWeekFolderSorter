using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpador
{
    public class Testes
    {

        public Testes()
        {
            

            Calendar cal = CultureInfo.InvariantCulture.Calendar;
            // 1/1/1990 starts on a Monday
            DateTime dt = new DateTime(1990, 1, 1);
            Console.WriteLine("Starting at " + dt + " day: " + cal.GetDayOfWeek(dt) + " Week: " + WeekSorter.WeeksInYear(dt));

            for (int i = 0; i < 100000; i++)
            {
                for (int days = 0; days < 7; dt = dt.AddDays(1), days++)
                {
                    if (WeekSorter.WeeksInYear(dt) != cal.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    {
                        Console.WriteLine("Iso Week " + WeekSorter.WeeksInYear(dt) +
                            " GetWeekOfYear: " + cal.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) +
                            " Date: " + dt + " Day: " + cal.GetDayOfWeek(dt));
                    }
                }
            }
        }
    }
}
