using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlowUI.Helpers
{
    public class CodeFlowFormManager
    {
        private static Dictionary<Type, object> openForms = new Dictionary<Type, object>();
        private static object lockObject = new object();

        public CodeFlowFormManager()
        {

        }

        private static bool Lock()
        {
            bool lockWasTaken = false;
            Monitor.Enter(lockObject, ref lockWasTaken);

            return lockWasTaken;
        }
        private static void ReleaseLock()
        {
            Monitor.Exit(lockObject);
        }

        public static void Open(Form form)
        {
            if (Lock())
            {
                Type t = form.GetType();
                object obj = null;
                if (openForms.ContainsKey(t))
                    obj = openForms[t];

                Form f2 = obj as Form;
                if (f2 == null || f2.IsDisposed)
                {
                    Close(form.GetType());
                    openForms.Add(t, form);
                    form.Show();
                }
                else
                    f2.Focus();

                ReleaseLock();
            }
        }

        private static void Close(Type t)
        {
            if(Lock())
            {
                if (openForms.ContainsKey(t))
                    openForms.Remove(t);
                ReleaseLock();
            }
        }
    }
}
