
namespace BlApi
{
    public interface ICall : IObservable
    {
        int[] GetCallQuantities();
        IEnumerable<BO.CallInList> GetCallList(BO.CallType? callType = null, BO.CallSortField? sortByField = null);
        BO.Call GetCallDetails(int callId);
        void UpdateCall(BO.Call call);
        void DeleteCall(int callId);
        void AddCall(BO.Call Newcall);
        IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volunteerId, Enum? callType, Enum? sortField);
        IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, Enum? callType, Enum? sortField);
        void CompleteCall(int volunteerId, int assignmentId);
        void CancelCall(int requesterId, int assignmentId);
        void selectionCall(int volunteerId, int callId);
    }

}

