using System;
using System.Data;

namespace LibrarieModele
{
    public class Client
    {
        public int Client_Id { get; set; }
        public string Nume { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Tara { get; set; }
        public string Oras { get; set; }

        public Client() { }

        public Client(string nume, string email, string telefon, string tara, string oras, int client_Id = 0)
        {
            Client_Id = client_Id;
            Nume = nume;
            Email = email;
            Telefon = telefon;
            Tara = tara;
            Oras = oras;
        }

        public Client(DataRow linieDB)
        {
            Client_Id = Convert.ToInt32(linieDB["client_id"].ToString());
            Nume = linieDB["nume"].ToString();
            Email = linieDB["email"].ToString();
            Telefon = linieDB["telefon"].ToString();
            Tara = linieDB["tara"].ToString();
            Oras = linieDB["oras"].ToString();
        }
    }
}
