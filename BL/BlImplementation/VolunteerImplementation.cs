namespace BlImplementation;
using BlApi;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using BO;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    /// <summary>
    /// Authenticates a volunteer by verifying their username and password.
    /// If authentication succeeds, the function returns the volunteer's role.
    /// </summary>
    /// <param name="username">The username of the volunteer attempting to log in.</param>
    /// <param name="password">The password provided for authentication.</param>
    /// <returns>
    /// A string representing the role of the authenticated volunteer.
    /// </returns>
    /// <exception cref="BlDoesNotExistException">
    /// Thrown when:
    /// - The username and password combination does not match any volunteer.
    /// - Other unexpected errors occur during the login process.
    /// </exception>
    public string Login(string username, string password)
    {

        // Retrieve all volunteers from the DAL
        var volunteersFromDal = _dal.Volunteer.ReadAll();

        // Convert the provided password to string (if necessary)
        string passwordAsString = password.ToString();

        // Find the volunteer with matching username and password
        var vol = volunteersFromDal.FirstOrDefault(v => v.Name == username && v.Password == passwordAsString);

        if (vol == null)
        {
            // Volunteer not found or incorrect password
            throw new BlDoesNotExistException("Volunteer not found or incorrect password.");
        }

        // Return the role of the authenticated volunteer
        return vol.Role.ToString();

    }

    /// <summary>
    /// Retrieves the details of a specific volunteer, including general information and any current call in progress.
    /// </summary>
    /// <param name="volunteerId">The unique ID of the volunteer whose details are being retrieved.</param>
    /// <returns>
    /// A <see cref="BO.Volunteer"/> object containing detailed information about the volunteer, including:
    /// - Name, contact details, role, and status.
    /// - Geographic information like address, latitude, longitude, and max distance.
    /// - Details of the current call in progress, if applicable.
    /// </returns>
    /// <exception cref="BlDoesNotExistException">
    /// Thrown when:
    /// - The volunteer with the specified ID is not found.
    /// - An error occurs during the retrieval or processing of volunteer data.
    /// </exception>
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {

        // Retrieve all volunteers from the DAL
        var volunteersFromDal = _dal.Volunteer.ReadAll();

        // Find the specific volunteer by ID
        var volunteerData = volunteersFromDal.FirstOrDefault(v => v.Id == volunteerId);

        if (volunteerData == null)
        {
            // Volunteer not found
            throw new BlDoesNotExistException($"Volunteer with ID {volunteerId} not found.");
        }

        // Map the DO.Volunteer data to a BO.Volunteer object
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

        // Map the current call in progress if applicable
        if (volunteer.CurrentCall != null)
        {
            var callInProgress = new BO.CallInProgress
            {
                Id = volunteer.CurrentCall.Id,
                CallId = volunteer.CurrentCall.CallId,
                CallType = volunteer.CurrentCall.CallType,
                GeneralDescription = volunteer.CurrentCall.GeneralDescription,
                Address = volunteer.CurrentCall.Address,
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

    /// <summary>
    /// Retrieves a list of volunteers with optional filtering by activity status and sorting by specified fields.
    /// </summary>
    /// <param name="isActive">Optional filter to include only active or inactive volunteers. If null, all volunteers are included.</param>
    /// <param name="sortByField">
    /// Optional sorting field for the results. Defaults to sorting by volunteer ID if not specified.
    /// Supported fields include ID, Name, Phone, IsActive, Role, Latitude, and Longitude.
    /// </param>
    /// <returns>
    /// A collection of <see cref="BO.VolunteerInList"/> objects representing the filtered and sorted list of volunteers.
    /// </returns>
    /// <exception cref="LogicException">
    /// Thrown when:
    /// - An invalid sorting field is provided.
    /// - Other unexpected errors occur during the retrieval or processing of the volunteer list.
    /// </exception>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerInListSortFields? sortByField = null)
    {

        // Retrieve all volunteers from the DAL
        var volunteersFromDal = _dal.Volunteer.ReadAll();

        // Filter by activity status if specified
        if (isActive.HasValue)
        {
            volunteersFromDal = volunteersFromDal.Where(v => v.IsActive == isActive.Value);
        }

        // Apply sorting based on the specified field
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
                    throw new LogicException("Invalid sort field provided.");
            }
        }
        else
        {
            // Default sorting by ID
            volunteersFromDal = volunteersFromDal.OrderBy(v => v.Id);
        }

        // Convert the data to the expected BO.VolunteerInList format
        return (IEnumerable<BO.VolunteerInList>)volunteersFromDal;

    }

    /// <summary>
    /// Updates the details of an existing volunteer in the system. 
    /// Ensures that the requester is authorized to make changes and validates the updated fields before saving.
    /// </summary>
    /// <param name="requesterId">The ID of the user (admin or the volunteer themselves) making the update request.</param>
    /// <param name="updatedVolunteer">A <see cref="BO.Volunteer"/> object containing the updated volunteer details.</param>
    /// <exception cref="BlInvalidValueException">
    /// Thrown when:
    /// - The requester is not authorized to update the volunteer.
    /// - Validation for any field (ID, name, phone, email, address, coordinates) fails.
    /// - The coordinates do not match the provided address.
    /// - An attempt is made to change the volunteer's role without admin privileges.
    /// - The volunteer does not exist in the database.
    /// - Other unexpected errors occur during the update process.
    /// </exception>
    public void UpdateVolunteer(int requesterId, BO.Volunteer updatedVolunteer)
    {
        var currentVolunteer = _dal.Volunteer.Read(updatedVolunteer.Id);

        // Validate that the requester is either the volunteer themselves or an admin
        if (!VolunteerManager.IsRequesterAuthorizedToCancel(requesterId, updatedVolunteer.Id))
        {
            throw new LogicException("The requester is not authorized to update this volunteer.");
        }

        // Check all fields
        if (!VolunteerManager.ValidId(updatedVolunteer.Id.ToString()))
            throw new BlInvalidValueException("Invalid ID.");

        if (!VolunteerManager.ValidName(updatedVolunteer.Name)) // Validate name
            throw new BlInvalidValueException("Invalid name.");

        if (!VolunteerManager.ValidPhone(updatedVolunteer.Phone)) // Validate phone
            throw new BlInvalidValueException("Invalid phone number.");

        if (!VolunteerManager.ValidEmail(updatedVolunteer.Email)) // Validate email
            throw new BlInvalidValueException("Invalid email.");

        if (!VolunteerManager.ValidAddress(updatedVolunteer.Address)) // Validate address
            throw new BlInvalidValueException("Invalid address.");

        // Ensure coordinates are not null and valid
        if (!updatedVolunteer.Latitude.HasValue || !updatedVolunteer.Longitude.HasValue)
            throw new BlNullPropertyException("Coordinates cannot be null.");

        // Ensure coordinates match the address
        if (!VolunteerManager.AreCoordinatesMatching(updatedVolunteer.Address, updatedVolunteer.Latitude.Value, updatedVolunteer.Longitude.Value))
            throw new BlInvalidValueException("Coordinates do not match the address.");

        if (currentVolunteer == null)
            throw new BlDoesNotExistException("Volunteer with the given ID does not exist.");

        // Check which fields have changed
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
            throw new LogicException("Only admin can change the role.");
        }

        // Update the volunteer in the database
        _dal.Volunteer.Update(updatedVolunteerDO);

    }

    /// <summary>
    /// Deletes a volunteer from the system. 
    /// Ensures the volunteer exists and has no associated or previously handled calls before deletion.
    /// </summary>
    /// <param name="volunteerId">The unique ID of the volunteer to be deleted.</param>
    /// <exception cref="BlDeletionImpossibleException">
    /// Thrown when:
    /// - The volunteer does not exist in the database.
    /// - The volunteer has associated or previously handled calls, making them ineligible for deletion.
    /// - Other unexpected errors occur during the deletion process.
    /// </exception>
    public void DeleteVolunteer(int volunteerId)
    {

        // Check if the volunteer exists
        var volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new BlDoesNotExistException("Volunteer with the given ID does not exist.");
        }

        // Check if the volunteer can be deleted
        var calls = _dal.Call.ReadAll().Where(c => c.Id == volunteerId);
        if (calls.Any())
        {
            throw new BlDeletionImpossibleException("Cannot delete volunteer who is currently or has previously handled calls.");
        }

        // Attempt to delete the volunteer
        _dal.Volunteer.Delete(volunteerId);

    }

    /// <summary>
    /// Adds a new volunteer to the system. 
    /// Validates all fields, ensures the data is consistent, and creates a new volunteer record in the database.
    /// </summary>
    /// <param name="volunteer">
    /// A <see cref="BO.Volunteer"/> object containing the details of the volunteer to be added.
    /// </param>
    /// <exception cref="BlInvalidValueException">
    /// Thrown when:
    /// - The ID is invalid.
    /// - The name, phone number, email, or address does not meet validation requirements.
    /// - The coordinates are null or do not match the address.
    /// - Other unexpected errors occur during the addition process.
    /// </exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        // Validate all fields
        if (!VolunteerManager.ValidId(volunteer.Id.ToString()))
            throw new BlInvalidValueException("Invalid ID.");

        if (!VolunteerManager.ValidName(volunteer.Name))
            throw new BlInvalidValueException("Invalid name.");

        if (!VolunteerManager.ValidPhone(volunteer.Phone))
            throw new BlInvalidValueException("Invalid phone number.");

        if (!VolunteerManager.ValidEmail(volunteer.Email))
            throw new BlInvalidValueException("Invalid email.");

        if (!VolunteerManager.ValidAddress(volunteer.Address))
            throw new BlInvalidValueException("Invalid address.");

        // Ensure coordinates are not null and valid
        if (!volunteer.Latitude.HasValue || !volunteer.Longitude.HasValue)
            throw new BlInvalidValueException("Coordinates cannot be null.");

        // Ensure coordinates match the address
        if (!VolunteerManager.AreCoordinatesMatching(volunteer.Address, volunteer.Latitude.Value, volunteer.Longitude.Value))
            throw new BlInvalidValueException("Coordinates do not match the address.");

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
            throw new LogicException("Failed to add volunteer.", ex);
        }
    }

}


