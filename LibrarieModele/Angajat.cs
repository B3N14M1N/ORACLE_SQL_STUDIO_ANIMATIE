using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModele
{
    public class Angajat
    {
        public int Angajat_Id { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Email { get; set; }
        public string Tara { get; set; }
        public string Oras { get; set; }
        public int Job_Id { get; set; }
        public int Manager_Id { get; set; }
        public float Salariu { get; set; }
        public DateTime Data_Angajare { get; set; }
        public string Telefon { get; set; }

        public Angajat() { }

        public Angajat( string nume, string prenume, string email, string tara, string oras, int job_Id, int manager_Id, float salariu, DateTime data_Angajare, string telefon, int angajat_Id = 0)
        {
            Angajat_Id = angajat_Id;
            Nume = nume;
            Prenume = prenume;
            Email = email;
            Tara = tara;
            Oras = oras;
            Job_Id = job_Id;
            Manager_Id = manager_Id;
            Salariu = salariu;
            Data_Angajare = data_Angajare;
            Telefon = telefon;
        }

        public Angajat(DataRow linieDB)
        {
            Angajat_Id = Convert.ToInt32(linieDB["angajat_id"].ToString());
            Nume = linieDB["nume"].ToString();
            Prenume = linieDB["prenume"].ToString();
            Email = linieDB["email"].ToString();
            Tara = linieDB["tara"].ToString();
            Oras = linieDB["oras"].ToString();
            Job_Id = linieDB["job_id"] == DBNull.Value ? 0 : Convert.ToInt32(linieDB["job_id"].ToString());
            Manager_Id = linieDB["manager_id"] == DBNull.Value ? 0 : Convert.ToInt32(linieDB["manager_id"].ToString());
            Salariu = (float) Convert.ToDouble(linieDB["salariu"].ToString());
            Data_Angajare = Convert.ToDateTime(linieDB["data_angajare"].ToString());
            Telefon = linieDB["telefon"].ToString();
        }
    }
}
