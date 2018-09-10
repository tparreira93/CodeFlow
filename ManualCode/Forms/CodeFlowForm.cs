using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow.Forms
{
    public abstract class CodeFlowForm : Form, ICodeFlowForm
    {
        private static Dictionary<Type, object> openForms = new Dictionary<Type, object>();

        public void Open()
        {
            Type t = this.GetType();
            object obj = null;
            if (openForms.ContainsKey(t))
                obj = openForms[t];

            Form form = obj as Form;
            if (form != null)
                form.Show();
            else
            {
                openForms.Add(t, this);
                this.Show();
            }
        }

        public void CloseForm()
        {

        }
    }
}
