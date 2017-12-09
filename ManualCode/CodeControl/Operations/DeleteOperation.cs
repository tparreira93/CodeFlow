using CodeFlow.ManualOperations;
using System;

namespace CodeFlow.CodeControl
{
    public class DeleteOperation : IOperation
    {
        private IChange operationData;
        private DateTime operationTime;
        public DeleteOperation()
        {

        }

        public DeleteOperation(IChange data)
        {
            OperationChanges = data ?? throw new ArgumentNullException(nameof(data));
            OperationTime = DateTime.Now;
        }
        public IChange OperationChanges { get => operationData; set => operationData = value; }
        public DateTime OperationTime { get => operationTime; set => operationTime = value; }
        public string LocalFileName { get => operationData?.Mine?.LocalFileName ?? ""; }
        public string OperationType => "Delete";

        public bool Undo(Profile profile)
        {
            bool result = false;
            try
            {
                result = OperationChanges.Theirs.Create(profile);
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
            bool result = false;
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
        public bool Execute(Profile profile, IManual man)
        {
            return man.Delete(profile);
        }
    }
}
