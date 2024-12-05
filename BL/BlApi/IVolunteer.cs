using BO;
using System;
using System.Collections.Generic;



namespace BlApi
{

    public interface IVolunteer
    {
        string Login(string username, string password);
        IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive = null, VolunteerInListSortFields? sortByField = null);
        Volunteer GetVolunteerDetails(int volunteerId);
        void UpdateVolunteer(int requesterId, Volunteer updatedVolunteer);
        void DeleteVolunteer(int volunteerId);
        void AddVolunteer(Volunteer volunteer);
    }
}







