using System;
using System.Collections;
using System.Reflection;

namespace GuideHoundEPG.Common.Model.MXF {
    public static class MxfFileExtension
    {

        public static String GetDelimitedString(this IList list, String fieldName) {
            string s = "";

            foreach (var item in list) {

                PropertyInfo prop = item.GetType().GetProperty(fieldName);
                String propValue = prop.GetValue(item, null).ToString();
                
                if (s.Length == 0) {
                    s += propValue;
                } else{
                    s += "," + propValue;
                }
            }

            return s;
        }


        public static String ToMxfDate(this DateTime dateTime) {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static String ToMxfBoolean(this Boolean boolean) {
            return boolean ? "true" : "false";
        }
    }
}