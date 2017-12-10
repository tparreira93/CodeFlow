using CodeFlow.ManualOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlow.GenioManual;

namespace CodeFlow.CodeControl
{
    public class CreateOperation : IOperation
    {
        private IChange operationData;
        private DateTime operationTime;
        public CreateOperation()
        {

        }

        public CreateOperation(IChange data)
        {
            OperationChanges = data ?? throw new ArgumentNullException(nameof(data));
            OperationTime = DateTime.Now;
        }
        public IChange OperationChanges { get => operationData; set => operationData = value; }
        public DateTime OperationTime { get => operationTime; set => operationTime = value; }
        public string LocalFileName { get => operationData?.Mine?.LocalFileName ?? ""; }
        public string OperationType => "Create";

        public bool Undo(Profile profile)
        {
            bool result;
            try
            {
                result = OperationChanges.Mine.Delete(profile);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(Properties.Resources.UnableToExecuteOperation, e.Message));
            }
            return result;
        }
        public bool Redo(Profile profile)
        {
            return Execute(profile);
        }
        public bool Execute(Profile profile)
        {
            bool result;
            try
            {
                result = Execute(profile, OperationChanges.Mine);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(Properties.Resources.UnableToExecuteOperation, e.Message));
            }

            return result;
        }
        private bool Execute(Profile profile, IManual man)
        {
            return man.Create(profile);
        }
    }
}
