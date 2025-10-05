using System;
using System.Collections.Generic;
using System.Linq;

namespace SkillBuddy.Core
{
  /// <summary>
  /// Provides extension methods for converting lists into comma-separated strings (CSV).
  /// </summary>
  public static class ListExtension
  {
    /// <summary>
    /// Converts a list to a CSV-formatted string.
    /// Supports numeric, enum, string, char, and Guid types.
    /// </summary>
    /// <typeparam name="T">List element type.</typeparam>
    /// <param name="list">The list to convert.</param>
    /// <returns>
    /// A comma-separated string representation of the list,
    /// with quotes applied for string-like types.
    /// Returns <see cref="String.Empty"/> for null or empty lists.
    /// </returns>
    public static string ToCsv<T>(this List<T>? list)
    {
      if (list == null || list.Count == 0)
        return String.Empty;

      Type type = typeof(T);

      // 🔹 Enums
      if (type.IsEnum)
      {
        Type underlyingType = Enum.GetUnderlyingType(type);
        bool isNumericEnum = Type.GetTypeCode(underlyingType) switch
        {
          TypeCode.Byte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or
          TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 => true,
          _ => false
        };

        return isNumericEnum
            ? String.Join(",", list.Select(x => x?.ToString() ?? String.Empty))
            : String.Join(",", list.Select(x => $"'{x?.ToString() ?? String.Empty}'"));
      }

      // 🔹 Numeric types
      if (Type.GetTypeCode(type) is TypeCode.Byte or
          TypeCode.Int16 or
          TypeCode.Int32 or
          TypeCode.Int64 or
          TypeCode.SByte or
          TypeCode.UInt16 or
          TypeCode.UInt32 or
          TypeCode.UInt64 or
          TypeCode.Single or
          TypeCode.Double or
          TypeCode.Decimal)
      {
        return String.Join(",", list.Select(x => x?.ToString() ?? String.Empty));
      }

      // 🔹 Character or string
      if (type == typeof(char) || type == typeof(string))
        return String.Join(",", list.Select(x => $"'{x?.ToString() ?? String.Empty}'"));

      // 🔹 Guid
      if (type == typeof(Guid))
        return String.Join(",", list.Select(x => $"'{x?.ToString() ?? String.Empty}'"));

      // 🔹 Unsupported types
      return String.Empty;
    }
  }
}
