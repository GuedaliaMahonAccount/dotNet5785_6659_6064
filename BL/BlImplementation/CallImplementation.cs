using BlApi;

namespace BlImplementation
{

    internal class CallImplementation : ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AddCall(BO.Call Newcall)
        {
            throw new NotImplementedException();
        }

        public void CancelCall(int requesterId, int assignmentId)
        {
            throw new NotImplementedException();
        }

        public void CompleteCall(int volunteerId, int assignmentId)
        {
            throw new NotImplementedException();
        }

        public void DeleteCall(int callId)
        {
            throw new NotImplementedException();
        }

        public BO.Call GetCallDetails(int callId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BO.CallInList> GetCallList(Enum? filterField, object? filterValue, Enum? sortField)
        {
            throw new NotImplementedException();
        }

        public int[] GetCallQuantities()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volunteerId, Enum? callType, Enum? sortField)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, Enum? callType, Enum? sortField)
        {
            throw new NotImplementedException();
        }

        public void selectionCall(int volunteerId, int callId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCall(BO.Call call)
        {
            throw new NotImplementedException();
        }
    }
}
