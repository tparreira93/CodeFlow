using CodeFlow.CodeControl;
using CodeFlow.Forms;
using CodeFlow.GenioOperations;
using CodeFlow.ManualOperations;
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
        private ManuaCode manualCode;
        private Dictionary<string, Guid> features;
        private Dictionary<string, Guid> modules;

        public CreateInGenioForm(ManuaCode manual)
        {
            InitializeComponent();
            manualCode = manual;
            features = new Dictionary<string, Guid>();
            modules = new Dictionary<string, Guid>();
            DialogResult = DialogResult.Cancel;
        }

        private void LoadFormData()
        {
            rtCode.Text = manualCode.Code;

            cmbPlataform.Items.Clear();
            cmbType.Items.Clear();
            
            cmbPlataform.Items.AddRange(PackageOperations.Instance.GetActiveProfile().GenioConfiguration.Plataforms.Select(x => x.ID).ToArray());
        }

        private void LoadDBInfo()
        {
            cmbModule.Items.Clear();
            cmbFeature.Items.Clear();

            modules = PackageOperations.Instance.GetActiveProfile().GenioConfiguration.GetModules();
            features = PackageOperations.Instance.GetActiveProfile().GenioConfiguration.GetFeatures();
            
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
            btnCreate.Enabled = true;
            lblProfile.Text = PackageOperations.Instance.GetActiveProfile().ToString();
            lblProfile.ForeColor= Color.Green;
            lblWarning.Visible = false;

            if (!String.IsNullOrEmpty(PackageOperations.Instance.SolutionProps.ClientInfo.Version)
                && !String.IsNullOrEmpty(PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion))
                lblSolutionVersion.Text = String.Format(Properties.Resources.SolutionVersion,
                    PackageOperations.Instance.SolutionProps.ClientInfo.Version, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion);
            else
                lblSolutionVersion.Text = String.Format(Properties.Resources.ProfileVersion, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion);

            if (PackageOperations.Instance.GetActiveProfile().GenioConfiguration.ProductionSystem)
            {
                lblProd.Text = String.Format(Properties.Resources.ProfileProd, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.GenioVersion);
                lblProd.ForeColor = Color.DarkRed;

                if (!String.IsNullOrEmpty(PackageOperations.Instance.SolutionProps.ClientInfo.Version)
                    && !String.IsNullOrEmpty(PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion)
                    && !PackageOperations.Instance.SolutionProps.ClientInfo.Version.Equals(PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion))
                {
                    lblWarning.Text = String.Format(Properties.Resources.WarningProfile);
                    lblWarning.Visible = true;
                }
            }
            else
            {
                lblProd.Text = String.Format(Properties.Resources.ProfileDev, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.GenioVersion);
                lblProd.ForeColor = Color.DarkGreen;
            }
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
            cmbModule.Enabled = !chkSystem.Checked;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string feature = (string)cmbFeature.SelectedItem ?? cmbFeature.SelectedText;
            string module = (string)cmbModule.SelectedItem ?? cmbModule.SelectedText;
            string plataform = (string)cmbPlataform.SelectedItem ?? cmbPlataform.SelectedText;
            string type = (string)cmbType.SelectedItem ?? cmbType.SelectedText;
            string param = txtParam.Text;
            string file = txtFile.Text;
            string order = txtOrder.Text;
            bool inhib = chkInhibt.Checked;
            bool system = chkSystem.Checked;
            string lang = "";

            if(String.IsNullOrEmpty(plataform) 
                || (!system && String.IsNullOrEmpty(module))
                || String.IsNullOrEmpty(type))
            {
                MessageBox.Show(Properties.Resources.CreateError, Properties.Resources.Create, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            GenioPlataform plat = PackageOperations.Instance.GetActiveProfile().GenioConfiguration.Plataforms.Find(x => x.ID.Equals(plataform));
            if(plat != null)
            {
                TipoRotina tipo = plat.TipoRotina.Find(x => x.Identifier.Equals(type));
                if (tipo != null)
                    lang = tipo.ProgrammingLanguage;
            }

            Double.TryParse(order, out var f_order);

            Guid codfeature = Guid.Empty;
            Guid codmodul = Guid.Empty;

            if (!String.IsNullOrEmpty(feature))
                features.TryGetValue(feature, out codfeature);

            if (!String.IsNullOrEmpty(module))
                modules.TryGetValue(module, out codmodul);

            manualCode.Code = rtCode.Text;
            manualCode.Codfeature = codfeature;
            manualCode.Feature = feature ?? "";
            manualCode.Codmodul = codmodul;
            manualCode.Modulo = module != null ? (system ? PackageOperations.Instance.GetActiveProfile().GenioConfiguration.SystemInitials : module ) : ""; //Change to system of active profile
            manualCode.Plataform = plataform;
            manualCode.TipoRotina = type;
            manualCode.Parameter = param;
            manualCode.ManualFile = file;
            manualCode.Order = (float)Math.Round(f_order, 1);
            manualCode.Inhib = inhib ? 1 : 0;
            manualCode.System = system ? 1 : 0;
            manualCode.Lang = lang ?? "";

            try
            {
                IOperation operation = new CodeCreate(manualCode).GetOperation();
                if (operation != null && PackageOperations.Instance.ExecuteOperation(operation))
                {
                    MessageBox.Show(Properties.Resources.CodeCreated, Properties.Resources.Create, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                    MessageBox.Show(String.Format(Properties.Resources.UnableToExecuteOperation, ""),
                        Properties.Resources.CreateInGenio, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorCreate, ex.Message), 
                    Properties.Resources.CreateInGenio, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateInGenioForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void cmbPlataform_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbType.Items.Clear();
            string plataform = cmbPlataform.SelectedItem?.ToString() ?? "";
            GenioPlataform plat = PackageOperations.Instance.GetActiveProfile().GenioConfiguration.Plataforms.Find(x => x.ID.Equals(plataform));
            if (plat != null)
                cmbType.Items.AddRange(plat.TipoRotina.Select(x => x.Identifier).ToArray());
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string plataform = (string)cmbPlataform.SelectedItem ?? cmbPlataform.SelectedText;
            string type = (string)cmbType.SelectedItem ?? cmbType.SelectedText;

            SelectParametersForm select = new SelectParametersForm();
            select.Plataform = plataform;
            select.TypeRot = type;

            select.ShowDialog();

            if(select.DialogResult != DialogResult.Cancel)
                UpdateSelection(select.Plataform, select.TypeRot);
        }

        private void UpdateSelection(string plataform, string typeRot)
        {
            int i = 0;
            int foundIDX = -1;
            foreach (var item in cmbPlataform.Items)
            {
                if (item.Equals(plataform))
                {
                    foundIDX = i;
                    break;
                }
                i++;
            }
            if (foundIDX != -1)
                cmbPlataform.SelectedIndex = foundIDX;

            cmbType.Items.Clear();
            GenioPlataform plat = PackageOperations.Instance.GetActiveProfile().GenioConfiguration.Plataforms.Find(x => x.ID.Equals(plataform));
            if (plat != null)
                cmbType.Items.AddRange(plat.TipoRotina.Select(x => x.Identifier).ToArray());

            foundIDX = -1;
            i = 0;
            foreach (var item in cmbType.Items)
            {
                if (item.Equals(typeRot))
                {
                    foundIDX = i;
                    break;
                }
                i++;
            }
            if (foundIDX != -1)
                cmbType.SelectedIndex = foundIDX;
        }
    }
}
