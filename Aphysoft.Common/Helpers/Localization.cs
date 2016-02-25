using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Common
{
    public class Localization
    {
        private string[] monthLong;

        public string[] MonthLong
        {
            get { return monthLong; }
        }

        private string[] dayLong;

        public string[] DayLong
        {
            get { return dayLong; }
        }

        private string[] monthShort;

        public string[] MonthShort
        {
            get { return monthShort; }
        }

        private string[] dayShort;

        public string[] DayShort
        {
            get { return dayShort; }
        }

        public Localization()
        {
            monthLong = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            dayLong = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            monthShort = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            dayShort = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        }
    }
}
