using System;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Changes;

namespace CodeFlowLibrary.CodeControl.Operations
{
    public class DeleteOperation : IOperation
    {
        private IChange operationData;
        private DateTime operationTime;
        private Profile operationProfile;

        public DeleteOperation()
        {

        }

        public DeleteOperation(IChange data, Profile operationProfile)
        {
            this.operationData = data ?? throw new ArgumentNullException(nameof(data));
            this.operationTime = DateTime.Now;
            this.operationProfile = operationProfile;
        }
        public IChange OperationChanges => operationData;
        public DateTime OperationTime => operationTime;
        public Profile OperationProfile => operationProfile;
        public string LocalFileName => operationData?.Mine?.LocalFileName ?? "";
        public string FullFileName => operationData?.Mine?.FullFileName ?? "";
        public string OperationType => "Deleted";

        public bool Undo()
        {
            bool result;
            try
            {
                result = OperationChanges.Theirs.Create(OperationProfile);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, e.Message));
            }
            return result;
        }
        public bool Redo()
        {
            return Execute();
        }
        public bool Execute()
        {
            bool result;
            try
            {
                result = Execute(OperationChanges.Mine);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, e.Message));
            }

            return result;
        }

        private bool Execute(IManual man)
        {
            return man.Delete(OperationProfile);
        }
    }
}
