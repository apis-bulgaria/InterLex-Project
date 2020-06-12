namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Diagnostics;
    using Enums;

    /// <summary>
    /// Represents period by year and month
    /// </summary>
    [DebuggerDisplay("{Year} - {Month}")]
    public struct YearMonth : IEquatable<YearMonth>, IComparable<YearMonth>
    {
        /// <summary>
        /// Creates YearMonth from the provied date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static YearMonth Create(DateTime date)
        {
            return new YearMonth { Month = (Month)date.Month, Year = date.Year };
        }

        public static YearMonth Create(int year, int month)
        {
            return new YearMonth { Month = (Month)month, Year = year };
        }

        public bool In(YearMonth start, YearMonth end)
        {
            return this.CompareTo(start) >= 0 && this.CompareTo(end) <= 0;
        }

        public bool InByYear(YearMonth start, YearMonth end)
        {
            return this.Year >= start.Year && this.Year <= end.Year;
        }

        /// <summary>
        /// Returns the relevant period for the today's date
        /// </summary>
        /// <returns></returns>
        public static YearMonth Now() => Create(DateTime.Now);

        /// <summary>
        /// Returns date of the start of the month
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static DateTime ToStartDate(YearMonth period) => new DateTime(
            year: period.Year,
            month: (int)period.Month,
            day: 1);

        /// <summary>
        /// Returns date of the end of the month
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static DateTime ToEndDate(YearMonth period) => new DateTime(
            year: period.Year,
            month: (int)period.Month,
            day: DateTime.DaysInMonth(period.Year, (int)period.Month));

        /// <summary>
        /// Year of the period
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Month of the period
        /// </summary>
        public Month Month { get; set; }


        public bool Equals(YearMonth other)
        {
            return this.Year == other.Year && this.Month == other.Month;
        }

        public int CompareTo(YearMonth other)
        {
            if (this.Year == other.Year)
            {
                if (this.Month == other.Month)
                {
                    return 0;
                }
                else if (this.Month > other.Month)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (this.Year > other.Year)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is YearMonth)
            {
                return ((YearMonth)obj).Equals(this);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = hash * 23 + this.Year.GetHashCode();
            hash = hash * 23 + ((int)this.Month).GetHashCode();

            return hash;
        }
    }
}
