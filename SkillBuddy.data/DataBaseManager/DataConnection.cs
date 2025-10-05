using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using SkillBuddy.data.General;

namespace SkillBuddy.data.DataBaseManager
{
    public class DataConnection : DataAccessBase, IDisposable
    {
        #region FIELDS
        private string _connectionString = String.Empty;
        internal NpgsqlConnection? oConn = null;
        internal NpgsqlTransaction? oTran = null;

        private bool blnIsOpen = false;
        private bool blnTranActive = false;
        #endregion

        #region Constructor
        public DataConnection() { }

        public DataConnection(string connectionString)
        {
            _connectionString = connectionString;
            Console.WriteLine("Connection String: " + connectionString);
        }
        #endregion

        #region Properties
        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        public bool IsOpen => blnIsOpen;

        public bool IsTransactionActive => blnTranActive;
        #endregion

        #region Sync Methods
        public bool Open()
        {
            return Open(_connectionString);
        }

        public bool Open(string connectionString)
        {
            if (oConn != null && oConn.State == ConnectionState.Open)
                return true;

            oConn = new NpgsqlConnection(connectionString);
            oConn.Open();
            blnIsOpen = true;
            return true;
        }

        public void Close()
        {
            oTran?.Dispose();
            oTran = null;

            if (oConn != null)
            {
                if (oConn.State == ConnectionState.Open)
                {
                    oConn.Close();
                }
                oConn.Dispose();
                oConn = null;
            }

            blnIsOpen = false;
            blnTranActive = false;
        }

        public void BeginTran()
        {
            if (oConn?.State == ConnectionState.Open)
            {
                oTran = oConn.BeginTransaction();
                blnTranActive = true;
            }
        }

        public void CommitTran()
        {
            if (oTran != null)
            {
                oTran.Commit();
                oTran.Dispose();
                oTran = null;
                blnTranActive = false;
            }
        }

        public void RollbackTran()
        {
            if (oTran != null)
            {
                oTran.Rollback();
                oTran.Dispose();
                oTran = null;
                blnTranActive = false;
            }
        }
        #endregion

        #region Async Methods
        public async Task<bool> OpenAsync()
        {
            return await OpenAsync(_connectionString);
        }

        public async Task<bool> OpenAsync(string connectionString)
        {
            if (oConn != null && oConn.State == ConnectionState.Open)
                return true;

            oConn = new NpgsqlConnection(connectionString);
            await oConn.OpenAsync();
            blnIsOpen = true;
            return true;
        }

        public async Task BeginTranAsync()
        {
            if (oConn?.State == ConnectionState.Open)
            {
                oTran = await oConn.BeginTransactionAsync();
                blnTranActive = true;
            }
        }

        public async Task CommitTranAsync()
        {
            if (oTran != null)
            {
                await oTran.CommitAsync();
                await oTran.DisposeAsync();
                oTran = null;
                blnTranActive = false;
            }
        }

        public async Task RollbackTranAsync()
        {
            if (oTran != null)
            {
                await oTran.RollbackAsync();
                await oTran.DisposeAsync();
                oTran = null;
                blnTranActive = false;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
