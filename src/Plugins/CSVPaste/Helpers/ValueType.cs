using System;
using System.Text;

namespace SsmsLite.CsvPaste.Helpers
{
    /// <summary>
    /// Helper class for working with the value types.
    /// </summary>
    internal static class ValueType
    {
        /// <summary>
        /// Determines the <see cref="ValueTypes"/> of the values and the existence of a header.
        /// </summary>
        /// <param name="values">The raw values.</param>
        /// <param name="header">Flag indicating whether the column header was present in the values or not.</param>
        /// <returns>The determined <see cref="ValueTypes"/> for all the values.</returns>
        public static ValueTypes DetermineValuesType(string[] values, out bool header)
        {
            header = false;

            // Determine the types of the last item (the first item from the back).
            // If the types is text, treat all the values as text.
            var initialType = GetValueType(values[values.Length - 1]);
            if (initialType == ValueTypes.Text)
                return ValueTypes.Text;

            // If the array contains only one element, return its types.
            if (values.Length == 1)
                return initialType;

            // For every item except the first and the last, check if their types matches the initially determined one.
            // If is doesn’t, treat all the values as text.
            for (var i = values.Length - 2; i >= 1; i--)
            {
                if (!EqualsValueType(values[i], initialType))
                {
                    return ValueTypes.Text;
                }
            }

            // Determine the types of the first item.
            // If the first item is text but none of the previous items is, then the selection contains a header.
            var firstItemType = GetValueType(values[0]);
            if (firstItemType == ValueTypes.Text)
            {
                header = true;
                return initialType;
            }

            // If the first item is not text but it still doesn’t match the types of the previous items,
            // then there are different types of items in the selection.
            // Treat all of them as text.
            if (!EqualsValueType(values[0], initialType))
            {
                return ValueTypes.Text;
            }

            return initialType;
        }

        /// <summary>
        /// Gets the <see cref="ValueTypes"/> of the value.
        /// </summary>
        /// <param name="value">The raw value.</param>
        /// <returns>The determined <see cref="ValueTypes"/> for the value.</returns>
        private static ValueTypes GetValueType(string value)
        {
            decimal decimalValue;
            if (decimal.TryParse(value, out decimalValue))
                return ValueTypes.Numeric;

            double doubleValue;
            if (double.TryParse(value, out doubleValue))
                return ValueTypes.Numeric;

            Guid guidValue;
            if (Guid.TryParse(value, out guidValue))
                return ValueTypes.Uniqueidentifier;

            DateTime dateTimeValue;
            if (DateTime.TryParse(value, out dateTimeValue))
                return ValueTypes.DateTime;

            return ValueTypes.Text;
        }

        /// <summary>
        /// Determines whether the value is of particular <see cref="ValueTypes"/>.
        /// </summary>
        /// <param name="value">The raw value.</param>
        /// <param name="valueTypes">The <see cref="ValueTypes"/>.</param>
        /// <returns>true if the <param name="value" /> parameter is of the provided <see cref="ValueTypes"/>.</returns>
        private static bool EqualsValueType(string value, ValueTypes valueTypes)
        {
            switch (valueTypes)
            {
                case ValueTypes.Numeric:
                    decimal decimalValue;
                    var isDecimal = decimal.TryParse(value, out decimalValue);
                    if (isDecimal)
                        return true;

                    double doubleValue;
                    return double.TryParse(value, out doubleValue);
                case ValueTypes.Uniqueidentifier:
                    Guid guidValue;
                    return Guid.TryParse(value, out guidValue);

                case ValueTypes.DateTime:
                    DateTime dateTimeValue;
                    return DateTime.TryParse(value, out dateTimeValue);
            }

            return true;
        }

        /// <summary>
        /// Formats and joins the raw values into a formatted string.
        /// </summary>
        /// <param name="values">The raw values.</param>
        /// <param name="types">The values types.</param>
        /// <returns>A string value consisting of all the values formatted and joined together.</returns>
        public static string JoinValues(string[] values, ValueTypes types)
        {
            var builder = new StringBuilder();

            switch (types)
            {
                case ValueTypes.Uniqueidentifier:
                case ValueTypes.DateTime:
                {
                    builder.Append("'");
                    builder.Append(string.Join("', '", values));
                    builder.Append("'");
                    break;
                }
                case ValueTypes.Numeric:
                {
                    builder.Append(string.Join(", ", values));
                    break;
                }
                default:
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        var fixedValue = values[i].Replace("'", "''");
                        if (i != values.Length - 1)
                        {
                            builder.AppendLine($"N'{fixedValue}',");
                        }
                        else
                        {
                            builder.Append($"N'{fixedValue}'");
                        }
                    }

                    break;
                }
            }

            return builder.ToString();
        }

    }
}