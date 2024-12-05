namespace BlImplementation;
using BlApi;
using Helpers;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

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
                var callInProgress = new BO.CallInProgress
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
        var currentVolunteer = _dal.Volunteer.Read(updatedVolunteer.Id);
        // Validate that the requester is either the volunteer themselves or an admin
        if (!VolunteerManager.IsRequesterAuthorizedToCancel(requesterId, updatedVolunteer.Id))
        {
            throw new UnauthorizedAccessException("The requester is not authorized to update this volunteer.");
        }

        // Check all fields
        if (!VolunteerManager.ValidId(updatedVolunteer.Id.ToString()))
            throw new ArgumentException("Invalid ID.");

        if (!VolunteerManager.ValidName(updatedVolunteer.Name)) // Use the single name field
            throw new ArgumentException("Invalid name.");

        if (!VolunteerManager.ValidPhone(updatedVolunteer.Phone))
            throw new ArgumentException("Invalid phone number.");

        if (!VolunteerManager.ValidEmail(updatedVolunteer.Email))
            throw new ArgumentException("Invalid email.");

        if (!VolunteerManager.ValidAddress(updatedVolunteer.Address))
            throw new ArgumentException("Invalid address.");

        // Ensure coordinates are not null and valid
        if (!updatedVolunteer.Latitude.HasValue || !updatedVolunteer.Longitude.HasValue)
            throw new ArgumentException("Coordinates cannot be null.");

        if (!VolunteerManager.ValidLatitude(updatedVolunteer.Latitude.Value) || !VolunteerManager.ValidLongitude(updatedVolunteer.Longitude.Value))
            throw new ArgumentException("Invalid coordinates.");

        // Ensure coordinates match the address
        if (!VolunteerManager.AreCoordinatesMatching(updatedVolunteer.Address, updatedVolunteer.Latitude.Value, updatedVolunteer.Longitude.Value))
            throw new ArgumentException("Coordinates do not match the address.");

        if (currentVolunteer == null)
            throw new ArgumentException("Volunteer with the given ID does not exist.");

        // Check which fields have changed
        bool hasChanged = false;
        var updatedVolunteerDO = new DO.Volunteer
        {
            Id = updatedVolunteer.Id,
            Name = updatedVolunteer.Name,
            Phone = updatedVolunteer.Phone,
            Email = updatedVolunteer.Email,
            Address = updatedVolunteer.Address,
            Latitude = updatedVolunteer.Latitude.Value,
            Longitude = updatedVolunteer.Longitude.Value,
            Role = (DO.Role)updatedVolunteer.Role,
            IsActive = updatedVolunteer.IsActive
        };

        // Only admin can change the role
        if (requesterId != updatedVolunteer.Id && updatedVolunteerDO.Role != currentVolunteer.Role)
        {
            throw new UnauthorizedAccessException("Only admin can change the role.");
        }

        // Update the volunteer in the database
        try
        {
            _dal.Volunteer.Update(updatedVolunteerDO);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update volunteer in the database.", ex);
        }
    }










public void DeleteVolunteer(int volunteerId)
{
    try
    {
        // Check if the volunteer exists
        var volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new ArgumentException("Volunteer with the given ID does not exist.");
        }

        // Check if the volunteer can be deleted
        var calls = _dal.Call.ReadAll().Where(c => c.Id == volunteerId);
        if (calls.Any())
        {
            throw new InvalidOperationException("Cannot delete volunteer who is currently or has previously handled calls.");
        }

        // Attempt to delete the volunteer
        _dal.Volunteer.Delete(volunteerId);
    }
    catch (Exception ex)
    {
        // Catch and rethrow exceptions to the presentation layer
        throw new Exception("Failed to delete volunteer.", ex);
    }
}





public void AddVolunteer(BO.Volunteer volunteer)
    {
        // Validate all fields
        if (!VolunteerManager.ValidId(volunteer.Id.ToString()))
            throw new ArgumentException("Invalid ID.");

        if (!VolunteerManager.ValidName(volunteer.Name)) 
            throw new ArgumentException("Invalid name.");

        if (!VolunteerManager.ValidPhone(volunteer.Phone))
            throw new ArgumentException("Invalid phone number.");

        if (!VolunteerManager.ValidEmail(volunteer.Email))
            throw new ArgumentException("Invalid email.");

        if (!VolunteerManager.ValidAddress(volunteer.Address))
            throw new ArgumentException("Invalid address.");

        // Ensure coordinates are not null and valid
        if (!volunteer.Latitude.HasValue || !volunteer.Longitude.HasValue)
            throw new ArgumentException("Coordinates cannot be null.");

        if (!VolunteerManager.ValidLatitude(volunteer.Latitude.Value) || !VolunteerManager.ValidLongitude(volunteer.Longitude.Value))
            throw new ArgumentException("Invalid coordinates.");

        // Ensure coordinates match the address
        if (!VolunteerManager.AreCoordinatesMatching(volunteer.Address, volunteer.Latitude.Value, volunteer.Longitude.Value))
            throw new ArgumentException("Coordinates do not match the address.");

        // Create a new DO.Volunteer object
        var newVolunteer = new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude.Value,
            Longitude = volunteer.Longitude.Value,
            Role = (DO.Role)volunteer.Role, // Convert BO.Role to DO.Role
            IsActive = volunteer.IsActive
        };
        try
        {
            // Attempt to add the volunteer to the database
            _dal.Volunteer.Create(newVolunteer);
        }
        catch (Exception ex)
        {
            // Catch and rethrow exceptions to the presentation layer
            throw new Exception("Failed to add volunteer.", ex);
        }
    }


}


