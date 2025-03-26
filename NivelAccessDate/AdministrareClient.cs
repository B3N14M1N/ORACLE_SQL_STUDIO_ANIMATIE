using System;
using System.Collections.Generic;
using System.Data;
using LibrarieModele;
using Oracle.DataAccess.Client;

namespace NivelAccesDate
{
    public static class AdministrareClient
    {
        private const int PRIMUL_TABEL = 0;
        private const int PRIMA_LINIE = 0;
        private static string tabel = "client_3134a_cb";
        public static string TABLE { get { return tabel; } }
        private static string SEQ = "seq_client_3134a_cb.nextval";

        public static List<Client> GetClienti()
        {
            var result = new List<Client>();
            var dsClienti = SqlDBHelper.ExecuteDataSet("select * from " + TABLE, CommandType.Text);

            foreach (DataRow linieDB in dsClienti.Tables[PRIMUL_TABEL].Rows)
            {
                result.Add(new Client(linieDB));
            }
            return result;
        }

        public static Client GetClient(int id)
        {
            Client result = null;
            var dsClienti = SqlDBHelper.ExecuteDataSet("select * from " + TABLE + " where Client_Id = :CLient_Id", CommandType.Text,
                new OracleParameter(":Client_Id", OracleDbType.Int32, id, ParameterDirection.Input));

            if (dsClienti.Tables[PRIMUL_TABEL].Rows.Count > 0)
            {
                DataRow linieDB = dsClienti.Tables[PRIMUL_TABEL].Rows[PRIMA_LINIE];
                result = new Client(linieDB);
            }
            return result;
        }

        public static bool AddClient(Client client)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "INSERT INTO " + TABLE + " VALUES (" + SEQ + ", :Nume, :Email, :Telefon, :Tara, :Oras)", CommandType.Text,
                new OracleParameter(":Nume", OracleDbType.NVarchar2, client.Nume, ParameterDirection.Input),
                new OracleParameter(":Email", OracleDbType.NVarchar2, client.Email, ParameterDirection.Input),
                new OracleParameter(":Telefon", OracleDbType.NVarchar2, client.Telefon, ParameterDirection.Input),
                new OracleParameter(":Tara", OracleDbType.NVarchar2, client.Tara, ParameterDirection.Input),
                new OracleParameter(":Oras", OracleDbType.NVarchar2, client.Oras, ParameterDirection.Input));
        }

        public static bool UpdateClient(Client client)
        {
            return SqlDBHelper.ExecuteNonQuery(
                "UPDATE " + TABLE + " set Nume = :Nume, Email = :Email, Telefon = :Telefon, Tara = :Tara, Oras = :Oras where Client_Id = :Client_Id", CommandType.Text,
                new OracleParameter(":Nume", OracleDbType.NVarchar2, client.Nume, ParameterDirection.Input),
                new OracleParameter(":Email", OracleDbType.NVarchar2, client.Email, ParameterDirection.Input),
                new OracleParameter(":Telefon", OracleDbType.NVarchar2, client.Telefon, ParameterDirection.Input),
                new OracleParameter(":Tara", OracleDbType.NVarchar2, client.Tara, ParameterDirection.Input),
                new OracleParameter(":Oras", OracleDbType.NVarchar2, client.Oras, ParameterDirection.Input),
                new OracleParameter(":Client_Id", OracleDbType.Int32, client.Client_Id, ParameterDirection.Input));
        }
        public static bool DeleteClient(Client client)
        {
            SqlDBHelper.ExecuteDataSet("UPDATE " + AdministrareProiect.TABLE + " set client_id = null where client_id = " + client.Client_Id, CommandType.Text);

            return SqlDBHelper.ExecuteNonQuery(
                "DELETE from " + TABLE + " where Client_Id = :Client_Id", CommandType.Text,
                new OracleParameter(":Client_Id", OracleDbType.Int32, client.Client_Id, ParameterDirection.Input));
        }
    }
}
