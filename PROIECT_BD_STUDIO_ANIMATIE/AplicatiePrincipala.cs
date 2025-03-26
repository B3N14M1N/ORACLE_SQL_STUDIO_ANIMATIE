using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using LibrarieModele;
using NivelAccesDate;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PROIECT_BD_STUDIO_ANIMATIE
{
    public partial class AplicatiePrincipala : Form
    {
        private List<Job> jobs = new List<Job>();
        private const int FORM_HEIGHT = 681;
        private const int FORM_WIDTH = 1264;
        private List<string> StatusProiecte = new List<string>() { "anulat", "intarziat", "planificat", "productie", "revizuire", "suspendat", "finalizat" };
        private string denumire_job_director = "director";
        private string denumire_job_producator = "producator";
        private string denumire_job_coordonator = "coordonator";
        #region FORM
        public AplicatiePrincipala()
        {
            InitializeComponent();
            comboBoxAddProiecteStatus.Items.Clear();
            comboBoxAddProiecteStatus.DataSource = StatusProiecte;
            comboBoxAddProiecteStatus.SelectedIndex = -1;
            comboBoxViewProiectFilterStatus.Items.Clear();
            comboBoxViewProiectFilterStatus.DataSource = StatusProiecte;
            comboBoxViewProiectFilterStatus.SelectedIndex = -1;
            comboBoxModificaProiecteStatus.Items.Clear();
            comboBoxModificaProiecteStatus.DataSource = StatusProiecte;
            comboBoxModificaProiecteStatus.SelectedIndex = -1;
            SetJobList();
        }
        private void btnFormClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close the program?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void btnFormSize_Click(object sender, EventArgs e)
        {
            if (this.Size == Screen.PrimaryScreen.WorkingArea.Size)
            {
                Size newSize = new Size(FORM_WIDTH, FORM_HEIGHT);
                Point newLocation = (Point)(Screen.PrimaryScreen.WorkingArea.Size - newSize);
                this.Location = new Point(newLocation.X / 2, newLocation.Y / 2);
                this.Size = newSize;
            }
            else
            {
                this.Location = new Point(0, 0);

                this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            }
        }

        private void btnFormMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region LOG OUTPUT ERRORS
        private void LogMessage(string message)
        {
            txtBoxOutput.AppendText(message + "\r\n");
        }
        private void LogError(string message)
        {
            txtBoxErrors.Text = "";
            tabControlLog.SelectedIndex = 1;
            txtBoxErrors.AppendText(DateTime.Now + ": " + message + "\r\n");

            if (scLeft.SplitterDistance > this.Size.Height - 131)
                scLeft.SplitterDistance = this.Size.Height - 131;
        }
        public void ClearLogError()
        {
            tabControlLog.SelectedIndex = 0;
            txtBoxErrors.Clear();
        }
        public void ClearLogMessage()
        {
            txtBoxOutput.Clear();
        }

        private void btnClearLogOutput_Click(object sender, EventArgs e)
        {
            ClearLogMessage();
        }
        private void btnClearLogErrors_Click(object sender, EventArgs e)
        {
            ClearLogError();
        }
        #endregion

        #region SCRIPT WINDOW
        private void btnScriptOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Select SQL Script",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "sql",
                Filter = "SQL Script (*.sql)|*.sql",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader sr = new StreamReader(openFileDialog1.FileName);
                    LogMessage(DateTime.Now + ": Opened script (\"" + openFileDialog1.FileName + "\") successfully.");
                    txtBoxScript.Text = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception exception)
                {
                    LogError(DateTime.Now + ": error opening File (" + openFileDialog1.FileName + ").");
                    LogError(exception.ToString());
                }
            }
            else
            {
                LogMessage(DateTime.Now + ": Open script cancelled.");
            }
        }
        private void btnScriptSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Save SQL Script",

                DefaultExt = "sql",
                Filter = "SQL Script (*.sql)|*.sql",
                RestoreDirectory = true,
            };
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = saveFileDialog1.FileName;
                    StreamWriter sw = new StreamWriter(path);
                    sw.Write(txtBoxScript.Text);
                    sw.Close();
                    LogMessage(DateTime.Now + ": Saved SQL Script successfully to \"" + saveFileDialog1.FileName + "\".");
                }
                catch (Exception exception)
                {
                    LogError(DateTime.Now + ": error writing to File (" + saveFileDialog1.FileName + ").");
                    LogError(exception.ToString());
                }
            }
            else
            {
                LogMessage(DateTime.Now + ": Save script cancelled.");
            }
        }
        private void btnScriptClear_Click(object sender, EventArgs e)
        {
            if (txtBoxScript.Text != "")
            {
                txtBoxScript.Clear();
                LogMessage(DateTime.Now + ": Window cleared.");
            }
        }
        private void btnScriptRun_Click(object sender, EventArgs e)
        {
            ClearLogError();
            var scripts = txtBoxScript.Text.Replace("\r\n", " ").Split(';');
            string errors = "";
            int i = 0, j = 0;
            foreach(string script in scripts)
            {
                if(!string.IsNullOrEmpty(script.Trim()))
                using (DataTable dataTable = SqlDBHelper.ExecuteScript(script.Trim(), out string errorMessage))
                {
                    j++;
                    if (dataTable != null)
                    {
                        FillDataGridView(dataTable);
                        i++;
                    }
                    else
                    {
                            errors += "script: " + script.Trim() + "\r\n" + errorMessage + "\r\n";
                    }
                }
            }
            LogMessage(DateTime.Now + ": " + i + "/" + j + " script executed successfully.");
            if (!string.IsNullOrEmpty(errors))
                LogError(errors);
        }
        #endregion

        #region DATA GRID VIEW
        public void FillDataGridView(DataTable dataTable)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dataTable;
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_Leave(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
        #endregion

        #region ADDING METHODS
        
        #region CHECK INPUTS
        private bool isEmailOk(string email, out string error)
        {
            bool ok = true;
            error = "";
            if (string.IsNullOrEmpty(email))
            {
                ok = false;
                error += "Câmp obligatoriu: \"E-MAIL\"!\r\n";
            }
            else
            {
                if (!email.Contains("@"))
                {
                    ok = false;
                    error += "Adresă invalidă.\r\n";
                }
            }
            return ok;
        }
        private bool isTelefonOk(string telefon, out string error)
        {
            bool ok = true;
            error = "";
            if (!string.IsNullOrEmpty(telefon))
            {
                if (telefon.Length < 10 || !isNumber(telefon))
                {
                    ok = false;
                    error += "Număr invalid.\r\n";
                }
            }
            return ok;
        }
        private bool isNumber(string number)
        {
            foreach (var c in Regex.Split(number, string.Empty))
            {
                if (!"0123456789".Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
        private bool isValidAngajatID(string angajatID, out int id)
        {
            id = 0;
            if (!string.IsNullOrEmpty(angajatID))
            {
                if (isNumber(angajatID))
                {
                    id = int.Parse(angajatID);
                    if (AdministrareAngajat.GetAngajat(id) == null)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private bool isValidAngajatJob(int angajatID, int jobID)
        {
            Angajat angajat = AdministrareAngajat.GetAngajat(angajatID);
            return angajat != null && angajat.Job_Id == jobID;
        }
        private bool isValidClientID(string clientID, out int id)
        {
            id = 0;
            if (!string.IsNullOrEmpty(clientID))
            {
                if (isNumber(clientID))
                {
                    id = int.Parse(clientID);
                    if (AdministrareClient.GetClient(id) == null)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dateTimePicker = sender as DateTimePicker;
            if (dateTimePicker.Name.ToLower().Contains("add")){
                if (dateTimePickerAddProiectInceput.Value.CompareTo(dateTimePickerAddProiectFinalizare.Value) > 0)
                {
                    dateTimePickerAddProiectFinalizare.Value = dateTimePickerAddProiectInceput.Value;
                }
            }
            if (dateTimePicker.Name.ToLower().Contains("modifica"))
            {
                if (dateTimePickerModificaProiecteTermenInceput.Value.CompareTo(dateTimePickerModificaProiecteTermenFinalizare.Value) > 0)
                {
                    dateTimePickerModificaProiecteTermenFinalizare.Value = dateTimePickerModificaProiecteTermenInceput.Value;
                }
            }
        }

        private Job CreateJob(TextBox tDenumire,
            TextBox tDescriere,
            out string error)
        {
            Job job = null;
            error = null;
            if (tDenumire.Text != "")
            {
                job = new Job(tDenumire.Text.Trim().ToLower(), tDescriere.Text.Trim().ToLower());
            }
            else
            {
                error = "Invaild DENUMIRE!";
            }
            return job;
        }

        private Angajat CreateAngajat(TextBox tNume,
            TextBox tPrenume,
            TextBox tTelefon,
            TextBox tEmail,
            TextBox tTara,
            TextBox tOras,
            TextBox tManager,
            ComboBox cJob,
            NumericUpDown nSalariu,
            DateTimePicker dData,
            out string error)
        {
            Angajat angajat = null;
            bool ok = true;
            error = "";
            string Nume = tNume.Text.Trim().ToLower();
            string Prenume = tPrenume.Text.Trim().ToLower();
            string Telefon = tTelefon.Text.Trim().ToLower();
            string Email = tEmail.Text.Trim().ToLower();
            string Denumire_Job = cJob.GetItemText(cJob.SelectedItem).Trim().ToLower();
            string Tara = tTara.Text.Trim().ToLower();
            string Oras = tOras.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(Nume))
            {
                ok = false;
                error += "Câmp obligatoriu: \"NUME\"!\r\n";
            }

            if (string.IsNullOrEmpty(Prenume))
            {
                ok = false;
                error += "Câmp obligatoriu: \"PRENUME\"!\r\n";
            }
            string temp;

            if (!isEmailOk(Email, out temp))
            {
                ok = false;
                error += temp;
            }

            if (!string.IsNullOrEmpty(Telefon) && !isTelefonOk(Telefon, out temp))
            {
                ok = false;
                error += temp;
            }

            if (string.IsNullOrEmpty(Tara))
            {
                ok = false;
                error += "Câmp obligatoriu: \"TARA\"!\r\n";
            }

            if (string.IsNullOrEmpty(Oras))
            {
                ok = false;
                error += "Câmp obligatoriu: \"ORAS\"!\r\n";
            }
            int Manager = 0;
            temp = tManager.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(temp))
            {
                if (!isValidAngajatID(temp, out Manager))
                {
                    ok = false;
                    error += "Invalid ID: \"Manager_ID\"!\r\n";
                }
            }

            if (ok)
            {
                int job_id = 0;
                if (cJob.SelectedIndex >= 0)
                {
                    job_id = GetJobIdByName(Denumire_Job);
                }

                angajat = new Angajat(Nume, Prenume, Email, Tara, Oras,
                    job_id, Manager, (float)(nSalariu.Value), dData.Value.Date, Telefon);
            }
            return angajat;
        }

        private Client CreateClient(TextBox tNume,
            TextBox tEmail,
            TextBox tTelefon,
            TextBox tTara,
            TextBox tOras,
            out string error)
        {
            Client client = null;
            error = "";

            string Nume = tNume.Text.Trim().ToLower();
            string Email = tEmail.Text.Trim().ToLower();
            string Telefon = tTelefon.Text.Trim().ToLower();
            string Tara = tTara.Text.Trim().ToLower();
            string Oras = tOras.Text.Trim().ToLower();

            bool ok = true;

            if (string.IsNullOrEmpty(Nume))
            {
                ok = false;
                error += "Câmp obligatoriu: \"NUME\"!\r\n";
            }
            string temp;
            if (!isEmailOk(Email, out temp))
            {
                ok = false;
                error += temp;
            }
            if (!string.IsNullOrEmpty(Telefon) && !isTelefonOk(Telefon, out temp))
            {
                ok = false;
                error += temp;
            }
            if (ok)
            {
                client = new Client(Nume, Email, Telefon, Tara, Oras);
            }

            return client;
        }

        private Proiect CreateProiect(TextBox tDenumire,
            TextBox tDirector,
            TextBox tProducator,
            TextBox tCoordonator,
            TextBox tClient,
            ComboBox cStatus,
            NumericUpDown nPret,
            DateTimePicker dInceput,
            DateTimePicker dFinalizare,
            out string error)
        {
            Proiect proiect = null;
            error = "";

            string Denumire = tDenumire.Text.Trim().ToLower();
            string Status = cStatus.GetItemText(cStatus.SelectedItem).Trim().ToLower();

            bool ok = true;

            if (string.IsNullOrEmpty(Denumire))
            {
                ok = false;
                error += "Câmp obligatoriu: \"DENUMIRE\"!\r\n";
            }

            if (cStatus.SelectedIndex < 0)
            {
                ok = false;
                error += "Câmp obligatoriu: \"STATUS\"!\r\n";
            }

            int Director = 0;
            string temp = tDirector.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(temp))
            {
                if (!isValidAngajatID(temp, out Director) && !isValidAngajatJob(Director, GetJobIdByName(denumire_job_director)))
                {
                    ok = false;
                    error += "Invalid ID: \"DIRECTOR_ID\"!\r\n";
                }
            }

            int Producator = 0;
            temp = tProducator.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(temp))
            {
                if (!isValidAngajatID(temp, out Producator) && !isValidAngajatJob(Producator, GetJobIdByName(denumire_job_producator)))
                {
                    ok = false;
                    error += "Invalid ID: \"PRODUCATOR_ID\"!\r\n";
                }
            }

            int Coordonator = 0;
            temp = tCoordonator.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(temp))
            {
                if (!isValidAngajatID(temp, out Coordonator) && !isValidAngajatJob(Coordonator, GetJobIdByName(denumire_job_coordonator)))
                {
                    ok = false;
                    error += "Invalid ID: \"COORDONATOR_ID\"!\r\n";
                }
            }

            int Client = 0;
            temp = tClient.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(temp))
            {
                if (!isValidClientID(temp, out Client))
                {
                    ok = false;
                    error += "Invalid ID: \"CLIENT_ID\"!\r\n";
                }
            }

            if (ok)
            {
                proiect = new Proiect(Denumire, (float)nPret.Value,
                   Status, Director, Producator, Coordonator,
                   dInceput.Value.Date, dFinalizare.Value.Date, Client);
            }

            return proiect;
        }
        #endregion

        #region SET JOBS LIST COMBOBOX 
        private void SetJobList()
        {
            jobs = AdministrareJob.GetJobs();
        }
        private void tabControlAdd_Selected(object sender, TabControlEventArgs e)
        {
            SetJobList();
            comboBoxAddAngajatJob.Items.Clear();
            foreach (Job job in jobs)
            {
                comboBoxAddAngajatJob.Items.Add(job.Denumire);
            }
        }
        private int GetJobIdByName(string jobName)
        {
            int job_id = 0;
            foreach (Job job in jobs)
            {
                if (job.Denumire.CompareTo(jobName) == 0)
                {

                    job_id = job.Job_Id;
                    break;
                }
            }
            return job_id;
        }
        #endregion

        #region ADD JOB
        private void btnAddJob_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;
            Job job = CreateJob(txtBoxAddJobDenumire, txtBoxAddJobDescriere, out error);
            if (job != null)
            {
                if (AdministrareJob.AddJob(job))
                {
                    ClearLogError();
                    LogMessage(DateTime.Now + ": Job adăugat cu success");
                    ClearAddJobTab();
                    SetJobList();
                }
                else
                    LogError(DateTime.Now + ": Nu s-a putut adăuga. Eroare BazaDate!?");
            }
            else
                LogError(DateTime.Now + ": " + error);
        }

        private void ClearAddJobTab()
        {
            txtBoxAddJobDenumire.Text = string.Empty;
            txtBoxAddJobDescriere.Text = string.Empty;
        }
        #endregion

        #region ADD ANGAJAT
        private void btnAddAngajatView_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = AdministrareAngajat.GetAngajati();

            LogMessage(DateTime.Now + ": " + dataGridView1.RowCount + " Angajati.");
        }
        private void btnAddAngajat_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;

            Angajat angajat = CreateAngajat(txtBoxAddAngajatNume, txtBoxAddAngajatPrenume,
                txtBoxAddAngajatTelefon, txtBoxAddAngajatEmail, txtBoxAddAngajatTara,
                txtBoxAddAngajatOras, txtBoxAddAngajatManager, comboBoxAddAngajatJob,
                numericUpDownAddAngajat, dateTimePickerAddAngajat, out error);

            if (string.IsNullOrEmpty(error))
            {
                if (AdministrareAngajat.AddAngajat(angajat))
                {
                    LogMessage(DateTime.Now + ": Angajat adăugat cu success.");
                    ClearAddAngajatTab();
                }
                else
                    error += DateTime.Now + ": Nu s-a putut adăuga. Eroare BazaDate!?.\r\n";
            }

            if (!string.IsNullOrEmpty(error))
                LogError(error);
        }
        private void ClearAddAngajatTab()
        {
            txtBoxAddAngajatNume.Text = string.Empty;
            txtBoxAddAngajatPrenume.Text = string.Empty;
            txtBoxAddAngajatEmail.Text = string.Empty;
            txtBoxAddAngajatTelefon.Text = string.Empty;
            txtBoxAddAngajatTara.Text = string.Empty;
            txtBoxAddAngajatOras.Text = string.Empty;
            txtBoxAddAngajatManager.Text = string.Empty;
            numericUpDownAddAngajat.Value = (decimal)0;
            comboBoxAddAngajatJob.SelectedIndex = -1;
            dateTimePickerAddAngajat.Value = DateTime.Now.Date;
        }
        #endregion

        #region ADD CLIENT
        private void btnAddClient_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;
            Client client = CreateClient(txtBoxAddClientNume, txtBoxAddClientEmail, txtBoxAddClientTelefon, txtBoxAddClientTara, txtBoxAddClientOras, out error);
            if (client!=null)
            {
                if (AdministrareClient.AddClient(client))
                {
                    LogMessage(DateTime.Now + ": Client adăugat cu success.");
                    ClearAddClientTab();
                }
                else
                    error = "Nu s-a putut adăuga. Eroare BazaDate!?\r\n";
            }
            if (!string.IsNullOrEmpty(error))
                LogError(DateTime.Now + ": " + error);
        }
        private void ClearAddClientTab()
        {
            txtBoxAddClientNume.Text = string.Empty;
            txtBoxAddClientEmail.Text = string.Empty;
            txtBoxAddClientTelefon.Text = string.Empty;
            txtBoxAddClientTara.Text = string.Empty;
            txtBoxAddClientOras.Text = string.Empty;
        }
        #endregion

        #region ADD PROIECT

        private void btnAddProiectViewDirector_Click(object sender, EventArgs e)
        {
            int job_id = GetJobIdByName(denumire_job_director); 
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = AdministrareAngajat.GetAngajati("Job_Id = " + job_id.ToString());

            LogMessage(DateTime.Now + ": " + dataGridView1.RowCount + " Directori.");
        }

        private void btnAddProiectViewProducator_Click(object sender, EventArgs e)
        {
            int job_id = GetJobIdByName(denumire_job_producator);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = AdministrareAngajat.GetAngajati("Job_Id = " + job_id.ToString());

            LogMessage(DateTime.Now + ": " + dataGridView1.RowCount + " Producatori.");
        }

        private void btnAddProiectViewCoordonator_Click(object sender, EventArgs e)
        {
            int job_id = GetJobIdByName(denumire_job_coordonator);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = AdministrareAngajat.GetAngajati("Job_Id = " + job_id.ToString());

            LogMessage(DateTime.Now + ": " + dataGridView1.RowCount + " Coordonatori.");
        }

        private void btnAddProiectViewClient_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = AdministrareClient.GetClienti();

            LogMessage(DateTime.Now + ": " + dataGridView1.RowCount + " Clienti.");
        }

        private void btnAddProiect_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;
            Proiect proiect = CreateProiect(txtBoxAddProiectDenumire, txtBoxAddProiectDirector,
                txtBoxAddProiectProducator, txtBoxAddProiectCoordonator, txtBoxAddProiectClient,
                comboBoxAddProiecteStatus, numericUpDownAddProiectPret,
                dateTimePickerAddProiectInceput, dateTimePickerAddProiectFinalizare, out error);

            if (AdministrareProiect.AddProiect(proiect))
            {
                LogMessage(DateTime.Now + ": Proiect adăugat cu success.");
                ClearAddProiectTab();
            }
            else
                error += "Nu s-a putut adăuga. Eroare BazaDate!?\r\n";

            if (!string.IsNullOrEmpty(error))
                LogError(DateTime.Now + ": " + error);
        }

        private void ClearAddProiectTab()
        {
            txtBoxAddProiectDenumire.Text = string.Empty;
            txtBoxAddProiectClient.Text = string.Empty;
            txtBoxAddProiectDirector.Text = string.Empty;
            txtBoxAddProiectProducator.Text = string.Empty;
            txtBoxAddProiectCoordonator.Text = string.Empty;
            numericUpDownAddProiectPret.Value = (decimal)0;
            comboBoxAddProiecteStatus.SelectedIndex = -1;
            dateTimePickerAddProiectInceput.Value = DateTime.Now.Date;
            dateTimePickerAddProiectFinalizare.Value = DateTime.Now.Date;
        }

        #endregion

        #endregion

        #region VIEWING METHODS

        #region events

        private void ckbListViewColumns_Leave(object sender, EventArgs e)
        {
            (sender as CheckedListBox).ClearSelected();
        }

        #endregion

        #region View JOBS

        private void ckbViewJobsFilter_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                if (cb.Name.ToLower().Contains("id"))
                {
                    txtBoxViewJobsFilterId.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewJobsFilterId.Clear();
                }
                if (cb.Name.ToLower().Contains("denumire"))
                {
                    txtBoxViewJobsFilterDenumire.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewJobsFilterDenumire.Clear();
                }
                if (cb.Name.ToLower().Contains("descriere"))
                {
                    txtBoxViewJobsFilterDescriere.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewJobsFilterDescriere.Clear();
                }
            }
        }

        private void btnViewJobs_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string columns = "job_id, ";
            string conditions = "";
            for (int i = 0; i < ckbListViewJobsColumns.Items.Count; i++)
            {
                if (!ckbListViewJobsColumns.GetItemChecked(i))
                {
                    columns += ckbListViewJobsColumns.Items[i].ToString() + ", ";
                }
            }
            if (columns.EndsWith(", "))
                columns = columns.Remove(columns.Length - 2);
            string errors = "";

            if (ckbViewJobsFilterId.Checked)
            {
                var id = txtBoxViewJobsFilterId.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    errors += "Filter by ID is checked. No input inserted!\r\n";
                }
                else
                {
                    if (isNumber(id))
                    {
                        conditions += "job_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid ID!\r\n";
                    }
                }
            }

            if (ckbViewJobsFilterDenumire.Checked)
            {
                var denumire = txtBoxViewJobsFilterDenumire.Text.Trim().ToLower();
                if (string.IsNullOrEmpty(denumire))
                {
                    errors += "Filter by DENUMIRE is checked. No input inserted!\r\n";
                }
                else
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "denumire like '%" + denumire + "%'";
                }
            }
            if (ckbViewJobsFilterDescriere.Checked)
            {
                var descriere = txtBoxViewJobsFilterDescriere.Text.Trim();
                if (string.IsNullOrEmpty(descriere))
                {
                    errors += "Filter by DESCRIERE is checked. No input inserted!\r\n";
                }
                else
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "descriere like '%" + descriere + "%'";
                }
            }

            if (string.IsNullOrEmpty(errors))
            {
                FillDataGridView(SqlDBHelper.GetDataTable(AdministrareJob.TABLE, columns, conditions, out string temp));
                if (!string.IsNullOrEmpty(temp))
                {
                    LogError(DateTime.Now + ": " + temp);
                }
            }
            else
            {
                LogError(DateTime.Now + ": " + errors);
            }
        }
        #endregion

        #region View ANGAJATI
        //gboxViewAngajatColumns

        private void ckbViewAngajatiFilter_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                if (cb.Name.ToLower().Contains("id"))
                {
                    txtBoxViewAngajatFilterId.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewAngajatFilterId.Clear();
                }
                if (cb.Name.ToLower().Contains("nume"))
                {
                    txtBoxViewAngajatFilterNume.Enabled = cb.Checked;
                    txtBoxViewAngajatFilterPrenume.Enabled = cb.Checked;
                    if (!cb.Checked)
                    {
                        txtBoxViewAngajatFilterNume.Clear();
                        txtBoxViewAngajatFilterPrenume.Clear();
                    }
                }
                if (cb.Name.ToLower().Contains("job"))
                {
                    comboBoxViewAngajatFilterJob.Enabled = cb.Checked;
                    if (!cb.Checked)
                        comboBoxViewAngajatFilterJob.SelectedIndex = -1;
                    else
                    {
                        SetJobList();
                        comboBoxViewAngajatFilterJob.Items.Clear();
                        foreach (Job job in jobs)
                        {
                            comboBoxViewAngajatFilterJob.Items.Add(job.Denumire);
                        }
                    }
                }
                if (cb.Name.ToLower().Contains("salariu"))
                {
                    numericUpDownViewAngajatFilterSalariuMin.Enabled = cb.Checked;
                    numericUpDownViewAngajatFilterSalariuMax.Enabled = cb.Checked;

                    if (!cb.Checked)
                    {
                        numericUpDownViewAngajatFilterSalariuMin.Value = 0;
                        numericUpDownViewAngajatFilterSalariuMax.Value = 0;
                    }
                }
                if (cb.Name.ToLower().Contains("manager"))
                {
                    txtBoxViewAngajatFilterManager.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewAngajatFilterManager.Clear();
                }
            }
        }

        private void numericUpDownViewAngajatiFilterSalariu_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownViewAngajatFilterSalariuMin.Value > numericUpDownViewAngajatFilterSalariuMax.Value)
            {
                numericUpDownViewAngajatFilterSalariuMax.Value = numericUpDownViewAngajatFilterSalariuMin.Value;
            }
        }

        private void btnViewAngajati_Click(object sender, EventArgs e)
        {

            ClearLogError();
            string columns = "angajat_id, ";
            string conditions = "";
            for (int i = 0; i < ckbListViewAngajatColumns.Items.Count; i++)
            {
                if (!ckbListViewAngajatColumns.GetItemChecked(i))
                {
                    columns += ckbListViewAngajatColumns.Items[i].ToString() + ", ";
                }
            }
            if (columns.EndsWith(", "))
                columns = columns.Remove(columns.Length - 2);
            string errors = "";

            if (ckbViewAngajatFilterId.Checked)
            {
                var id = txtBoxViewAngajatFilterId.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    errors += "Filter by ID is checked. No input inserted!\r\n";
                }
                else
                {
                    if (isNumber(id))
                    {
                        conditions += "angajat_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid ID!\r\n";
                    }
                }
            }

            if (ckbViewAngajatFilterNumePrenume.Checked)
            {
                var nume = txtBoxViewAngajatFilterNume.Text.Trim().ToLower();
                var prenume = txtBoxViewAngajatFilterPrenume.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(nume))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "nume like '%" + nume + "%'";
                }
                if (!string.IsNullOrEmpty(prenume))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "prenume like '%" + prenume + "%'";
                }

            }

            if (ckbViewAngajatFilterJob.Checked)
            {
                if (comboBoxViewAngajatFilterJob.SelectedIndex < 0)
                {
                    errors += "Filter by JOB is checked. No value selected!\r\n";
                }
                else
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "job_id = '" + GetJobIdByName(comboBoxViewAngajatFilterJob.GetItemText(comboBoxViewAngajatFilterJob.SelectedItem).Trim().ToLower()) + "'";
                }
            }

            if (ckbViewAngajatFilterSalariu.Checked)
            {
                if (conditions.Length > 0)
                    conditions += " and ";
                conditions += "salariu between " + numericUpDownViewAngajatFilterSalariuMin.Value + " and " + numericUpDownViewAngajatFilterSalariuMax.Value;
            }

            if (ckbViewAngajatFilterManager.Checked)
            {
                var id = txtBoxViewAngajatFilterManager.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "manager_id is null";
                }
                else
                {
                    if (isNumber(id))
                    {
                        if (conditions.Length > 0)
                            conditions += " and ";
                        conditions += "manager_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid ID!\r\n";
                    }
                }
            }

            if (string.IsNullOrEmpty(errors))
            {
                FillDataGridView(SqlDBHelper.GetDataTable(AdministrareAngajat.TABLE, columns, conditions, out string temp));
                if (!string.IsNullOrEmpty(temp))
                {
                    LogError(DateTime.Now + ": " + temp);
                }
            }
            else
            {
                LogError(DateTime.Now + ": " + errors);
            }
        }
        #endregion

        #region View CLIENTI

        private void ckbViewClientFilter_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                if (cb.Name.ToLower().Contains("id"))
                {
                    txtBoxViewClientFilterId.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewClientFilterId.Clear();
                }
                if (cb.Name.ToLower().Contains("nume"))
                {
                    txtBoxViewClientFilterNume.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewClientFilterNume.Clear();
                }
                if (cb.Name.ToLower().Contains("email"))
                {
                    txtBoxViewClientFilterEmail.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewClientFilterEmail.Clear();
                }
                if (cb.Name.ToLower().Contains("telefon"))
                {
                    txtBoxViewClientFilterTelefon.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewClientFilterTelefon.Clear();
                }
            }
        }

        private void btnViewClienti_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string errors = "";
            string columns = "client_id, ";
            string conditions = "";

            for (int i = 0; i < ckbListViewClientColumns.Items.Count; i++)
            {
                if (!ckbListViewClientColumns.GetItemChecked(i))
                {
                    columns += ckbListViewClientColumns.Items[i].ToString().ToLower() + ", ";
                }
            }
            if (columns.EndsWith(", "))
                columns = columns.Remove(columns.Length - 2);

            if (ckbViewClientFilterId.Checked)
            {
                var id = txtBoxViewClientFilterId.Text.Trim().ToLower();
                if (string.IsNullOrEmpty(id))
                {
                    errors += "Filter by ID is checked. No input inserted!\r\n";
                }
                else
                {
                    if (isNumber(id))
                    {
                        conditions += "client_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid ID!\r\n";
                    }
                }
            }
            if (ckbViewClientFilterNume.Checked)
            {
                var nume = txtBoxViewClientFilterNume.Text.Trim().ToLower();
                if (string.IsNullOrEmpty(nume))
                {
                    errors += "Filter by NUME is checked. No input inserted!\r\n";
                }
                else
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "nume like '%" + nume + "%'";
                }
            }
            if (ckbViewClientFilterEmail.Checked)
            {
                var email = txtBoxViewClientFilterEmail.Text.Trim().ToLower();
                if (isEmailOk(email, out string tmp))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "email like '%" + email + "%'";
                }
                else
                {
                    errors += tmp + "\r\n";
                }
            }

            if (ckbViewClientFilterTelefon.Checked)
            {
                var telefon = txtBoxViewClientFilterTelefon.Text.Trim().ToLower();
                if (isTelefonOk(telefon, out string tmp))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "telefon like '%" + telefon + "%'";
                }
                else
                {
                    errors += tmp;
                }
            }

            if (string.IsNullOrEmpty(errors))
            {
                FillDataGridView(SqlDBHelper.GetDataTable(AdministrareClient.TABLE, columns, conditions, out string temp));

                if (!string.IsNullOrEmpty(temp))
                {
                    LogError(DateTime.Now + ": " + temp);
                }
            }
            else
            {
                LogError(DateTime.Now + ": " + errors);
            }
        }
        #endregion

        #region View PROIECTE

        private void ckbViewProiecteFilter_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                if (cb.Name.ToLower().Contains("id"))
                {
                    txtBoxViewProiectFilterId.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewProiectFilterId.Clear();
                }
                if (cb.Name.ToLower().Contains("denumire"))
                {
                    txtBoxViewProiectFilterDenumire.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewProiectFilterDenumire.Clear();
                }
                if (cb.Name.ToLower().Contains("director"))
                {
                    txtBoxViewProiectFilterDirector.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewProiectFilterDirector.Clear();
                }
                if (cb.Name.ToLower().Contains("producator"))
                {
                    txtBoxViewProiectFilterProducator.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewProiectFilterProducator.Clear();
                }
                if (cb.Name.ToLower().Contains("coordonator"))
                {
                    txtBoxViewProiectFilterCoordonator.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewProiectFilterCoordonator.Clear();
                }
                if (cb.Name.ToLower().Contains("client"))
                {
                    txtBoxViewProiectFilterClient.Enabled = cb.Checked;
                    if (!cb.Checked)
                        txtBoxViewProiectFilterClient.Clear();
                }
                if (cb.Name.ToLower().Contains("status"))
                {
                    comboBoxViewProiectFilterStatus.Enabled = cb.Checked;
                    if (!cb.Checked)
                        comboBoxViewProiectFilterStatus.SelectedIndex = -1;
                }
                if (cb.Name.ToLower().Contains("pret"))
                {
                    numericUpDownViewProiectFilterPretMin.Enabled = cb.Checked;
                    numericUpDownViewProiectFilterPretMax.Enabled = cb.Checked;

                    if (!cb.Checked)
                    {
                        numericUpDownViewProiectFilterPretMin.Value = 0;
                        numericUpDownViewProiectFilterPretMax.Value = 0;
                    }
                }
            }
        }

        private void numericUpDownViewProiecteFilterPret_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownViewProiectFilterPretMin.Value > numericUpDownViewProiectFilterPretMax.Value)
            {
                numericUpDownViewProiectFilterPretMax.Value = numericUpDownViewProiectFilterPretMin.Value;
            }
        }

        private void btnViewProiecte_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string columns = "proiect_id, ";
            string conditions = "";
            for (int i = 0; i < ckbListViewProiectColumns.Items.Count; i++)
            {
                if (!ckbListViewProiectColumns.GetItemChecked(i))
                {
                    columns += ckbListViewProiectColumns.Items[i].ToString() + ", ";
                }
            }
            if (columns.EndsWith(", "))
                columns = columns.Remove(columns.Length - 2);
            string errors = "";

            if (ckbViewProiectFilterId.Checked)
            {
                var id = txtBoxViewProiectFilterId.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    errors += "Filter by ID is checked. No input inserted!\r\n";
                }
                else
                {
                    if (isNumber(id))
                    {
                        conditions += "proiect_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid ID!\r\n";
                    }
                }
            }

            if (ckbViewProiectFilterDenumire.Checked)
            {
                var denumire = txtBoxViewProiectFilterDenumire.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(denumire))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "denumire like '%" + denumire + "%'";
                }
            }

            if (ckbViewProiectFilterStatus.Checked)
            {
                if (comboBoxViewProiectFilterStatus.SelectedIndex < 0)
                {
                    errors += "Filter by STATUS is checked. No value selected!\r\n";
                }
                else
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "status = '" + comboBoxViewProiectFilterStatus.GetItemText(comboBoxViewProiectFilterStatus.SelectedItem).Trim().ToLower() + "'";
                }
            }

            if (ckbViewProiectFilterPret.Checked)
            {
                if (conditions.Length > 0)
                    conditions += " and ";
                conditions += "pret between " + numericUpDownViewProiectFilterPretMin.Value + " and " + numericUpDownViewProiectFilterPretMax.Value;
            }

            if (ckbViewProiectFilterDirector.Checked)
            {
                var id = txtBoxViewProiectFilterDirector.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "director_id is null";
                }
                else
                {
                    if (isNumber(id))
                    {
                        if (conditions.Length > 0)
                            conditions += " and ";
                        conditions += "director_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid DIRECTOR ID!\r\n";
                    }
                }
            }

            if (ckbViewProiectFilterProducator.Checked)
            {
                var id = txtBoxViewProiectFilterProducator.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "producator_id is null";
                }
                else
                {
                    if (isNumber(id))
                    {
                        if (conditions.Length > 0)
                            conditions += " and ";
                        conditions += "producator_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid PRODUCATOR ID!\r\n";
                    }
                }
            }

            if (ckbViewProiectFilterCoordonator.Checked)
            {
                var id = txtBoxViewProiectFilterCoordonator.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "coordonator_id is null";
                }
                else
                {
                    if (isNumber(id))
                    {
                        if (conditions.Length > 0)
                            conditions += " and ";
                        conditions += "coordonator_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid COORDONATOR ID\r\n!";
                    }
                }
            }

            if (ckbViewProiectFilterClient.Checked)
            {
                var id = txtBoxViewProiectFilterClient.Text.Trim();
                if (string.IsNullOrEmpty(id))
                {
                    if (conditions.Length > 0)
                        conditions += " and ";
                    conditions += "client_id is null";
                }
                else
                {
                    if (isNumber(id))
                    {
                        if (conditions.Length > 0)
                            conditions += " and ";
                        conditions += "client_id = " + id;
                    }
                    else
                    {
                        errors += "Invalid CLIENT ID!\r\n";
                    }
                }
            }
            if (string.IsNullOrEmpty(errors))
            {
                FillDataGridView(SqlDBHelper.GetDataTable(AdministrareProiect.TABLE, columns, conditions, out string temp));
                if (!string.IsNullOrEmpty(temp))
                {
                    LogError(DateTime.Now + ": " + temp);
                }
            }
            else
            {
                LogError(DateTime.Now + ": " + errors);
            }
        }

        #endregion

        #endregion

        #region MODIFY/DELETE METHODS

        Job selectedJob = null;
        Angajat selectedAngajat = null;
        Client selectedClient = null;
        Proiect selectedProiect = null;

        #region INTERFACE
        private void btnModificaVeziJobs_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error = "";
            string id = txtBoxModificaJobsId.Text.Trim().ToLower();
            if (id.Length > 0 && isNumber(id))
            {
                Job job = AdministrareJob.GetJob(int.Parse(id));
                if ( job != null)
                {
                    FillModificaJob(job);
                }
                else
                {
                    error += "ID inexistent.\r\n";
                }
            }
            else
            {
                error += "ID invalid.\r\n";
            }
            if (!string.IsNullOrEmpty(error))
            {
                FillModificaJob(null);
                LogError(DateTime.Now + ": " + error);
            }
        }
        private void FillModificaJob(Job job)
        {
            selectedJob = job;
            if (job != null)
            {
                btnModificaJobSterge.Enabled = true;
                panelModificaJobs.Enabled = true;
            }
            else
            {
                btnModificaJobSterge.Enabled = false;
                panelModificaJobs.Enabled = false;
            }
            txtBoxModificaJobsDenumire.Text = job == null ? string.Empty : job.Denumire;
            txtBoxModificaJobsDescriere.Text = job == null ? string.Empty : job.Descriere;
        }
        private void btnModificaVeziAngajati_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error = "";
            string id = txtBoxModificaAngajatiId.Text.Trim().ToLower();
            if (id.Length > 0 && isNumber(id))
            {
                Angajat angajat = AdministrareAngajat.GetAngajat(int.Parse(id));
                if (angajat != null)
                {
                    FillModificaAngajat(angajat);
                }
                else
                {
                    error += "ID inexistent.\r\n";
                }
            }
            else
            {
                error += "ID invalid.\r\n";
            }
            if (!string.IsNullOrEmpty(error))
            {
                FillModificaAngajat(null);
                LogError(DateTime.Now + ": " + error);
            }
        }
        private void FillModificaAngajat(Angajat angajat)
        {
            selectedAngajat = angajat;
            int index = -1;
            if (angajat != null)
            {
                btnModificaAngajatiSterge.Enabled = true;
                panelModificaAngajat.Enabled = true;
                SetJobList();
                comboBoxModificaAngajatiJob.Items.Clear();
                foreach (Job job in jobs)
                {
                    comboBoxModificaAngajatiJob.Items.Add(job.Denumire);
                }
                index = comboBoxModificaAngajatiJob.Items.IndexOf(AdministrareJob.GetJob(angajat.Job_Id).Denumire);
            }
            else
            {
                btnModificaAngajatiSterge.Enabled = false;
                panelModificaAngajat.Enabled = false;
            }
            txtBoxModificaAngajatiNume.Text = angajat == null ? string.Empty : angajat.Nume;
            txtBoxModificaAngajatiPrenume.Text = angajat == null ? string.Empty : angajat.Prenume;
            txtBoxModificaAngajatiEmail.Text = angajat == null ? string.Empty : angajat.Email;
            txtBoxModificaAngajatiTelefon.Text = angajat == null ? string.Empty : angajat.Telefon;
            txtBoxModificaAngajatiTara.Text = angajat == null ? string.Empty : angajat.Tara;
            txtBoxModificaAngajatiOras.Text = angajat == null ? string.Empty : angajat.Oras;
            txtBoxModificaAngajatManager.Text = angajat == null || angajat?.Manager_Id == 0 ? string.Empty : angajat.Manager_Id.ToString();
            dateTimePickerModificaAngajatDataAngajare.Value = angajat == null ? DateTime.Now.Date : angajat.Data_Angajare;
            numericUpDownModificaAngajatiSalariu.Value = angajat == null ? 0 : (decimal)angajat.Salariu;
            comboBoxModificaAngajatiJob.SelectedIndex = index;
        }
        private void btnModificaVeziClienti_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error = "";
            string id = txtBoxModificaClientiId.Text.Trim().ToLower();
            if (id.Length > 0 && isNumber(id))
            {
                Client client = AdministrareClient.GetClient(int.Parse(id));
                if (client != null)
                {
                    FillModificaClient(client);
                }
                else
                {
                    error += "ID inexistent.\r\n";
                }
            }
            else
            {
                error += "ID invalid.\r\n";
            }
            if (!string.IsNullOrEmpty(error))
            {
                FillModificaClient(null);
                LogError(DateTime.Now + ": " + error);
            }
        }
        private void FillModificaClient(Client client)
        {
            selectedClient = client;
            if (client != null)
            {
                btnModificaClientiSterge.Enabled = true;
                panelModificaClienti.Enabled = true;
            }
            else
            {
                btnModificaClientiSterge.Enabled = false;
                panelModificaClienti.Enabled = false;
            }
            txtBoxModificaClientiNume.Text = client == null ? string.Empty : client.Nume;
            txtBoxModificaClientiEmail.Text = client == null ? string.Empty : client.Email;
            txtBoxModificaClientiTelefon.Text = client == null ? string.Empty : client.Telefon;
            txtBoxModificaClientiTara.Text = client == null ? string.Empty : client.Tara;
            txtBoxModificaClientiOras.Text = client == null ? string.Empty : client.Oras;
        }
        private void btnModificaVeziProiecte_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error = "";
            string id = txtBoxModificaProiecteId.Text.Trim().ToLower();
            if (id.Length > 0 && isNumber(id))
            {
                Proiect proiect = AdministrareProiect.GetProiect(int.Parse(id));
                if (proiect != null)
                {
                    FillModificaProiect(proiect);
                }
                else
                {
                    error += "ID inexistent.\r\n";
                }
            }
            else
            {
                error += "ID invalid.\r\n";
            }
            if (!string.IsNullOrEmpty(error))
            {
                FillModificaClient(null);
                LogError(DateTime.Now + ": " + error);
            }
        }
        private void FillModificaProiect(Proiect proiect)
        {
            selectedProiect = proiect;
            int index = -1;
            if (proiect != null)
            {
                btnModificaProiecteSterge.Enabled = true;
                panelModificaProiecte.Enabled = true;
                index = comboBoxModificaProiecteStatus.Items.IndexOf(proiect.Status);
            }
            else
            {
                btnModificaProiecteSterge.Enabled = false;
                panelModificaProiecte.Enabled = false;
            }
            txtBoxModificaProiecteDenumire.Text = proiect == null ? string.Empty : proiect.Denumire;
            txtBoxModificaProiecteDirector.Text = proiect == null || proiect?.Director_Id == 0 ? string.Empty : proiect.Director_Id.ToString();
            txtBoxModificaProiecteProducator.Text = proiect == null || proiect?.Producator_Id == 0 ? string.Empty : proiect.Producator_Id.ToString();
            txtBoxModificaProiecteCoordonator.Text = proiect == null || proiect?.Coordonator_Id == 0? string.Empty : proiect.Coordonator_Id.ToString();
            txtBoxModificaProiecteClient.Text = proiect == null || proiect?.Client_Id == 0? string.Empty : proiect.Client_Id.ToString();
            numericUpDownModificaProiectePret.Value = proiect == null ? 0 : (decimal)proiect.Pret;
            dateTimePickerModificaProiecteTermenInceput.Value = proiect == null ? DateTime.Now.Date : proiect.Termen_Inceput;
            dateTimePickerModificaProiecteTermenFinalizare.Value = proiect == null ? DateTime.Now.Date : proiect.Termen_Finalizare;
            comboBoxModificaProiecteStatus.SelectedIndex = index;
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = ((TabControl)sender).SelectedIndex;

            if (index != 0)
            {
                txtBoxModificaJobsId.Text = string.Empty;
                FillModificaJob(null);
            }
            if (index != 1)
            {
                txtBoxModificaAngajatiId.Text = string.Empty;
                FillModificaAngajat(null);
            }
            if (index != 2)
            {
                txtBoxModificaAngajatiId.Text = string.Empty;
                FillModificaAngajat(null);
            }
            if (index != 3)
            {
                txtBoxModificaClientiId.Text = string.Empty;
                FillModificaClient(null);
            }
            if (index != 4)
            {
                txtBoxModificaProiecteId.Text = string.Empty;
                FillModificaProiect(null);
            }
        }
        #endregion

        #region DELETE

        private void btnModificaJobSterge_Click(object sender, EventArgs e)
        {
            if (selectedJob != null && AdministrareJob.DeleteJob(selectedJob))
            {
                txtBoxModificaJobsId.Text = string.Empty;
                FillModificaJob(null);
                LogMessage(DateTime.Now+": Șters cu success!");
            }
        }
        private void btnModificaAngajatiSterge_Click(object sender, EventArgs e)
        {
            if (selectedAngajat != null && AdministrareAngajat.DeleteAngajat(selectedAngajat))
            {
                FillModificaAngajat(null);
                txtBoxModificaAngajatiId.Text = string.Empty;
                LogMessage(DateTime.Now + ": Șters cu success!");
            }
        }
        private void btnModificaClientiSterge_Click(object sender, EventArgs e)
        {
            if (selectedClient != null && AdministrareClient.DeleteClient(selectedClient))
            {
                FillModificaClient(null);
                txtBoxModificaClientiId.Text = string.Empty;
                LogMessage(DateTime.Now + ": Șters cu success!");
            }
        }
        private void btnModificaProiecteSterge_Click(object sender, EventArgs e)
        {
            if(selectedProiect != null && AdministrareProiect.DeleteProiect(selectedProiect))
            {
                FillModificaProiect(null);
                txtBoxModificaProiecteId.Text = string.Empty;
                LogMessage(DateTime.Now + ": Șters cu success!");
            }
        }

        #endregion

        #region UPDATE
        private void btnModificaJobs_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;
            Job job = CreateJob(txtBoxModificaJobsDenumire, txtBoxModificaJobsDescriere, out error);
            if (job != null)
            {
                job.Job_Id = selectedJob.Job_Id;
                if (AdministrareJob.UpdateJob(job))
                {
                    LogMessage(DateTime.Now + ": Modificat cu success.\r\n");
                    txtBoxModificaJobsId.Text = string.Empty;
                    FillModificaJob(null);
                }
                else
                    error = "Nu s-a putut modifica JOB.";
            }
            if (!string.IsNullOrEmpty(error))
            {
                LogError(DateTime.Now + ": " + error);
            }
        }

        private void btnModificaAngajati_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;
            Angajat angajat = CreateAngajat(txtBoxModificaAngajatiNume, txtBoxModificaAngajatiPrenume,
                txtBoxModificaAngajatiTelefon, txtBoxModificaAngajatiEmail, txtBoxModificaAngajatiTara,
                txtBoxModificaAngajatiOras, txtBoxModificaAngajatManager, comboBoxModificaAngajatiJob,
                numericUpDownModificaAngajatiSalariu, dateTimePickerModificaAngajatDataAngajare, out error);
            if (angajat != null)
            {
                angajat.Angajat_Id = selectedAngajat.Angajat_Id;

                if (AdministrareAngajat.UpdateAngajat(angajat))
                {
                    LogMessage(DateTime.Now + ": Modificat cu success.\r\n");
                    txtBoxModificaAngajatiId.Text = string.Empty;
                    FillModificaAngajat(null);
                }
                else
                    error = "Nu s-a putut modifica ANGAJAT.";
            }
            if (!string.IsNullOrEmpty(error))
            {
                LogError(DateTime.Now + ": " + error);
            }
        }

        private void btnModificaClienti_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;
            Client client = CreateClient(txtBoxModificaClientiNume, txtBoxModificaClientiEmail,
                txtBoxModificaClientiTelefon, txtBoxModificaClientiTara, txtBoxModificaClientiOras, out error);
            if (client != null)
            {
                client.Client_Id = selectedClient.Client_Id;
                if (AdministrareClient.UpdateClient(client))
                {
                    LogMessage(DateTime.Now + ": Modificat cu success.\r\n");
                    txtBoxModificaClientiId.Text = string.Empty;
                    FillModificaClient(null);
                }
                else
                    error = "Nu s-a putut modifica CLIENT.";
            }
            if (!string.IsNullOrEmpty(error))
            {
                LogError(DateTime.Now + ": " + error);
            }
        }

        private void btnModificaProiecte_Click(object sender, EventArgs e)
        {
            ClearLogError();
            string error;
            Proiect proiect = CreateProiect(txtBoxModificaProiecteDenumire, txtBoxModificaProiecteDirector,
                txtBoxModificaProiecteProducator, txtBoxModificaProiecteCoordonator,
                txtBoxModificaProiecteClient, comboBoxModificaProiecteStatus, numericUpDownModificaProiectePret,
                dateTimePickerModificaProiecteTermenInceput, dateTimePickerModificaProiecteTermenFinalizare, out error);
            if (proiect != null)
            {
                proiect.Proiect_Id = selectedProiect.Proiect_Id;
                if (AdministrareProiect.UpdateProiect(proiect))
                {
                    LogMessage(DateTime.Now + ": Modificat cu success.\r\n");
                    txtBoxModificaProiecteId.Text = string.Empty;
                    FillModificaProiect(null);
                }
                else
                    error = "Nu s-a putut modifica PROIECT.";
            }
            if (!string.IsNullOrEmpty(error))
            {
                LogError(DateTime.Now + ": " + error);
            }
        }
        #endregion

        #endregion
    }
}
