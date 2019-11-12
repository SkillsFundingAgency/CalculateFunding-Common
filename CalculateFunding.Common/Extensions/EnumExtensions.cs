using System;
using System.Reflection;

namespace CalculateFunding.Common.Extensions
{
    public static class EnumExtensions
    {
        public static TTargetEnum AsMatchingEnum<TTargetEnum>(this Enum value)
            where TTargetEnum : struct
        {
            return value.ToString().AsEnum<TTargetEnum>();
        }

        public static object PropertyMapping<T>(this Enum value, T instance)
        {
            Type genericType = instance.GetType();
            string enumValue = value?.ToString();
            if (string.IsNullOrWhiteSpace(enumValue))
            {
                throw new InvalidOperationException("Null or empty string for field name");
            }

            PropertyInfo propertyInfo = genericType.GetProperty(enumValue);

            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"The field '{enumValue}' was not found on the type '{genericType.FullName}'");
            }

            return propertyInfo.GetValue(instance);
        }

        public static TTargetEnum AsEnum<TTargetEnum>(this string enumLiteral)
            where TTargetEnum : struct
        {
            if (Enum.TryParse(enumLiteral, true, out TTargetEnum targetEnum))
            {
                if (!Enum.IsDefined(typeof(TTargetEnum), targetEnum))
                {
                    throw new ArgumentException($"{enumLiteral} is not an underlying value of the {typeof(TTargetEnum).Name} enumeration.");
                }
            }
            else
            {
                throw new ArgumentException($"{enumLiteral} is not a member of the {typeof(TTargetEnum).Name} enumeration.");
            }

            return targetEnum;
        }
    }
}
