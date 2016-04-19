using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GuideHoundEPG.Common
{
    public static class ObjectExtensions {

        public static void UpdateFrom(this object destination, object source) {
            
            Type type = source.GetType();

            FieldInfo[] myObjectFields = type.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo fi in myObjectFields) {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }

    }
}
