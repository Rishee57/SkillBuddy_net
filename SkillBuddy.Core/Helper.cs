using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace SkillBuddy.Core
{
  /// <summary>
  /// General helper utilities for SkillBuddy project:
  /// - Value handling
  /// - String/CSV conversions
  /// - Excel/CSV file import/export
  /// - Stream reading
  /// </summary>
  public static class Helper
  {
    #region Value Helpers

    public static object GetValueWithDefaults(object? value, object defaultValue)
        => value ?? defaultValue;

    public static string ToCsvString<T>(this List<T> list, Type? toType = null)
    {
      if (list == null || list.Count == 0)
        return String.Empty;

      return String.Join(",", list.Select(x => x.GetQuotedDbString()));
    }

    public static string GetQuotedDbString(this object? obj, Type? toType = null, string? format = null)
    {
      if (obj == null) return String.Empty;

      var type = obj.GetType();

      return type switch
      {
        Type t when t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long)
            => $"{obj}",
        Type t when t == typeof(bool)
            => (bool)obj ? "1" : "0",
        Type t when t == typeof(char) || t == typeof(string) || t == typeof(Guid)
            => $"'{obj}'",
        Type t when t == typeof(DateTime)
            => !String.IsNullOrEmpty(format)
                ? $"'{((DateTime)obj).ToString(format)}'"
                : $"'{((DateTime)obj):dd-MMM-yyyy HH:mm:ss.fff}'",
        _ when type.IsEnum => toType switch
        {
          Type tt when tt == typeof(char)
              => ((char)Convert.ChangeType(obj, typeof(char))).GetQuotedDbString(),
          Type tt when tt == typeof(byte) || tt == typeof(short) || tt == typeof(int) || tt == typeof(long)
              => ((long)Convert.ChangeType(obj, typeof(long))).GetQuotedDbString(),
          _ => (obj.ToString() ?? String.Empty).GetQuotedDbString()
        },
        _ => String.Empty
      };
    }

    public static bool IsList(this object? o) =>
        o is IList && o.GetType().IsGenericType && o.GetType().GetGenericTypeDefinition() == typeof(List<>);

    public static bool IsDictionary(this object? o) =>
        o is IDictionary &&
        o.GetType().IsGenericType &&
        o.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>);

    #endregion

    #region Time Helpers

    public static DateTime? TimeStampToLocalDate(string? unixDateString)
        => long.TryParse(unixDateString, out long unixDate) ? TimeStampToLocalDate(unixDate) : null;

    public static DateTime? TimeStampToLocalDate(long? unixDate)
    {
      if (!unixDate.HasValue) return null;
      return DateTime.UnixEpoch.AddSeconds(unixDate.Value).ToLocalTime();
    }

    #endregion

    #region Excel Helpers

    /// <summary>
    /// Converts an Excel file to a DataSet (cross-platform using ClosedXML).
    /// </summary>
    public static DataSet? ExcelToDataSet(string filePath, bool hasHeaders = true)
    {
      try
      {
        using var workbook = new XLWorkbook(filePath);
        DataSet ds = new DataSet();

        foreach (var ws in workbook.Worksheets)
        {
          DataTable dt = new DataTable(ws.Name);
          bool firstRow = true;

          foreach (var row in ws.RowsUsed())
          {
            if (firstRow)
            {
              foreach (var cell in row.Cells())
              {
                string colName = hasHeaders ? cell.GetString() : $"Column{cell.Address.ColumnNumber}";
                dt.Columns.Add(colName);
              }
              if (hasHeaders) { firstRow = false; continue; }
              firstRow = false;
            }

            var dr = dt.NewRow();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
              dr[i] = row.Cell(i + 1).Value.ToString() ?? String.Empty;
            }
            dt.Rows.Add(dr);
          }
          ds.Tables.Add(dt);
        }

        return ds;
      }
      catch (Exception)
      {
        return null; // could log here
      }
    }

    #endregion

    #region CSV Helpers

    public static DataTable CsvToDataTableSimple(string filePath, char separator = ',', bool hasHeaders = true, char? qualifier = '"')
    {
      DataTable csvTable = new DataTable();
      using StreamReader reader = new StreamReader(filePath, Encoding.Default);

      string? headerLine = reader.ReadLine();
      if (headerLine == null) return csvTable;

      string[] columnNames = headerLine.Split(separator);
      for (int index = 0; index < columnNames.Length; index++)
      {
        csvTable.Columns.Add(hasHeaders ? columnNames[index] : $"Column{index}");
      }

      if (!hasHeaders)
      {
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        reader.DiscardBufferedData();
      }

      while (!reader.EndOfStream)
      {
        string? row = reader.ReadLine();
        if (String.IsNullOrWhiteSpace(row)) continue;

        string[] rowContent = qualifier != null
            ? CsvParser(row, separator, qualifier.Value)
            : row.Split(separator);

        DataRow dRow = csvTable.NewRow();
        for (int colCount = 0; colCount < csvTable.Columns.Count && colCount < rowContent.Length; colCount++)
        {
          dRow[colCount] = rowContent[colCount];
        }
        csvTable.Rows.Add(dRow);
      }

      return csvTable;
    }

    public static string[] CsvParser(string csvText, char separator = ',', char qualifier = '"')
    {
      List<string> tokens = new List<string>();
      int last = -1;
      bool inText = false;
      string ql = Convert.ToString(qualifier);

      for (int current = 0; current < csvText.Length; current++)
      {
        if (csvText[current] == qualifier)
        {
          inText = !inText;
        }
        else if (csvText[current] == separator && !inText)
        {
          tokens.Add(csvText.Substring(last + 1, current - last - 1).Replace(ql, "").Trim());
          last = current;
        }
      }

      if (last != csvText.Length - 1)
      {
        tokens.Add(csvText.Substring(last + 1).Replace(ql, "").Trim());
      }

      return tokens.ToArray();
    }

    public static DataTable CsvToDataTableRegex(string filePath, char delimiter = ',')
    {
      DataTable dtTable = new DataTable();
      Regex csvParser = new Regex(delimiter + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

      using StreamReader sr = new StreamReader(filePath);
      string? firstLine = sr.ReadLine();
      if (firstLine == null) return dtTable;

      string[] headers = firstLine.Split(delimiter);
      foreach (string header in headers) dtTable.Columns.Add(header);

      while (!sr.EndOfStream)
      {
        string? line = sr.ReadLine();
        if (line == null) continue;

        string[] rows = csvParser.Split(line);
        DataRow dr = dtTable.NewRow();
        for (int i = 0; i < headers.Length && i < rows.Length; i++)
        {
          dr[i] = rows[i].Replace("\"", String.Empty);
        }
        dtTable.Rows.Add(dr);
      }

      return dtTable;
    }

    public static DataTable CsvToDataTableCsvHelper(string filePath, string delimiter, char qualifier = '"')
    {
      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        HasHeaderRecord = true,
        Delimiter = delimiter,
        Quote = qualifier,
      };

      DataTable dt = new DataTable();
      using var reader = File.OpenText(filePath);
      using var csv = new CsvReader(reader, config);
      using var dr = new CsvDataReader(csv);
      dt.Load(dr);

      return dt;
    }

    public static string DataTableToCsv(DataTable dtDataTable, char separator = ',', bool includeHeaders = true)
    {
      var sb = new StringBuilder();
      if (includeHeaders)
      {
        sb.AppendLine(String.Join(separator, dtDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
      }

      foreach (DataRow dr in dtDataTable.Rows)
      {
        var values = dr.ItemArray.Select(v =>
        {
          string value = v?.ToString() ?? String.Empty;
          return value.Contains(separator) ? $"\"{value}\"" : value;
        });
        sb.AppendLine(String.Join(separator, values));
      }

      return sb.ToString();
    }

    #endregion

    #region IO Helpers

    public static bool DeleteTempFile(string fileName)
    {
      try
      {
        File.Delete(fileName);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public static string ReadStreamInChunks(Stream stream)
    {
      const int bufferLength = 4096;
      stream.Seek(0, SeekOrigin.Begin);

      using var textWriter = new StringWriter();
      using var reader = new StreamReader(stream, true);

      var buffer = new char[bufferLength];
      int readLength;
      while ((readLength = reader.ReadBlock(buffer, 0, bufferLength)) > 0)
      {
        textWriter.Write(buffer, 0, readLength);
      }

      return textWriter.ToString();
    }

    #endregion
  }
}
