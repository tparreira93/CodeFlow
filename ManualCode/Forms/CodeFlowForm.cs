using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow.Forms
{
    public abstract class CodeFlowForm : Form, ICodeFlowForm
    {
        private static Dictionary<Type, object> openForms = new Dictionary<Type, object>();
        private object lockObject = new object();

        private bool Lock()
        {
            bool lockWasTaken = false;
            Monitor.Enter(lockObject, ref lockWasTaken);

            return lockWasTaken;
        }
        private void ReleaseLock()
        {
            Monitor.Exit(lockObject);
        }

        public void Open()
        {
            if (Lock())
            {
                Type t = this.GetType();
                object obj = null;
                if (openForms.ContainsKey(t))
                    obj = openForms[t];

                Form form = obj as Form;
                if (form != null && !form.IsDisposed)
                    form.Focus();
                else
                {
                    openForms.Add(t, this);
                    this.FormClosing += CloseForm;
                    this.Show();
                }
                ReleaseLock();
            }
        }

        public void CloseForm(object sender, FormClosingEventArgs e)
        {
            if(Lock())
            {
                if (openForms.ContainsKey(this.GetType()))
                    openForms.Remove(this.GetType());
                ReleaseLock();
            }
        }
    }
}
