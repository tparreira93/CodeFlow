using CodeFlowLibrary.Genio;
using CodeFlowLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFlowLibrary.Util;
using CodeFlowLibrary.Bridge;

namespace CodeFlowUI
{
    public partial class SelectParametersForm : Form
    {
        public string Plataform { get; set; }
        public string TypeRot { get; set; }
        Profile profile { get; }

        public SelectParametersForm(Profile profile)
        {
            InitializeComponent();
            this.profile = profile;
        }

        private void LoadFormInfo()
        {
            cmbPlataform.Items.AddRange(profile.GenioConfiguration.Plataforms.Select(x => x.ID).ToArray());
            int i = 0;
            int foundIDX = -1;
            foreach (var item in cmbPlataform.Items)
            {
                if (item.Equals(Plataform))
                {
                    foundIDX = i;
                    break;
                }
                i++;
            }
            if (foundIDX != -1)
                cmbPlataform.SelectedIndex = foundIDX;

            i = 0;
            foundIDX = -1;
            foreach (var item in cmbType.Items)
            {
                if (item.Equals(TypeRot))
                {
                    foundIDX = i;
                    break;
                }
                i++;
            }
            if (foundIDX != -1)
                cmbType.SelectedIndex = foundIDX;
        }

        private void SelectParameters_Load(object sender, EventArgs e)
        {
            LoadFormInfo();
        }

        private void cmbPlataform_SelectedValueChanged(object sender, EventArgs e)
        {
            cmbType.Items.Clear();
            string plataform = cmbPlataform.SelectedItem?.ToString() ?? "";
            GenioPlataform plat = profile.GenioConfiguration.Plataforms.Find(x => x.ID.Equals(plataform));
            if (plat != null)
                cmbType.Items.AddRange(plat.TipoRotina.Select(x => x.Identifier).ToArray());
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string plataform = cmbPlataform.SelectedItem?.ToString() ?? "";
            string type = cmbType.SelectedItem?.ToString() ?? "";
            GenioPlataform plat = profile.GenioConfiguration.Plataforms.Find(x => x.ID.Equals(plataform));
            TipoRotina tipoRotina = plat.TipoRotina.Find(x => x.Identifier.Equals(type));
            txtHelp.Text = (tipoRotina?.Description?.Replace("\\r\\n", Helpers.NewLine) ?? "") + Helpers.NewLine+ Helpers.NewLine
                + (tipoRotina?.Destination?.Replace("\\r\\n", Helpers.NewLine) ?? "") + Helpers.NewLine + Helpers.NewLine
                + (tipoRotina?.Example?.Replace("\\r\\n", Helpers.NewLine) ?? "");
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Plataform = cmbPlataform.SelectedItem?.ToString() ?? "";
            TypeRot = cmbType.SelectedItem?.ToString() ?? "";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
