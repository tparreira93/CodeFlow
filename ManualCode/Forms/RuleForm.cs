using CodeFlow.Genio;
using CodeFlow.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow.Forms
{
    public partial class RuleForm : Form
    {
        public IRule R { get; private set; }
        public RuleForm(IRule r)
        {
            InitializeComponent();
            ConstructorInfo constructor = r.GetType().GetConstructor(new Type[] { });
            R = constructor.Invoke(null) as IRule;

            Util.CopyFrom(r.GetType(), r, R);
        }
        public RuleForm()
        {
            InitializeComponent();
            R = null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.R = null;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Dictionary<RuleProvider, Type> providers = Util.GetAtrributes<RuleProvider>();
            string selectedItem = cmbType.Items[cmbType.SelectedIndex].ToString();
            Type t = providers.Where(entry => entry.Key.RuleName.Equals(selectedItem) && !entry.Key.IsDefaultType).Select(entry => entry.Value).ToList().First();
            
            if (t != null)
            {
                ConstructorInfo constructor = t.GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    R = constructor.Invoke(null) as IRule;
                    R.Pattern = txtPattern.Text;
                }
            }
            this.Close();
        }

        private void Rule_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        private int FindType(string txt)
        {
            for (int i = 0; i < cmbType.Items.Count; i++)
            {
                if (cmbType.Items[i].Equals(txt))
                    return i;
            }
            return -1;
        }

        private void RefreshData()
        {
            Dictionary<RuleProvider, Type> providers = Util.GetAtrributes<RuleProvider>();
            foreach (var item in providers)
            {
                cmbType.Items.Add(item.Key.RuleName);
            }

            if (R != null)
            {
                RuleProvider provider = Util.GetAttribute<RuleProvider>(R.GetType()) as RuleProvider;
                int pos = FindType(provider.RuleName);
                if (pos != -1)
                    cmbType.SelectedIndex = pos;
            }
        }
    }
}
