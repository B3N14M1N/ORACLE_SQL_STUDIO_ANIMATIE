using System;
using System.Collections.Generic;
using System.Data;
using LibrarieModele;
using Oracle.DataAccess.Client;

namespace NivelAccesDate
{
    public static class AdministrareAngajat
    {
        private const int PRIMUL_TABEL = 0;
        private const int PRIMA_LINIE = 0;
        private static string tabel = "angajat_3134a_cb";
        public static string TABLE { get { return tabel; } }
        private static string SEQ = "seq_angajat_3134a_cb.nextval";

        public static List<Angajat> GetAngajati()
        {
            var result = new List<Angajat>();
            var dsAngajati = SqlDBHelper.ExecuteDataSet("select * from " + TABLE, CommandType.Text);

            foreach (DataRow linieDB in dsAngajati.Tables[PRIMUL_TABEL].Rows)
            {
                result.Add(new Angajat(linieDB));
            }
            return result;
        }

        public static List<Angajat> GetAngajati(string condition)
        {
            var result = new List<Angajat>();
            var dsAngajati = SqlDBHelper.ExecuteDataSet("select * from " + TABLE + " where " + condition, CommandType.Text);

            foreach (DataRow linieDB in dsAngajati.Tables[PRIMUL_TABEL].Rows)
            {
                result.Add(new Angajat(linieDB));
            }
            return result;
        }

        public static Angajat GetAngajat(int id)
        {
            Angajat result = null;
            var dsAngajati = SqlDBHelper.ExecuteDataSet("select * from " + TABLE + " where Angajat_Id = :Angajat_Id", CommandType.Text,
                new OracleParameter(":Angajat_Id", OracleDbType.Int32, id, ParameterDirection.Input));

            if (dsAngajati.Tables[PRIMUL_TABEL].Rows.Count > 0)
            {
                DataRow linieDB = dsAngajati.Tables[PRIMUL_TABEL].Rows[PRIMA_LINIE];
                result = new Angajat(linieDB);
            }
            return result;
        }

        public static bool AddAngajat(Angajat angajat)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "INSERT INTO " + TABLE + " VALUES (" + SEQ + ", :Nume, :Prenume, :Email, :Tara , :Oras, :Job_Id, :Manager_Id, :Salariu, :Data_Angajare, :Telefon)", CommandType.Text,
                string.IsNullOrEmpty(angajat.Nume) ? new OracleParameter(":Nume", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Nume", OracleDbType.NVarchar2, angajat.Nume, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Prenume) ? new OracleParameter(":Prenume", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Prenume", OracleDbType.NVarchar2, angajat.Prenume, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Email) ? new OracleParameter(":Email", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Email", OracleDbType.NVarchar2, angajat.Email, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Tara) ? new OracleParameter(":Tara", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Tara", OracleDbType.NVarchar2, angajat.Tara, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Oras) ? new OracleParameter(":Oras", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Oras", OracleDbType.NVarchar2, angajat.Oras, ParameterDirection.Input),
                angajat.Job_Id == 0 ? new OracleParameter(":Job_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Job_Id", OracleDbType.Int32, angajat.Job_Id, ParameterDirection.Input),
                angajat.Manager_Id == 0 ? new OracleParameter(":Manager_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Manager_Id", OracleDbType.Int32, angajat.Manager_Id, ParameterDirection.Input),
                new OracleParameter(":Salariu", OracleDbType.NVarchar2, angajat.Salariu, ParameterDirection.Input),
                new OracleParameter(":Data_Angajare", OracleDbType.Date, angajat.Data_Angajare, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Telefon) ? new OracleParameter(":Telefon", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Telefon", OracleDbType.NVarchar2, angajat.Telefon, ParameterDirection.Input));
        }

        public static bool UpdateAngajat(Angajat angajat)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "UPDATE " + TABLE + " set Nume = :Nume, Prenume = :Prenume, Email = :Email, Tara = :Tara , Oras = :Oras, Job_ID = :Job_Id, Manager_ID = :Manager_Id, Salariu = :Salariu, Data_Angajare = :Data_Angajare, Telefon = :Telefon where Angajat_Id = :Angajat_Id", CommandType.Text,
                string.IsNullOrEmpty(angajat.Nume) ? new OracleParameter(":Nume", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Nume", OracleDbType.NVarchar2, angajat.Nume, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Prenume) ? new OracleParameter(":Prenume", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Prenume", OracleDbType.NVarchar2, angajat.Prenume, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Email) ? new OracleParameter(":Email", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Email", OracleDbType.NVarchar2, angajat.Email, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Tara) ? new OracleParameter(":Tara", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Tara", OracleDbType.NVarchar2, angajat.Tara, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Oras) ? new OracleParameter(":Oras", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Oras", OracleDbType.NVarchar2, angajat.Oras, ParameterDirection.Input),
                angajat.Job_Id == 0 ? new OracleParameter(":Job_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Job_Id", OracleDbType.Int32, angajat.Job_Id, ParameterDirection.Input),
                angajat.Manager_Id == 0 ? new OracleParameter(":Manager_Id", OracleDbType.Int32, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Manager_Id", OracleDbType.Int32, angajat.Manager_Id, ParameterDirection.Input),
                new OracleParameter(":Salariu", OracleDbType.NVarchar2, angajat.Salariu, ParameterDirection.Input),
                new OracleParameter(":Data_Angajare", OracleDbType.Date, angajat.Data_Angajare, ParameterDirection.Input),
                string.IsNullOrEmpty(angajat.Telefon) ? new OracleParameter(":Telefon", OracleDbType.NVarchar2, DBNull.Value, ParameterDirection.Input)
                    : new OracleParameter(":Telefon", OracleDbType.NVarchar2, angajat.Telefon, ParameterDirection.Input),
                new OracleParameter(":Angajat_Id", OracleDbType.Int32, angajat.Angajat_Id, ParameterDirection.Input));
        }
        public static bool DeleteAngajat(Angajat angajat)
        {
            SqlDBHelper.ExecuteDataSet("UPDATE " + TABLE + " set manager_id = null where manager_id = " + angajat.Angajat_Id, CommandType.Text);

            SqlDBHelper.ExecuteDataSet("UPDATE " + AdministrareProiect.TABLE + " set director_id = null where director_id = " + angajat.Angajat_Id, CommandType.Text);

            SqlDBHelper.ExecuteDataSet("UPDATE " + AdministrareProiect.TABLE + " set producator_id = null where producator_id = " + angajat.Angajat_Id, CommandType.Text);

            SqlDBHelper.ExecuteDataSet("UPDATE " + AdministrareProiect.TABLE + " set coordonator_id = null where coordonator_id = " + angajat.Angajat_Id, CommandType.Text);


            return SqlDBHelper.ExecuteNonQuery(
                "DELETE from " + TABLE + " where Angajat_Id = :Angajat_Id", CommandType.Text,
                new OracleParameter(":Angajat_Id", OracleDbType.Int32, angajat.Angajat_Id, ParameterDirection.Input));
        }
    }
}
