using System;

namespace GuideHoundEPG.Common.Model.XmlTv {
    public static class DateTools {
        
        /// <summary>
        /// Convert the string found in the xmltv file into a DateTime object in UTC time
        /// </summary>
        /// <param name="dateString">The xmltv string representing the DateTime</param>
        /// <returns>A new DateTime object in UTC time</returns>
        public static DateTime FormatDate(string dateString) {

            // ensure the date is padded to 20 characters
            dateString = dateString.PadRight(20, '0');

            // get the individual dates
            int year = Int32.Parse(dateString.Substring(0, 4));
            int month = Int32.Parse(dateString.Substring(4, 2));
            int day = Int32.Parse(dateString.Substring(6, 2));
            int hour = Int32.Parse(dateString.Substring(8, 2));
            int min = Int32.Parse(dateString.Substring(10, 2));
            int sec = Int32.Parse(dateString.Substring(12, 2));

            //string op = dateString.Substring(15, 1);
            //string offset = dateString.Substring(16, 4);

            //int offsethour = Int32.Parse(offset.Substring(0, 2));
            //int offsetmin = Int32.Parse(offset.Substring(2, 2));

            // return new DateTime(year, month, day, hour, min, sec).ToUniversalTime();
            DateTime utc = new DateTime(year, month, day, hour, min, sec, DateTimeKind.Local);
            //if (op=="+") {
            //    utc = utc.Add(new TimeSpan(offsethour, offsetmin, 0));
            //} else if (op == "-") {
            //    utc = utc.Subtract(new TimeSpan(offsethour, offsetmin, 0));
            //}

            return utc;
        }

    }
}