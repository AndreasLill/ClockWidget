using System;
using System.Text.RegularExpressions;

namespace ClockWidget
{
    public static class Helper
    {
        /// <summary>
        /// Clamps a value between min and max.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>The clamped value</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;

            return value;
        }

        /// <summary>
        /// Filters a string using regex and remove other characters.
        /// </summary>
        /// <param name="text">Text to filter</param>
        /// <param name="regex">RegEx to filter by</param>
        /// <returns>The filtered string</returns>
        public static string FilterByRegex(string text, string regex)
        {
            string filtered = text;

            foreach (char ch in text)
            {
                if (!Regex.IsMatch(Char.ToString(ch), regex))
                {
                    filtered = filtered.Replace(Char.ToString(ch), "");
                }
            }

            return filtered;
        }
    }
}
