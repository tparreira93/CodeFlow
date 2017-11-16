using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CodeFlow
{
    public partial class CreateInGenioForm : Form
    {
        private List<ManuaCode> manualCode;
        private Dictionary<string, Guid> features;
        private Dictionary<string, Guid> modules;
        private Dictionary<string, string> tipos;

        public CreateInGenioForm()
        {
            InitializeComponent();
            manualCode = new List<ManuaCode>();
            features = new Dictionary<string, Guid>();
            modules = new Dictionary<string, Guid>();
            tipos = new Dictionary<string, string>();
        }
        public CreateInGenioForm(List<ManuaCode> manual)
        {
            InitializeComponent();
            manualCode = manual;
            features = new Dictionary<string, Guid>();
            modules = new Dictionary<string, Guid>();
            tipos = new Dictionary<string, string>();
        }

        private void LoadFormData()
        {
            rtCode.Text = manualCode[0].Code;

            cmbPlataform.Items.Clear();
            cmbType.Items.Clear();
            tipos.Clear();

            tipos = PackageOperations.ActiveProfile.GenioConfiguration.Tipos;
            cmbPlataform.Items.AddRange(PackageOperations.ActiveProfile.GenioConfiguration.Plataforms.ToArray());
        }

        private void LoadDBInfo()
        {
            modules = PackageOperations.ActiveProfile.GenioConfiguration.GetModules();
            features = PackageOperations.ActiveProfile.GenioConfiguration.GetFeatures();
            
            foreach (KeyValuePair<string, Guid> pair in modules)
                cmbModule.Items.Add(pair.Key);

            foreach (KeyValuePair<string, Guid> pair in features)
                cmbFeature.Items.Add(pair.Key);
        }

        private void CreateInGenioForm_Load(object sender, EventArgs e)
        {
            LoadFormData();
            LoadDBInfo();
            RefreshForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshForm()
        {
            cmbModule.Items.Clear();
            cmbFeature.Items.Clear();
            modules.Clear();
            features.Clear();

            btnCreate.Enabled = true;
            tssServer.Text = PackageOperations.ActiveProfile.ToString();
            tssServer.ForeColor= Color.Green;
        }

        private void btnSetCon_Click(object sender, EventArgs e)
        {
            ProfilesForm profilesForm = new ProfilesForm();
            profilesForm.ShowDialog();
            LoadDBInfo();
            RefreshForm();
        }

        private void chkSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSystem.Checked)
                cmbModule.Enabled = false;
            else
                cmbModule.Enabled = true;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            ManuaCode man = new ManuaCode(rtCode.Text);

            string feature = (string)cmbFeature.SelectedItem;
            string module = (string)cmbModule.SelectedItem;
            string plataform = (string)cmbPlataform.SelectedItem;
            string type = (string)cmbType.SelectedItem;
            string param = txtParam.Text;
            string file = txtFile.Text;
            string order = txtOrder.Text;
            bool inhib = chkInhibt.Checked;
            bool system = chkSystem.Checked;
            string lang = "";

            double f_order = .0f;
            Double.TryParse(order, out f_order);

            Guid codfeature = Guid.Empty;
            Guid codmodul = Guid.Empty;

            if (feature != null)
                features.TryGetValue(feature, out codfeature);

            if (module != null)
                modules.TryGetValue(feature, out codmodul);

            if (type != null)
                tipos.TryGetValue(type, out lang);

            man.Codfeature = codfeature;
            man.Feature = feature ?? "";
            man.Codmodul = codmodul;
            man.Modulo = module != null ? (system ? "GIP" : module ) : "";
            man.Plataform = plataform ?? "";
            man.Tag = type ?? "";
            man.Parameter = param;
            man.ManualFile = file;
            man.Order = (float)Math.Round(f_order, 1);
            man.Inhib = inhib ? 1 : 0;
            man.System = system ? 1 : 0;
            man.Lang = lang;

            try
            {
                if (man.Insert(PackageOperations.ActiveProfile))
                    this.Close();
                //TODO: REPLACE MANUAL CODE WITH THIS TOSTRING
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorCreate, ex.Message), 
                    Properties.Resources.CreateInGenio, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }

        private void CreateInGenioForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
