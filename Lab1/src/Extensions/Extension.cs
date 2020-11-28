using System;
using System.Globalization;
using Extreme.Mathematics;

namespace Lab1.Extensions
{
    public static class Extension
    {
        public static void Add(this Vector<int> vertex, int i, string[] parts, int index)
        {
            if(parts.Length < index) return;

            var success = int.TryParse(parts[index], out var result);
            if (!success) throw new ArgumentException("Could not parse parameter as int");

            vertex[i] = result;
        }

        public static float GetFloatByIndex(this string[] data, int index, string parameterNaming = "")
        {
            if (data.Length < index) return default;

            var success = float.TryParse(data[index], NumberStyles.Any, CultureInfo.InvariantCulture, out var x);
            if (!success) throw new ArgumentException($"Could not parse {parameterNaming} parameter as float");

            return x;
        }
    }
}