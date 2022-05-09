using System;
using System.Collections.Generic;
using System.Linq;

namespace SsmsLite.CsvPaste.Helpers
{
    /// <summary>
    /// Helper class for formatting the raw clipboard text.
    /// </summary>
    internal static class Text
    {
        /// <summary>
        /// Gets the values as a formatted comma separated list.
        /// </summary>
        /// <param name="text">The raw clipboard text.</param>
        /// <returns>The final formatted string ready to be pasted.</returns>
        public static string GetFormattedText(string text)
        {
            var allValues = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var values = RemoveNullValues(allValues);
            var type = ValueType.DetermineValuesType(values, out var header);

            if (!header)
            {
                return ValueType.JoinValues(values, type);
            }

            var valuesWithoutHeader = new string[values.Length - 1];
            Array.Copy(values, 1, valuesWithoutHeader, 0, values.Length - 1);

            return ValueType.JoinValues(valuesWithoutHeader, type);
        }

        /// <summary>
        /// Returns a list without the NULL values.
        /// </summary>
        /// <param name="values">The raw values.</param>
        /// <returns>A new array without the NULL values.</returns>
        private static string[] RemoveNullValues(IEnumerable<string> values)
        {
            return values.Where(value => !value.Equals("NULL")).ToArray();
        }
    }
}