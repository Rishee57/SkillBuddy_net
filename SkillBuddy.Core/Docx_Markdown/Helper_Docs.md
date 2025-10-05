# Helper.cs – Full Documentation

## Overview
The `Helper` class provides a collection of static utility methods for the SkillBuddy project.  
It covers common tasks such as:

- Value handling and conversions  
- Working with lists and dictionaries  
- Handling Unix timestamps  
- Importing data from Excel and CSV files  
- Exporting data to CSV  
- File and stream utilities  

All methods are defined as `public static` in the `SkillBuddy.Core.Helper` class.

---

## ⚙️ Value Helpers

### 🔹 GetValueWithDefaults
```csharp
public static object GetValueWithDefaults(object? value, object defaultValue)
```
- Returns the provided `value` or the `defaultValue` if `value` is null.

---

### 🔹 ToCsvString
```csharp
public static string ToCsvString<T>(this List<T> list, Type? toType = null)
```
- Converts a `List<T>` into a comma-separated string.  
- Uses `GetQuotedDbString` internally.

---

### 🔹 GetQuotedDbString
```csharp
public static string GetQuotedDbString(this object? obj, Type? toType = null, string? format = null)
```
- Converts an object to a quoted string suitable for database queries.  
- Handles numeric, boolean, string, `Guid`, `DateTime`, and enums.  
- Supports custom formatting.

---

### 🔹 IsList / IsDictionary
```csharp
public static bool IsList(this object? o)
public static bool IsDictionary(this object? o)
```
- Returns `true` if the object is a generic list or dictionary.

---

## ⏱ Time Helpers

### 🔹 TimeStampToLocalDate
```csharp
public static DateTime? TimeStampToLocalDate(string? unixDateString)
public static DateTime? TimeStampToLocalDate(long? unixDate)
```
- Converts a Unix timestamp (seconds since 1970-01-01) to a local `DateTime`.

---

## 📊 Excel Helpers

### 🔹 ExcelToDataSet
```csharp
public static DataSet? ExcelToDataSet(string filePath, bool hasHeaders = true)
```
- Loads an Excel file into a `DataSet` using **ClosedXML**.  
- Supports multiple worksheets.  
- Each worksheet becomes a `DataTable`.  
- If `hasHeaders` is true, the first row defines column names.

---

## 📑 CSV Helpers

### 🔹 CsvToDataTableSimple
```csharp
public static DataTable CsvToDataTableSimple(string filePath, char separator = ',', bool hasHeaders = true, char? qualifier = '"')
```
- Reads a CSV file into a `DataTable` using simple splitting logic.  
- Handles quoted fields with a qualifier.

---

### 🔹 CsvParser
```csharp
public static string[] CsvParser(string csvText, char separator = ',', char qualifier = '"')
```
- Splits a CSV line into tokens, respecting quoted values.

---

### 🔹 CsvToDataTableRegex
```csharp
public static DataTable CsvToDataTableRegex(string filePath, char delimiter = ',')
```
- Reads a CSV file using a **regular expression parser** for more complex quoted cases.

---

### 🔹 CsvToDataTableCsvHelper
```csharp
public static DataTable CsvToDataTableCsvHelper(string filePath, string delimiter, char qualifier = '"')
```
- Reads a CSV file into a `DataTable` using the **CsvHelper** library.  
- Recommended for robust CSV parsing.

---

### 🔹 DataTableToCsv
```csharp
public static string DataTableToCsv(DataTable dtDataTable, char separator = ',', bool includeHeaders = true)
```
- Converts a `DataTable` to a CSV string.  
- Properly quotes values containing separators.

---

## 📂 IO Helpers

### 🔹 DeleteTempFile
```csharp
public static bool DeleteTempFile(string fileName)
```
- Deletes a file.  
- Returns `true` if successful, `false` otherwise.

---

### 🔹 ReadStreamInChunks
```csharp
public static string ReadStreamInChunks(Stream stream)
```
- Reads a stream into a string using a 4 KB buffer.  
- Resets the stream position before reading.

---

## 🔐 Best Practices
1. Use `ExcelToDataSet` for cross-platform Excel support (ClosedXML).  
2. Prefer `CsvToDataTableCsvHelper` for reliable CSV parsing.  
3. Use `DataTableToCsv` when exporting tabular data.  
4. Handle exceptions in Excel/CSV methods at the caller level for logging.  
5. For large files, prefer `ReadStreamInChunks` over `ReadToEnd()` for efficiency.  
