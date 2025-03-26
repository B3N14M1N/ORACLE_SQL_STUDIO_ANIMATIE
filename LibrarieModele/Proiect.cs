using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModele
{
    public class Proiect
    {
        public int Proiect_Id { get; set; }
        public string Denumire { get; set; }
        public float Pret { get; set; }
        public string Status{ get; set; }
        public int Director_Id { get; set; }
        public int Producator_Id { get; set; }
        public int Coordonator_Id { get; set; }
        public DateTime Termen_Inceput { get; set; }
        public DateTime Termen_Finalizare { get; set; }
        public int Client_Id { get; set; }

        public Proiect() { }

        public Proiect(string denumire, float pret, string status,
            int director_Id, int producator_Id, int coordonator_Id,
            DateTime termen_Inceput, DateTime termen_Finalizare, int client_Id, int proiect_Id = 0)
        {
            Proiect_Id = proiect_Id;
            Denumire = denumire;
            Pret = pret;
            Status = status;
            Director_Id = director_Id;
            Producator_Id = producator_Id;
            Coordonator_Id = coordonator_Id;
            Termen_Inceput = termen_Inceput;
            Termen_Finalizare = termen_Finalizare;
            Client_Id = client_Id;
        }

        public Proiect(DataRow linieDB)
        {
            Proiect_Id = Convert.ToInt32(linieDB["proiect_id"].ToString());
            Denumire = linieDB["denumire"].ToString();
            Pret = (float)Convert.ToDouble(linieDB["pret"].ToString());
            Status = linieDB["status"].ToString();
            Director_Id = linieDB["director_id"] == DBNull.Value ? 0 : Convert.ToInt32(linieDB["director_id"].ToString());
            Producator_Id = linieDB["producator_id"] == DBNull.Value ? 0 : Convert.ToInt32(linieDB["producator_id"].ToString());
            Coordonator_Id = linieDB["coordonator_id"] == DBNull.Value ? 0 : Convert.ToInt32(linieDB["coordonator_id"].ToString());
            Termen_Inceput = Convert.ToDateTime(linieDB["termen_inceput"].ToString());
            Termen_Finalizare = Convert.ToDateTime(linieDB["termen_finalizare"].ToString());
            Client_Id = linieDB["client_id"] == DBNull.Value ? 0 : Convert.ToInt32(linieDB["client_id"].ToString());
        }
    }
}
