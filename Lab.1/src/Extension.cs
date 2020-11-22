using System;
using System.Globalization;

namespace Lab._1
{
    public static class Extension
    {
        public static void Add(this int[] vertex, int i, string[] parts, int index)
        {
            if(parts.Length < index) return;

            var success = int.TryParse(parts[index], out var vIndex);
            if (!success) throw new ArgumentException("Could not parse parameter as int");

            vertex[i] = vIndex;
        }

        public static decimal GetDecimalByIndex(this string[] data, int index, string parameterNaming = "")
        {
            if (data.Length < index) return default;

            var success = decimal.TryParse(data[index], NumberStyles.Any, CultureInfo.InvariantCulture, out var x);
            if (!success) throw new ArgumentException($"Could not parse {parameterNaming} parameter as decimal");

            return x;
        }
    }
}