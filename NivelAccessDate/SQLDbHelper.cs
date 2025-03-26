using System.Data;
using System.Configuration;
using System;
using Oracle.DataAccess.Client;
using System.Reflection.Emit;

namespace NivelAccesDate
{
    /// <summary>
    /// contine metode generice de interogare, respectiv actualizare a bazei de date
    /// </summary>
    public static class SqlDBHelper
    {
        private const int EROARE_LA_EXECUTIE = 0;

        private static string _connectionString = null;
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = ConfigurationManager.AppSettings.Get("StringConectareMyBD");
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// executa un script
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="errorMessage"></param>
        /// <returns>returneaza o tabela si/sau erorile, afiseaza direct in gridview. nu mai face obiecte</returns>
        public static DataTable ExecuteScript(string sql, out string errorMessage)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    conn.Open();

                    OracleCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql.Replace("\r\n"," ");
                    OracleDataReader reader = cmd.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    errorMessage = null;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "\r\n" + ex.Message.ToString() + "\r\n";
                return null;
            }

        }

        public static DataTable GetDataTable(string TABLE, string columns, string conditions, out string errors)
        {
            if (string.IsNullOrEmpty(columns))
                columns = "*";

            if (string.IsNullOrEmpty(conditions))
                conditions = "";
            else
                conditions = " where " + conditions;

            return ExecuteScript("select " + columns + " from " + TABLE + conditions, out errors);
        }

        /// <summary>
        /// executa o instructiune SQL text
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>returneaza valorile primite ca un obiect generic de tip 'DataSet'</returns>
        public static DataSet ExecuteDataSet(string sql, CommandType cmdType)
        {
            using (DataSet ds = new DataSet())
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.CommandType = cmdType;
                    try
                    {
                        new OracleDataAdapter(cmd).Fill(ds);
                    }
                    catch (OracleException ex)
                    {
                        //salveaza exceptii in fisiere log
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// executa o instructiune de tip SELECT
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>returneaza valorile primite ca un obiect generic de tip 'DataSet'</returns>
        public static DataSet ExecuteDataSet(string sql, CommandType cmdType, params OracleParameter[] parameters)
        {
            using (DataSet ds = new DataSet())
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.CommandType = cmdType;
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }

                    try
                    {
                        new OracleDataAdapter(cmd).Fill(ds);
                    }
                    catch (OracleException ex)
                    {
                        //salveaza exceptii in fisiere log
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// executa instructiuni INSERT/UPDATE/DELETE 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns> returneaza 'true' daca instructiunea a fost executata cu success</returns>
        public static bool ExecuteNonQuery(string sql, CommandType cmdType, params OracleParameter[] parameters)
        {
            int rezult = EROARE_LA_EXECUTIE;
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.CommandType = cmdType;
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }

                    try
                    {
                        cmd.Connection.Open();
                        rezult = cmd.ExecuteNonQuery();
                        Console.WriteLine(rezult);
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine(ex.Message);
                        //salveaza exceptii in fisiere log
                    }
                }
            }
            return Convert.ToBoolean(rezult);
        }
    }
}
