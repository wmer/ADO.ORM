using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ADO.ORM.Helpers; 
internal static class ByteConverter {
    public static byte[] ToBytes(this object obj) =>
                Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));

    public static T ToObject<T>(this byte[] array) {
        var json = Encoding.ASCII.GetString(array);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static string ToBase64String(this byte[] array) =>
                                        Convert.ToBase64String(array);

    public static bool ToBytes(this string str, out byte[] bytes) {
        if (IsBase64String(str)) {
            bytes = Convert.ChangeType(Convert.FromBase64String(str), typeof(byte[])) as byte[];
            return true;
        }
        bytes = null;
        return false;
    }

    private static bool IsBase64String(string s) {
        if (string.IsNullOrEmpty(s)) return false;
        s = s.Trim();
        return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
    }
}
