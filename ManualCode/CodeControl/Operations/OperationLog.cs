using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class OperationLog
    {
        private ObservableCollection<IOperation> operations = new ObservableCollection<IOperation>();

        public OperationLog()
        {
        }

        public ObservableCollection<IOperation> OperationList { get => operations; set => operations = value; }

        public void LogOperation(IOperation oper)
        {
            OperationList.Insert(0, oper);
        }

        public void Clear()
        {
            OperationList.Clear();
        }
    }
}
