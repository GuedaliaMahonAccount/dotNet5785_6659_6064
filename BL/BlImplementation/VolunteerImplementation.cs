namespace BlImplementation;
using BlApi;
using BO;
using DO;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
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






    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer updatedVolunteer)
    {
        if (requesterId != updatedVolunteer.Id)
        {
            throw new UnauthorizedAccessException("Only the volunteer or a manager can update the details.");
        }

        //בדיקה שכל הערכים תקינים
        //ValidateVolunteerDetails(updatedVolunteer);

        var existingVolunteer = _dal.Volunteer.Read(updatedVolunteer.Id);

        if (existingVolunteer == null)
        {
            throw new VolunteerNotFoundException($"Volunteer with ID {updatedVolunteer.Id} not found.");
        }

        var updatedVolunteerDO = new DO.Volunteer
        {
            Id = updatedVolunteer.Id,
            Name = updatedVolunteer.Name,
            Phone = updatedVolunteer.Phone,
            Email = updatedVolunteer.Email,
            IsActive = updatedVolunteer.IsActive,
            Role = (DO.Role)updatedVolunteer.Role,
            DistanceType = (DO.DistanceType)updatedVolunteer.DistanceType,
            Password = updatedVolunteer.Password,
            Address = updatedVolunteer.Address,
            Latitude = updatedVolunteer.Latitude,
            Longitude = updatedVolunteer.Longitude,
            MaxDistance = updatedVolunteer.MaxDistance
        };

        if (requesterId != updatedVolunteer.Id)
        {
            updatedVolunteerDO.Role = existingVolunteer.Role;
        }

        _dal.Volunteer.Update(updatedVolunteerDO);
    }

    private void ValidateVolunteerDetails(Volunteer volunteer)
    {
        if (!IsValidEmail(volunteer.Email))
        {
            throw new ArgumentException("Invalid email format.");
        }

        if (!IsValidIsraeliId(volunteer.Id))
        {
            throw new ArgumentException("Invalid Israeli ID number.");
        }

        if (!IsValidAddress(volunteer.Address, out double? latitude, out double? longitude))
        {
            throw new ArgumentException("Invalid address.");
        }

        volunteer.Latitude = latitude;
        volunteer.Longitude = longitude;
        if (volunteer.MaxDistance < 0)
        {
            throw new ArgumentException("Max distance cannot be negative.");
        }
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool IsValidIsraeliId(int id)
    {
        string idString = id.ToString().PadLeft(9, '0');
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = idString[i] - '0';
            sum += (i % 2 == 0) ? digit : (digit * 2 > 9) ? digit * 2 - 9 : digit * 2;
        }
        return sum % 10 == 0;
    }

    private bool IsValidAddress(string address, out double? latitude, out double? longitude)
    {
        // בדיקת תקינות הכתובת וקבלת קווי האורך והרוחב
        // ניתן להשתמש בשירותי מיפוי כמו Google Maps API לקבלת פרטי הכתובת
        // לדוגמה:
        // var location = GetLocationFromAddress(address);
        // latitude = location.Latitude;
        // longitude = location.Longitude;
        // return location != null;

        // במקרה זה, נחזיר ערכים דיפולטיביים
        latitude = null;
        longitude = null;
        return true;
    }
}

public class VolunteerNotFoundException : Exception
{
    public VolunteerNotFoundException(string message) : base(message) { }
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




