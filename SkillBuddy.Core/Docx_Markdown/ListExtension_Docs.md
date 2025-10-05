# ListExtension.cs – Full Documentation

## Overview
The `ListExtension` class provides extension methods for converting generic lists into comma-separated string (CSV) representations.  
It is defined as a static class in the `SkillBuddy.Core` namespace and supports numeric, enum, string, char, and Guid types.

This utility simplifies SQL-style value formatting, logging, and text exports.

---

## ⚙️ Method

### 🔹 ToCsv
```csharp
public static string ToCsv<T>(this List<T>? list)
```
Converts a list of items into a CSV-formatted string.

**Parameters:**
- `list` – The list to convert. Can be null or empty.

**Returns:**
- A CSV string representation of the list.  
- Returns `String.Empty` for null or empty lists.  
- Wraps strings and GUIDs in single quotes.  
- Enums are automatically handled based on their underlying type.

**Supported Types:**
- Numeric (`int`, `long`, `float`, `decimal`, etc.)
- Enums (both numeric and string-based)
- `string`, `char`
- `Guid`

---

## 🧠 Logic Summary

1. **Null or Empty Check**
   ```csharp
   if (list == null || list.Count == 0)
       return String.Empty;
   ```

2. **Enum Handling**
   - Detects if the list type is an enum.
   - Determines its underlying numeric type using `Enum.GetUnderlyingType()`.
   - Produces either numeric or quoted CSV values.

3. **Numeric Handling**
   - Uses `Type.GetTypeCode()` to detect numeric types.
   - Includes all signed/unsigned integer and floating-point variations.

4. **Character & String Handling**
   - Wraps each value in single quotes.

5. **GUID Handling**
   - Converts each GUID to string and wraps in quotes.

6. **Fallback**
   - Returns `String.Empty` for unsupported types.

---

## ✅ Usage Examples

### Example 1 – Numeric List
```csharp
var numbers = new List<int> { 1, 2, 3 };
string result = numbers.ToCsv(); // Output: 1,2,3
```

### Example 2 – String List
```csharp
var names = new List<string> { "Alice", "Bob" };
string result = names.ToCsv(); // Output: 'Alice','Bob'
```

### Example 3 – GUID List
```csharp
var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
string result = ids.ToCsv(); // Output: 'guid1','guid2'
```

### Example 4 – Enum List
```csharp
enum Status { Active, Inactive }
var statuses = new List<Status> { Status.Active, Status.Inactive };
string result = statuses.ToCsv(); // Output: 'Active','Inactive'
```

---

## ⚡ Best Practices

1. **Use only for simple formatting needs.**
   - This method is ideal for CSV creation or building SQL `IN (...)` queries.

2. **Avoid for large data exports.**
   - For thousands of records, use a proper CSV library like CsvHelper.

3. **Consistent C# conventions**
   - `string` for variables and types.  
   - `String` for static class references (`String.Empty`, `String.Join`).

4. **Immutable Logic**
   - `ToCsv()` never modifies the original list.  
   - Safe for parallel and concurrent usage.

---

## ⚙️ Implementation Summary
```csharp
public static string ToCsv<T>(this List<T>? list)
{
    if (list == null || list.Count == 0)
        return String.Empty;

    Type type = typeof(T);

    // Enum Handling
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

    // Numeric Handling
    if (Type.GetTypeCode(type) is TypeCode.Byte or
        TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or
        TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or
        TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal)
    {
        return String.Join(",", list.Select(x => x?.ToString() ?? String.Empty));
    }

    // String/Char Handling
    if (type == typeof(char) || type == typeof(string))
        return String.Join(",", list.Select(x => $"'{x?.ToString() ?? String.Empty}'"));

    // GUID Handling
    if (type == typeof(Guid))
        return String.Join(",", list.Select(x => $"'{x?.ToString() ?? String.Empty}'"));

    // Fallback
    return String.Empty;
}
```

---

## 🧩 Summary

| Type Category | Behavior | Example Output |
|----------------|-----------|----------------|
| Numeric | Plain values | `1,2,3` |
| String / Char | Quoted | `'A','B','C'` |
| GUID | Quoted | `'guid1','guid2'` |
| Enum | Quoted or numeric | `'Active','Inactive'` or `1,2` |
| Null / Empty | Returns `String.Empty` | *(empty string)* |

---
