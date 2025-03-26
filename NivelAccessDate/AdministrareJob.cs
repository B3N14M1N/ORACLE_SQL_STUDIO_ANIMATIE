using System;
using System.Collections.Generic;
using System.Data;
using LibrarieModele;
using Oracle.DataAccess.Client;

namespace NivelAccesDate
{
    public static class AdministrareJob
    {
        private const int PRIMUL_TABEL = 0;
        private const int PRIMA_LINIE = 0;
        private static string tabel = "job_3134a_cb";
        public static string TABLE { get { return tabel; } }
        private static string SEQ = "seq_job_3134a_cb.nextval";



        public static List<Job> GetJobs()
        {
            var result = new List<Job>();
            var dsJobs = SqlDBHelper.ExecuteDataSet("select * from " + TABLE, CommandType.Text);

            foreach (DataRow linieDB in dsJobs.Tables[PRIMUL_TABEL].Rows)
            {
                result.Add(new Job(linieDB));
            }
            return result;
        }

        public static Job GetJob(int id)
        {
            Job result = null;
            var dsJobs = SqlDBHelper.ExecuteDataSet("select * from " + TABLE + " where Job_Id = :Job_Id", CommandType.Text,
                new OracleParameter(":Job_Id", OracleDbType.Int32, id, ParameterDirection.Input));

            if (dsJobs.Tables[PRIMUL_TABEL].Rows.Count > 0)
            {
                DataRow linieDB = dsJobs.Tables[PRIMUL_TABEL].Rows[PRIMA_LINIE];
                result = new Job(linieDB);
            }
            return result;
        }

        public static bool AddJob(Job job)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "INSERT INTO " + TABLE + " VALUES (" + SEQ + ", :Denumire, :Descriere)", CommandType.Text,
                new OracleParameter(":Denumire", OracleDbType.NVarchar2, job.Denumire, ParameterDirection.Input),
                string.IsNullOrEmpty(job.Descriere) ? new OracleParameter(":Descriere", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                                : new OracleParameter(":Descriere", OracleDbType.NVarchar2, job.Descriere, ParameterDirection.Input));
        }

        public static bool UpdateJob(Job job)
        {
            Console.WriteLine(job.Denumire,job.Descriere,job.Job_Id);
            return SqlDBHelper.ExecuteNonQuery(
                "UPDATE " + TABLE + " SET denumire = :Denumire, descriere = :Descriere where job_id = :Job_Id", CommandType.Text,
                new OracleParameter(":Denumire", OracleDbType.NVarchar2, job.Denumire, ParameterDirection.Input),
                string.IsNullOrEmpty(job.Descriere) ? new OracleParameter(":Descriere", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                                : new OracleParameter(":Descriere", OracleDbType.NVarchar2, job.Descriere, ParameterDirection.Input),
                job.Job_Id == 0 ? new OracleParameter(":Job_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                                : new OracleParameter(":Job_Id", OracleDbType.Int32, job.Job_Id, ParameterDirection.Input));
        }
        public static bool DeleteJob(Job job)
        {
            SqlDBHelper.ExecuteScript("UPDATE " + AdministrareAngajat.TABLE + " set job_id = null where job_id = " + job.Job_Id, out string error);
            Console.WriteLine(error);

            return SqlDBHelper.ExecuteNonQuery(
                "DELETE from " + TABLE + " where Job_Id = :Job_Id", CommandType.Text,
                job.Job_Id == 0 ? new OracleParameter(":Job_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                                : new OracleParameter(":Job_Id", OracleDbType.Int32, job.Job_Id, ParameterDirection.Input));
        }
    }
}
