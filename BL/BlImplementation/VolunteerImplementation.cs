namespace BlImplementation;
using BlApi;
using BO;
using DO;
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




    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            var volunteersFromDal = _dal.Volunteer.ReadAll();

            var volunteerData = volunteersFromDal.FirstOrDefault(v => v.Id == volunteerId);

            if (volunteerData == null)
            {
                throw new VolunteerNotFoundException($"Volunteer with ID {volunteerId} not found.");
            }
            var volunteer = new BO.Volunteer
            {
                Id = volunteerData.Id,
                Name = volunteerData.Name,
                Phone = volunteerData.Phone,
                Email = volunteerData.Email,
                IsActive = volunteerData.IsActive,
                Role = (BO.Role)volunteerData.Role,
                DistanceType = (BO.DistanceType)volunteerData.DistanceType,
                Password = volunteerData.Password,
                Address = volunteerData.Address,
                Latitude = volunteerData.Latitude,
                Longitude = volunteerData.Longitude,
                MaxDistance = volunteerData.MaxDistance
            };
            if (volunteer.CurrentCall != null)
            {
                var callInProgress = new CallInProgress
                {
                    Id = volunteer.CurrentCall.Id,
                    CallId = volunteer.CurrentCall.CallId,
                    CallType = volunteer.CurrentCall.CallType,
                    GeneralDescription = volunteer.CurrentCall.GeneralDescription,
                    AdditionalNotes = volunteer.CurrentCall.AdditionalNotes,
                    StartTime = volunteer.CurrentCall.StartTime,
                    EstimatedCompletionTime = volunteer.CurrentCall.EstimatedCompletionTime,
                    AssignmentStartTime = volunteer.CurrentCall.AssignmentStartTime,
                    Distance = volunteer.CurrentCall.Distance,
                    Status = volunteer.CurrentCall.Status
                };
                volunteer.CurrentCall = callInProgress;
            }
            return volunteer;
        }
        catch (Exception ex)
        {
            throw;
        }
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
            return (IEnumerable<BO.VolunteerInList>)volunteersFromDal;
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





    public void AddVolunteer(BO.Volunteer newVolunteer)
    {

        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int volunteerId)
    {
        throw new NotImplementedException();
    }
}


