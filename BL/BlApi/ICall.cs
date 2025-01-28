namespace BlApi
{
    public interface ICall : IObservable
    {
        int[] GetCallQuantities();
        IEnumerable<BO.CallInList> GetCallList(BO.CallType? callType = null, BO.CallSortField? sortByField = null);
        BO.Call GetCallDetails(int callId);
        Task UpdateCallAsync(BO.Call call); // Changed to async
        void DeleteCall(int callId);
        Task AddCallAsync(BO.Call newCall); // Changed to async
        IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volunteerId, Enum? callType, Enum? sortField);
        IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, Enum? callType, Enum? sortField);
        void CompleteCall(int volunteerId, int assignmentId);
        void CancelCall(int requesterId, int assignmentId);
        void selectionCall(int volunteerId, int callId, bool isSimulator);
        IEnumerable<BO.Call> CallHistoryByVolunteerId(int volunteerId);
        int GetAssignmentIdByCallId(int callId, int volunteerId);
        double _CalculateDistance(int callId, int volunteerId);
    }
}
