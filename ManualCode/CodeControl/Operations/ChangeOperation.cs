using CodeFlow.ManualOperations;
using System;

namespace CodeFlow.CodeControl
{
    public class ChangeOperation : IOperation
    {
        private IChange operationData;
        private DateTime operationTime;
        public ChangeOperation()
        {

        }

        public ChangeOperation(IChange data)
        {
            OperationChanges = data ?? throw new ArgumentNullException(nameof(data));
            OperationTime = DateTime.Now;
        }
        public IChange OperationChanges { get => operationData; set => operationData = value; }
        public DateTime OperationTime { get => operationTime; set => operationTime = value; }
        public string LocalFileName { get => operationData?.Mine?.LocalFileName ?? ""; }
        public string OperationType => "Modified";

        public bool Undo(Profile profile)
        {
            bool result = false;
            try
            {
                result = Execute(profile, OperationChanges.Theirs);
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
                result = Execute(profile, OperationChanges.Merged);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(Properties.Resources.UnableToExecuteOperation, e.Message));
            }

            return result;
        }
        private bool Execute(Profile profile, IManual man)
        {
            return man.Update(profile);
        }
    }
}
