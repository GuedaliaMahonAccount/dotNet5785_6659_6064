using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Helpers;
internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in typeof(T).GetProperties())
        {
            var value = item.GetValue(t, null);
            str += item.Name + ": ";
            if (value is not string && value is IEnumerable)
            {
                str += "\n";
                foreach (var it in (IEnumerable<object>)value)
                {
                    str += it.ToString() + '\n';
                }
            }
            else
                str += value?.ToString() + '\n';
        }
        return str;
    }
}
