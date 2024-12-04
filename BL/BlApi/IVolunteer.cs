using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlApi
{

    public interface IVolunteer
    {
        string Login(string username, string password);
        IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive, VolunteerInList? sortBy);
        Volunteer GetVolunteerDetails(int id);
        void UpdateVolunteer(int requesterId, Volunteer updatedVolunteer);
        void DeleteVolunteer(int volunteerId);
        void AddVolunteer(Volunteer newVolunteer);
    }
}







