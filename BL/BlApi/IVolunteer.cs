using BO;


namespace BlApi
{

    public interface IVolunteer : IObservable
    {
        string Login(string username, string password);
        IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive = null, VolunteerInListSortFields? sortByField = null);
        Volunteer GetVolunteerDetails(int volunteerId);
        void UpdateVolunteer(int requesterId, Volunteer updatedVolunteer);
        void DeleteVolunteer(int volunteerId);
        void AddVolunteer(Volunteer volunteer);
        int FindVolunteerID(string name);
        List<BO.CallInProgress> GetCurrentCallsForVolunteer(int volunteerId);
    }
}







