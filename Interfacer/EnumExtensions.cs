using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfacer
{
    public static class EnumExtentions
    {
        /// <summary>
        /// Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="variable">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable == null)
                return false;

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentException(
                    $"Enumeration type mismatch.  The flag is of type '{value.GetType()}', was expecting '{variable.GetType()}'.");
            }

            var num = Convert.ToUInt64(value);
            return (Convert.ToUInt64(variable) & num) == num;

        }

    }
}
