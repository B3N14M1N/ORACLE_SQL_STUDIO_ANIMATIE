using System;
using System.Data;

namespace LibrarieModele
{
    public class Job
    {
        public int Job_Id { get; set; }
        public string Denumire { get; set; }
        public string Descriere { get; set; }

        public Job() { }

        public Job(string denumire, string descriere, int job_Id = 0)
        {
            Job_Id = job_Id;
            Denumire = denumire;
            Descriere = descriere;
        }

        public Job(DataRow linieDB)
        {
            Job_Id = Convert.ToInt32(linieDB["job_id"].ToString());
            Denumire = linieDB["denumire"].ToString();
            Descriere = linieDB["descriere"].ToString();
        }
    }
}
