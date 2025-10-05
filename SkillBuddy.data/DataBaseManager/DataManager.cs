using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace SkillBuddy.data.DataBaseManager
{
    #region Enum: Parameter Directions
    public enum QueryParameterDirection : int
    {
        // The parameter is an input parameter.
        Input = 1,
        // The parameter is capable of both input and output.
        Output = 2,
        // The parameter represents a return value from an 
        // operation such as a stored procedure, built-in
        // function, or user-defined function.
        Return = 3,
        InputOutput = 4
    }
    #endregion
    // Class to execute any kind of database query.
    public class DataManager : IDisposable
    {
        #region FIELDS
        private string strCommandText = string.Empty;
        private bool blnSP = true;
        private readonly List<NpgsqlParameter> oParameters = new List<NpgsqlParameter>();
        private bool blnLocalConn = true;
        #endregion

        #region Constructors
        public DataManager(string StoredProcName)
            : this(StoredProcName, false) { }

        public DataManager(string SqlString, bool IsTextQuery)
        {
            blnSP = !IsTextQuery;
            strCommandText = SqlString;
        }
        #endregion

        #region Run Query - DataTable / DataSet / NonQuery / Scalar / Reader
        public DataTable? GetTable()
        {
            DataTable? dt = null;
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            var da = new NpgsqlDataAdapter(oCmd);
            var ds = new DataSet();

            try
            {
                da.Fill(ds);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception)
            {
                throw; // preserve stack
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                {
                    this.oConn.Close();
                }
            }

            return dt;
        }

        public async Task<DataTable?> GetTableAsync()
        {
            DataTable? dt = null;
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            var da = new NpgsqlDataAdapter(oCmd);
            var ds = new DataSet();

            try
            {
                await Task.Run(() => da.Fill(ds));
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                {
                    this.oConn.Close();
                }
            }

            return dt;
        }

        public DataSet GetDataSet()
        {
            var ds = new DataSet();
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            var da = new NpgsqlDataAdapter(oCmd);

            try
            {
                da.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                {
                    this.oConn.Close();
                }
            }

            return ds;
        }

        public async Task<DataSet> GetDataSetAsync()
        {
            var ds = new DataSet();
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            var da = new NpgsqlDataAdapter(oCmd);

            try
            {
                await Task.Run(() => da.Fill(ds));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                {
                    this.oConn.Close();
                }
            }

            return ds;
        }

        public NpgsqlCommand GetCommand()
        {
            var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);
            return oCmd;
        }

        public int RunActionQuery()
        {
            int intRowsAffected = -1;
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            try
            {
                intRowsAffected = oCmd.ExecuteNonQuery();
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                    this.oConn.Close();
            }

            return intRowsAffected;
        }

        public async Task<int> RunActionQueryAsync()
        {
            int intRowsAffected = -1;
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            try
            {
                intRowsAffected = await oCmd.ExecuteNonQueryAsync();
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                    this.oConn.Close();
            }

            return intRowsAffected;
        }

        public object? GetScalar()
        {
            object? oRetVal = null;
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            try
            {
                oRetVal = oCmd.ExecuteScalar();
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                    this.oConn.Close();
            }

            return oRetVal;
        }

        public async Task<object?> GetScalarAsync()
        {
            object? oRetVal = null;
            using var oCmd = new NpgsqlCommand();
            this.InitQuery(oCmd);

            try
            {
                oRetVal = await oCmd.ExecuteScalarAsync();
            }
            finally
            {
                if (this.blnLocalConn && this.oConn != null)
                    this.oConn.Close();
            }

            return oRetVal;
        }

        public NpgsqlDataReader? ExecuteReader()
        {
            try
            {
                var oCmd = new NpgsqlCommand();
                this.InitQuery(oCmd);
                // Do NOT close the connection here because reader depends on it.
                return oCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                return null;
            }
        }

        public async Task<NpgsqlDataReader?> ExecuteReaderAsync()
        {
            try
            {
                var oCmd = new NpgsqlCommand();
                this.InitQuery(oCmd);
                return await oCmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region InitQuery
        private void InitQuery(NpgsqlCommand oCmd)
        {
            // determine if we should use a local connection
            blnLocalConn = (this.oConn == null);
            if (blnLocalConn)
            {
                this.oConn = new DataConnection();
                this.oConn.Open();
            }

            // assign the underlying NpgsqlConnection from DataConnection
            if (this.oConn == null || this.oConn.oConn == null)
                throw new InvalidOperationException("No open connection available.");

            oCmd.Connection = this.oConn.oConn;

            oCmd.CommandTimeout = 0;
            oCmd.CommandText = this.strCommandText;
            oCmd.CommandType = (this.blnSP ? CommandType.StoredProcedure : CommandType.Text);

            // add parameters
            foreach (var p in this.oParameters)
            {
                oCmd.Parameters.Add(p);
            }
        }
        #endregion

        #region Parameter handling
        // ... existing parameter helpers (Integer, Varchar, Bool, etc.) unchanged ...

        // ** Array parameters
        // Integer
        // public void AddIntegerPara(string Name, int Value) => AddIntegerPara(Name, Value, QueryParameterDirection.Input);
        public void AddSmallIntPara(string name, short? value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Smallint)
            {
                Direction = GetParaType(direction),
                Value = value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }



        public void AddIntegerPara(string Name, int? Value) => AddIntegerPara(Name, Value, QueryParameterDirection.Input);

        // public void AddIntegerPara(string Name, int Value, QueryParameterDirection Direction)
        // {
        //     var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Integer)
        //     {
        //         Direction = GetParaType(Direction),
        //         Value = Value
        //     };
        //     this.oParameters.Add(oPara);
        // }

        public void AddIntegerPara(string Name, int? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Integer)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Bigint
        // public void AddIntegerBigPara(string Name, long Value) => AddIntegerBigPara(Name, Value, QueryParameterDirection.Input);
        public void AddIntegerBigPara(string Name, long? Value) => AddIntegerBigPara(Name, Value, QueryParameterDirection.Input);

        // public void AddIntegerBigPara(string Name, long Value, QueryParameterDirection Direction)
        // {
        //     var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Bigint)
        //     {
        //         Direction = GetParaType(Direction),
        //         Value = Value
        //     };
        //     this.oParameters.Add(oPara);
        // }

        public void AddIntegerBigPara(string Name, long? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Bigint)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Char
        public void AddCharPara(string Name, int Size, char Value) => AddCharPara(Name, Size, Value, QueryParameterDirection.Input);
        public void AddCharPara(string Name, int Size, char Value, QueryParameterDirection Direction)
        {
            object oValue = Value; // char is value type - cannot be null
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Char)
            {
                Size = Size,
                Direction = GetParaType(Direction),
                Value = oValue
            };
            this.oParameters.Add(oPara);
        }

        // Varchar / Text
        public void AddVarcharPara(string Name, int Size, string Value) => AddVarcharPara(Name, Size, Value, QueryParameterDirection.Input);
        public void AddVarcharPara(string Name, int Size, string? Value, QueryParameterDirection Direction)
        {
            object oValue = string.IsNullOrEmpty(Value) ? (object)DBNull.Value : Value!;
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Varchar)
            {
                Size = Size,
                Direction = GetParaType(Direction),
                Value = oValue
            };
            this.oParameters.Add(oPara);
        }

        // Boolean
        public void AddBoolPara(string Name, bool? Value) => AddBoolPara(Name, Value, QueryParameterDirection.Input);
        public void AddBoolPara(string Name, bool? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Boolean)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Image / bytea
        public void AddImagePara(string Name, string? Value) => AddImagePara(Name, Value, QueryParameterDirection.Input);
        public void AddImagePara(string Name, string? Value, QueryParameterDirection Direction)
        {
            object oValue = string.IsNullOrEmpty(Value) ? (object)DBNull.Value : Value!;
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Bytea)
            {
                Direction = GetParaType(Direction),
                Value = oValue
            };
            this.oParameters.Add(oPara);
        }

        // GUID / UUID
        public void AddGuidPara(string Name, Guid? Value) => AddGuidPara(Name, Value, QueryParameterDirection.Input);
        public void AddGuidPara(string Name, Guid? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Uuid)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // DateTime / Timestamp
        public void AddDatePara(string name, DateTime? value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Date)
            {
                Direction = GetParaType(direction),
                Value = value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }


        public void AddDateTimePara(string Name, DateTime Value) => AddDateTimePara(Name, Value, QueryParameterDirection.Input);
        public void AddDateTimePara(string Name, DateTime? Value) => AddDateTimePara(Name, Value, QueryParameterDirection.Input);

        public void AddDateTimePara(string Name, DateTime Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Timestamp)
            {
                Direction = GetParaType(Direction),
                Value = Value
            };
            this.oParameters.Add(oPara);
        }

        public void AddDateTimePara(string Name, DateTime? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Timestamp)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Time
        public void AddTimePara(string Name, TimeSpan Value) => AddTimePara(Name, Value, QueryParameterDirection.Input);
        public void AddTimePara(string Name, TimeSpan? Value) => AddTimePara(Name, Value, QueryParameterDirection.Input);

        public void AddTimePara(string Name, TimeSpan Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Time)
            {
                Direction = GetParaType(Direction),
                Value = Value
            };
            this.oParameters.Add(oPara);
        }

        public void AddTimePara(string Name, TimeSpan? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Time)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Decimal / Numeric
        public void AddDecimalPara(string Name, byte Scale, byte Precision, decimal Value) => AddDecimalPara(Name, Scale, Precision, Value, QueryParameterDirection.Input);
        public void AddDecimalPara(string Name, byte Scale, byte Precision, decimal? Value) => AddDecimalPara(Name, Scale, Precision, Value, QueryParameterDirection.Input);

        public void AddDecimalPara(string Name, byte Scale, byte Precision, decimal Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Numeric)
            {
                Direction = GetParaType(Direction),
                Scale = Scale,
                Precision = Precision,
                Value = Value
            };
            this.oParameters.Add(oPara);
        }

        public void AddDecimalPara(string Name, byte Scale, byte Precision, decimal? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Numeric)
            {
                Direction = GetParaType(Direction),
                Scale = Scale,
                Precision = Precision,
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Float / Double
        public void AddFloatPara(string Name, double Value) => AddFloatPara(Name, Value, QueryParameterDirection.Input);
        public void AddFloatPara(string Name, double? Value) => AddFloatPara(Name, Value, QueryParameterDirection.Input);

        public void AddFloatPara(string Name, double Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Double)
            {
                Direction = GetParaType(Direction),
                Value = Value
            };
            this.oParameters.Add(oPara);
        }

        public void AddFloatPara(string Name, double? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Double)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Timestamp / rowversion-like
        public void AddTimeStampPara(string Name, byte[]? Value) => AddTimeStampPara(Name, Value, QueryParameterDirection.Input);
        public void AddTimeStampPara(string Name, byte[]? Value, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Bytea)
            {
                Direction = GetParaType(Direction),
                Value = Value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // VarBinary
        public void AddVarBinaryPara(string Name, byte[]? oValue) => AddVarBinaryPara(Name, oValue, QueryParameterDirection.Input);
        public void AddVarBinaryPara(string Name, byte[]? oValue, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Bytea)
            {
                Direction = GetParaType(Direction),
                Value = oValue ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // Structured - no direct equivalent in Postgres; accept as JSON array or JSONB
        public void AddStructuredPara(string Name, DataTable oValue) => AddStructuredPara(Name, oValue, QueryParameterDirection.Input);
        public void AddStructuredPara(string Name, DataTable oValue, QueryParameterDirection Direction)
        {
            // Serialize DataTable to JSON and send as jsonb
            var json = JsonConvert.SerializeObject(oValue);
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Jsonb)
            {
                Direction = GetParaType(Direction),
                Value = json
            };
            this.oParameters.Add(oPara);
        }

        // JSON / JSONB
        public void AddJsonPara(string Name, object oValue) => AddJsonPara(Name, oValue, QueryParameterDirection.Input);
        public void AddJsonPara(string Name, object oValue, QueryParameterDirection Direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(Name, NpgsqlDbType.Jsonb)
            {
                Direction = GetParaType(Direction),
                Value = JsonConvert.SerializeObject(oValue)
            };
            this.oParameters.Add(oPara);
        }

        public void AddJsonbPara(string name, object value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
            {
                Direction = GetParaType(direction),
                Value = JsonConvert.SerializeObject(value)
            };
            this.oParameters.Add(oPara);
        }
        // NULL parameter helpers
        public void AddNullValuePara(string Name)
        {
            var oPara = new NpgsqlParameter(Name, DBNull.Value)
            {
                Direction = ParameterDirection.Input
            };
            this.oParameters.Add(oPara);
        }

        public void AddNullValuePara(string Name, QueryParameterDirection Direction)
        {
            var oPara = new NpgsqlParameter(Name, DBNull.Value)
            {
                Direction = GetParaType(Direction)
            };
            this.oParameters.Add(oPara);
        }

        public void AddReturnPara() => AddIntegerPara("ReturnIntPara", 0, QueryParameterDirection.Return);

        public object? GetParaValue(string ParaName)
        {
            if (string.IsNullOrWhiteSpace(ParaName))
                return null;

            var name = ParaName.Trim().ToLower();
            foreach (var p in this.oParameters)
            {
                if (p.ParameterName != null && p.ParameterName.Trim().ToLower() == name)
                    return p.Value;
            }
            return null;
        }

        public object? GetReturnParaValue() => GetParaValue("ReturnIntPara");

        public void ClearParameters() => this.oParameters.Clear();
        #endregion

        #region Parameters for PostgreSQL-specific types
        // ** Array types
        // Note: Npgsql requires the array to be a one-dimensional array of a supported type
        // Supported types include int[],  Guid[], etc.

        public void AddIntArrayPara(string name, int[]? values, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Direction = GetParaType(direction),
                Value = values ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // string[],
        public void AddTextArrayPara(string name, string[]? values, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Text)
            {
                Direction = GetParaType(direction),
                Value = values ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }
        // Guid[]
        public void AddUuidArrayPara(string name, Guid[]? values, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Array | NpgsqlDbType.Uuid)
            {
                Direction = GetParaType(direction),
                Value = values ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }



        // ** Large Objects (Bytea)
        public void AddByteaPara(string name, byte[]? value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Bytea)
            {
                Direction = GetParaType(direction),
                Value = value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // ** Range types
        public void AddIntRangePara(string name, NpgsqlRange<int>? range, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Range | NpgsqlDbType.Integer)
            {
                Direction = GetParaType(direction),
                Value = range ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // DateTime range
        public void AddTimestampRangePara(string name, NpgsqlRange<DateTime>? range, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Range | NpgsqlDbType.Timestamp)
            {
                Direction = GetParaType(direction),
                Value = range ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        // ** Composite types (records)
        //82 - 564 =
        public void AddCompositePara<T>(string name, T? value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            // Composite must be registered in Npgsql with a mapping to a CLR type.
            var oPara = new NpgsqlParameter(name, value ?? (object)DBNull.Value)
            {
                Direction = GetParaType(direction)
            };
            this.oParameters.Add(oPara);
        }

        // ** Table-Valued Parameter Simulation (send list of objects as JSON array)
        public void AddTableValuedPara<T>(string name, IEnumerable<T>? values, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var json = values == null ? (object)DBNull.Value : JsonConvert.SerializeObject(values);
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Jsonb)
            {
                Direction = GetParaType(direction),
                Value = json
            };
            this.oParameters.Add(oPara);
        }
        // AddTextPara method to add a text parameter   
        public void AddTextPara(string name, string? value, QueryParameterDirection direction)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Text)
            {
                Direction = GetParaType(direction),
                Value = string.IsNullOrEmpty(value) ? DBNull.Value : value
            };
            this.oParameters.Add(oPara);
        }
        // Interval
        public void AddIntervalPara(string name, TimeSpan? value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Interval)
            {
                Direction = GetParaType(direction),
                Value = value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }
        // Real (float4)
        public void AddRealPara(string name, float? value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Real)
            {
                Direction = GetParaType(direction),
                Value = value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }
        // Money
        public void AddMoneyPara(string name, decimal? value, QueryParameterDirection direction = QueryParameterDirection.Input)
        {
            var oPara = new NpgsqlParameter(name, NpgsqlDbType.Money)
            {
                Direction = GetParaType(direction),
                Value = value ?? (object)DBNull.Value
            };
            this.oParameters.Add(oPara);
        }

        #endregion

        #region Dispose & Connection Property
        public void Dispose()
        {
            try
            {
                if (this.oConn != null)
                {
                    this.oConn.Close();
                }
            }
            catch { }

            this.oParameters.Clear();
            GC.SuppressFinalize(this);
        }

        // Allow setting an external connection 
        private DataConnection? oConn = null;
        public DataConnection Connection
        {
            set
            {
                oConn = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
        #endregion
        // Helper to map custom enum to ParameterDirection  
        private ParameterDirection GetParaType(QueryParameterDirection Direction)
        {
            return Direction switch
            {
                QueryParameterDirection.Output => ParameterDirection.InputOutput,
                QueryParameterDirection.Return => ParameterDirection.ReturnValue,
                QueryParameterDirection.InputOutput => ParameterDirection.InputOutput,
                _ => ParameterDirection.Input,
            };
        }




    }
}
