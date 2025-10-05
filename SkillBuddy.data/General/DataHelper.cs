using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SkillBuddy.Entity;
using SkillBuddy.Core;

namespace SkillBuddy.Data.General
{
    public static class DataHelper
    {
        public static string ToCsvString<T>(this List<T> list, Type? toType = null)
        {
            if (list == null || list.Count == 0)
                return String.Empty;

            var typeCode = Type.GetTypeCode(typeof(T));

            if (typeCode is TypeCode.Byte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64)
            {
                return String.Join(",", list);
            }
            else if (typeCode is TypeCode.Char or TypeCode.String)
            {
                return String.Join(",", list.Select(x => $"'{x}'"));
            }
            else if (typeof(T) == typeof(Guid))
            {
                return String.Join(",", list.Select(x => $"'{x}'"));
            }
            else if (typeof(T).IsEnum)
            {
                #region [ enum ]
                if (toType == null || toType == typeof(char))
                {
                    return list.Select(x => (char)(Convert.ChangeType(x, typeof(char)) ?? default(char)))
                               .ToList()
                               .ToCsvString();
                }
                if (toType == typeof(byte)
                    || toType == typeof(short)
                    || toType == typeof(int)
                    || toType == typeof(long))
                {
                    return list.Select(x => Convert.ToInt64(x))
                               .ToList()
                               .ToCsvString();
                }
                else if (toType == typeof(string))
                {
                    return list.Select(x => x?.ToString() ?? String.Empty)
                               .ToList()
                               .ToCsvString();
                }
                #endregion
            }
            return String.Empty;
        }

        public static void QueryBuilderToDataTable(QueryBuilder queryBuilder, ref DataTable? dataTable, int? level = null)
        {
            if (dataTable != null && dataTable.Columns.Count == 0)
            {
                dataTable.Columns.Add("Group", typeof(int));
                dataTable.Columns.Add("ParentGroup", typeof(int));
                dataTable.Columns.Add("Type", typeof(string));
                dataTable.Columns.Add("Field", typeof(string));
                dataTable.Columns.Add("Condition", typeof(string));
                dataTable.Columns.Add("Value1", typeof(string));
                dataTable.Columns.Add("Value2", typeof(string));
            }

            try
            {
                int currentLevel = (level ?? 0) + 1;

                DataRow dataRow;

                if (queryBuilder.Condition.HasValue)
                {
                    // dataRow = dataTable.NewRow();
                    if (dataTable == null)
                        throw new ArgumentNullException(nameof(dataTable));
                    dataRow = dataTable.NewRow();
                    dataRow["Group"] = currentLevel;
                    dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                    dataRow["Type"] = "CONDITION";
                    dataRow["Field"] = queryBuilder.Condition.Value.ToString();
                    dataTable.Rows.Add(dataRow);
                }

                if (queryBuilder.Not.HasValue && queryBuilder.Not.Value)
                {
                    if (dataTable == null)
                        throw new ArgumentNullException(nameof(dataTable));
                    dataRow = dataTable.NewRow();
                    dataRow["Group"] = currentLevel;
                    dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                    dataRow["Type"] = "NOT";
                    dataRow["Field"] = "NOT";
                    dataTable.Rows.Add(dataRow);
                }

                if (level == null)
                {
                    if (queryBuilder.Count.HasValue)
                    {
                        if (dataTable == null)
                            throw new ArgumentNullException(nameof(dataTable));
                        dataRow = dataTable.NewRow();
                        dataRow["Group"] = currentLevel;
                        dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                        dataRow["Type"] = "COUNT";
                        dataRow["Field"] = queryBuilder.Count.Value.ToString();
                        dataTable.Rows.Add(dataRow);
                    }

                    if (queryBuilder.OrderBy != null)
                    {
                        for (int i = 0; i < queryBuilder.OrderBy.Count; i++)
                        {
                            var order = queryBuilder.OrderBy[i];
                            if (order == null) continue;

                            if (dataTable == null)
                                throw new ArgumentNullException(nameof(dataTable));
                            dataRow = dataTable.NewRow();
                            dataRow["Group"] = currentLevel;
                            dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                            dataRow["Type"] = "ORDER";
                            dataRow["Field"] = order.Field ?? String.Empty;
                            dataRow["Condition"] = order.Type.ToString();
                            dataRow["Value2"] = $"{i:0000000000}";
                            dataTable.Rows.Add(dataRow);
                        }
                    }

                    if (queryBuilder.Offset.HasValue)
                    {
                        if (dataTable == null)
                            throw new ArgumentNullException(nameof(dataTable));
                        dataRow = dataTable.NewRow();
                        dataRow["Group"] = currentLevel;
                        dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                        dataRow["Type"] = "OFFSET";
                        dataRow["Field"] = queryBuilder.Offset.Value.ToString();
                        dataTable.Rows.Add(dataRow);
                    }

                    if (queryBuilder.PageSize.HasValue)
                    {
                        if (dataTable == null)
                            throw new ArgumentNullException(nameof(dataTable));
                        dataRow = dataTable.NewRow();
                        dataRow["Group"] = currentLevel;
                        dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                        dataRow["Type"] = "ROWCOUNT";
                        dataRow["Field"] = queryBuilder.PageSize.Value.ToString();
                        dataTable.Rows.Add(dataRow);

                        if (dataTable == null)
                            throw new ArgumentNullException(nameof(dataTable));
                        dataRow = dataTable.NewRow();
                        dataRow["Group"] = currentLevel;
                        dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                        dataRow["Type"] = "TOTALCOUNT";
                        dataRow["Field"] = queryBuilder.PageSize.Value.ToString();
                        dataTable.Rows.Add(dataRow);
                    }
                }

                int rulesCount = queryBuilder.Rules?.Count ?? 0;

                if (rulesCount == 0)
                    return;

                for (int i = 0; i < rulesCount; i++)
                {
                    var rule = queryBuilder.Rules?[i];
                    if (rule == null) continue;

                    if (!rule.Condition.HasValue)
                    {
                        string op = rule.Operator switch
                        {
                            QueryBuilderOperator.Equals or QueryBuilderOperator.Is => "=",
                            QueryBuilderOperator.NotEquals or QueryBuilderOperator.IsNot => "<>",
                            QueryBuilderOperator.GreaterThan => ">",
                            QueryBuilderOperator.GreaterThanOrEqual => ">=",
                            QueryBuilderOperator.LessThan => "<",
                            QueryBuilderOperator.LessThanOrEquals => "<=",
                            QueryBuilderOperator.In => "IN",
                            QueryBuilderOperator.NotIn => "NOT IN",
                            QueryBuilderOperator.Between => "BETWEEN",
                            QueryBuilderOperator.NotBetween => "NOT BETWEEN",
                            QueryBuilderOperator.IsNull => "IS NULL",
                            QueryBuilderOperator.IsNotNull => "IS NOT NULL",
                            _ => String.Empty
                        };

                        if (rule.Operator is QueryBuilderOperator.IsNull or QueryBuilderOperator.IsNotNull)
                        {
                            rule.Value1 = null;
                            rule.Value2 = null;
                        }

                        if (dataTable == null)
                            throw new ArgumentNullException(nameof(dataTable));
                        dataRow = dataTable.NewRow();
                        dataRow["Group"] = currentLevel;
                        dataRow["ParentGroup"] = Helper.GetValueWithDefaults(level, DBNull.Value);
                        dataRow["Type"] = "RULE";
                        dataRow["Field"] = rule.Field ?? String.Empty;
                        dataRow["Condition"] = op;
                        dataRow["Value1"] = Helper.GetValueWithDefaults(rule.Value1 ?? DBNull.Value, DBNull.Value);
                        dataRow["Value2"] = Helper.GetValueWithDefaults(rule.Value2 ?? DBNull.Value, DBNull.Value);
                        dataTable.Rows.Add(dataRow);
                    }
                    else
                    {
                        var nested = new QueryBuilder
                        {
                            Condition = rule.Condition,
                            Rules = rule.Rules ?? new List<QueryRule>()
                        };
                        QueryBuilderToDataTable(nested, ref dataTable, currentLevel);
                    }
                }
            }
            catch
            {
                throw; // preserves stack trace
            }
        }
    }
}
