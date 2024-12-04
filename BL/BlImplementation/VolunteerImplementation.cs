namespace BlImplementation;
using BlApi;
using BO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void AddVolunteer(Volunteer newVolunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public Volunteer GetVolunteerDetails(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive, VolunteerInList? sortBy)
    {
        throw new NotImplementedException();
    }

    public string Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteer(int requesterId, Volunteer updatedVolunteer)
    {
        throw new NotImplementedException();
    }
}


