namespace BlImplementation;
using BlApi;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public string Login(string username, string password)
    {
        try
        {
            var volunteersFromDal = _dal.Volunteer.ReadAll();
            string passwordAsString = password.ToString();
            var vol = volunteersFromDal.FirstOrDefault(v => v.Name == username && v.Password == passwordAsString);
            if (vol == null)
            {
                throw new BO.ArgumentNullException("Volunteer not found or incorrect password.");
            }
            return vol.Role.ToString();
        }
        catch (DO.ArgumentNullException ex)
        {
            // This handles the specific case where the volunteer is not found or password is incorrect
            throw new BO.ArgumentNullException(ex.Message);
        }
    }

    public void AddVolunteer(BO.Volunteer newVolunteer)
    {

        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public BO.Volunteer GetVolunteerDetails(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerInListSortFields? sortByField = null)
    {
        try
        {
            var volunteersFromDal = _dal.Volunteer.ReadAll();
            if (isActive.HasValue)
            {
                volunteersFromDal = volunteersFromDal.Where(v => v.IsActive == isActive.Value);
            }
            if (sortByField.HasValue)
            {
                switch (sortByField.Value)
                {
                    case BO.VolunteerInListSortFields.Id:
                        volunteersFromDal = volunteersFromDal.OrderBy(v => v.Id);
                        break;
                    case BO.VolunteerInListSortFields.Name:
                        volunteersFromDal = volunteersFromDal.OrderBy(v => v.Name);
                        break;
                    case BO.VolunteerInListSortFields.Phone:
                        volunteersFromDal = volunteersFromDal.OrderBy(v => v.Phone);
                        break;
                    case BO.VolunteerInListSortFields.IsActive:
                        volunteersFromDal = volunteersFromDal.OrderBy(v => v.IsActive);
                        break;
                    case BO.VolunteerInListSortFields.Role:
                        volunteersFromDal = volunteersFromDal.OrderBy(v => v.Role);
                        break;
                    case BO.VolunteerInListSortFields.Latitude:
                        volunteersFromDal = volunteersFromDal.OrderBy(v => v.Latitude);
                        break;
                    case BO.VolunteerInListSortFields.Longitude:
                        volunteersFromDal = volunteersFromDal.OrderBy(v => v.Longitude);
                        break;

                     
                    default:
                        throw new ArgumentException("Invalid sort field", nameof(sortByField));
                }
            }
            else
            {
                volunteersFromDal = volunteersFromDal.OrderBy(v => v.Id);
            }
            return BO.volunteersFromDal;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error occurred while processing the volunteer list.", ex);
        }
    }






    public void UpdateVolunteer(int requesterId, BO.Volunteer updatedVolunteer)
    {
        throw new NotImplementedException();
    }
}


