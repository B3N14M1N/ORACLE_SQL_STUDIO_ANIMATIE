using System;
using System.Collections.Generic;
using System.Data;
using LibrarieModele;
using Oracle.DataAccess.Client;

namespace NivelAccesDate
{
    public static class AdministrareProiect
    {
        private const int PRIMUL_TABEL = 0;
        private const int PRIMA_LINIE = 0;
        private static string tabel = "proiect_3134a_cb";
        public static string TABLE { get { return tabel; } }
        private static string SEQ = "seq_proiect_3134a_cb.nextval";

        public static List<Proiect> GetProiecte()
        {
            var result = new List<Proiect>();
            var dsJobs = SqlDBHelper.ExecuteDataSet("select * from " + TABLE, CommandType.Text);

            foreach (DataRow linieDB in dsJobs.Tables[PRIMUL_TABEL].Rows)
            {
                result.Add(new Proiect(linieDB));
            }
            return result;
        }

        public static List<Proiect> GetProiecte(string condition)
        {
            var result = new List<Proiect>();
            var dsProiecte = SqlDBHelper.ExecuteDataSet("select * from " + TABLE + " where " + condition, CommandType.Text);

            foreach (DataRow linieDB in dsProiecte.Tables[PRIMUL_TABEL].Rows)
            {
                result.Add(new Proiect(linieDB));
            }
            return result;
        }
        public static Proiect GetProiect(int id)
        {
            Proiect result = null;
            var dsJobs = SqlDBHelper.ExecuteDataSet("select * from " + TABLE + " where Proiect_Id = :Proiect_Id", CommandType.Text,
                new OracleParameter(":Proiect_Id", OracleDbType.Int32, id, ParameterDirection.Input));

            if (dsJobs.Tables[PRIMUL_TABEL].Rows.Count > 0)
            {
                DataRow linieDB = dsJobs.Tables[PRIMUL_TABEL].Rows[PRIMA_LINIE];
                result = new Proiect(linieDB);
            }
            return result;
        }

        public static bool AddProiect(Proiect proiect)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "INSERT INTO " + TABLE + " VALUES (" + SEQ + ", :Denumire, :Pret, :Status, :Director_Id, :Producator_Id, :Coordonator_Id, :Termen_Inceput, :Termen_Finalizare, :Client_Id)", CommandType.Text,
                new OracleParameter(":Denumire", OracleDbType.NVarchar2, proiect.Denumire.ToString(),ParameterDirection.Input),
                new OracleParameter(":Pret", OracleDbType.Double, proiect.Pret, ParameterDirection.Input),
                new OracleParameter(":Status", OracleDbType.NVarchar2, proiect.Status, ParameterDirection.Input),
                proiect.Director_Id == 0 ? new OracleParameter(":Director_ID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Director_ID", OracleDbType.Int32, proiect.Director_Id, ParameterDirection.Input),
                proiect.Producator_Id == 0 ? new OracleParameter(":Producator_ID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Producator_ID", OracleDbType.Int32, proiect.Producator_Id, ParameterDirection.Input),
                proiect.Coordonator_Id == 0 ? new OracleParameter(":Coordonator_ID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Coordonator_ID", OracleDbType.Int32, proiect.Coordonator_Id, ParameterDirection.Input),
                new OracleParameter(":Termen_Inceput", OracleDbType.Date, proiect.Termen_Inceput, ParameterDirection.Input),
                new OracleParameter(":Termen_Finalizare", OracleDbType.Date, proiect.Termen_Finalizare, ParameterDirection.Input),
                proiect.Client_Id == 0 ? new OracleParameter(":Client_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Client_Id", OracleDbType.Int32, proiect.Client_Id, ParameterDirection.Input));
        }

        public static bool UpdateProiect(Proiect proiect)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "UPDATE " + TABLE + " set Denumire = :Denumire, Pret = :Pret, Status = :Status, Director_ID = :Director_Id, Producator_Id = :Producator_Id, Coordonator_Id = :Coordonator_Id, Termen_Inceput = :Termen_Inceput, Termen_Finalizare = :Termen_Finalizare, Client_Id = :Client_Id where Proiect_Id = :Proiect_Id", CommandType.Text,
                new OracleParameter(":Denumire", OracleDbType.NVarchar2, proiect.Denumire.ToString(), ParameterDirection.Input),
                new OracleParameter(":Pret", OracleDbType.Double, proiect.Pret, ParameterDirection.Input),
                new OracleParameter(":Status", OracleDbType.NVarchar2, proiect.Status, ParameterDirection.Input),
                proiect.Director_Id == 0 ? new OracleParameter(":Director_ID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Director_ID", OracleDbType.Int32, proiect.Director_Id, ParameterDirection.Input),
                proiect.Producator_Id == 0 ? new OracleParameter(":Producator_ID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Producator_ID", OracleDbType.Int32, proiect.Producator_Id, ParameterDirection.Input),
                proiect.Coordonator_Id == 0 ? new OracleParameter(":Coordonator_ID", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Coordonator_ID", OracleDbType.Int32, proiect.Coordonator_Id, ParameterDirection.Input),
                new OracleParameter(":Termen_Inceput", OracleDbType.Date, proiect.Termen_Inceput, ParameterDirection.Input),
                new OracleParameter(":Termen_Finalizare", OracleDbType.Date, proiect.Termen_Finalizare, ParameterDirection.Input),
                proiect.Client_Id == 0 ? new OracleParameter(":Client_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Client_Id", OracleDbType.Int32, proiect.Client_Id, ParameterDirection.Input),
                new OracleParameter(":Proiect_Id", OracleDbType.Int32, proiect.Proiect_Id, ParameterDirection.Input));
        }
        public static bool DeleteProiect(Proiect proiect)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "DELETE from " + TABLE + " where Proiect_Id = :Proiect_Id", CommandType.Text,
                new OracleParameter(":Proiect_Id", OracleDbType.Int32, proiect.Proiect_Id, ParameterDirection.Input));
        }
    }
}
