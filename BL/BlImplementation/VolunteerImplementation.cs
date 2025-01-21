namespace BlImplementation;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using BO;
using BlApi;
using DO;


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
    /// 
    public string Login(string username, string password)
    {
        // Retrieve all volunteers from the DAL
        var volunteersFromDal = _dal.Volunteer.ReadAll();

        // Encrypt the provided password for comparison
        string encryptedPassword = AesEncryptionHelper.Encrypt(password);

        // Find the volunteer with matching username and encrypted password
        var vol = volunteersFromDal.FirstOrDefault(v => v.Name == username && v.Password == encryptedPassword);

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
            Address = volunteerData.Address,
            Latitude = volunteerData.Latitude,
            Longitude = volunteerData.Longitude,
            MaxDistance = volunteerData.MaxDistance,
            Password = volunteerData.Password,
            CurrentCall = null // Initialize as null
        };

        // Check if the volunteer has an active assignment
        var assignments = _dal.Assignment.ReadAll();
        var activeAssignment = assignments.FirstOrDefault(a => a.VolunteerId == volunteerId && a.EndTime == null);

        if (activeAssignment != null)
        {
            // Retrieve the associated call from the DAL
            var callData = _dal.Call.Read(activeAssignment.CallId);

            // Map the active assignment and call to BO.CallInProgress
            var callInProgress = new BO.CallInProgress
            {
                Id = activeAssignment.Id,
                CallId = callData.Id,
                CallType = (BO.CallType)callData.CallType,
                GeneralDescription = callData.Description,
                Address = callData.Address,
                StartTime = callData.StartTime,
                EstimatedCompletionTime = activeAssignment.EndTime,
                AssignmentStartTime = activeAssignment.StartTime,
                Distance = VolunteerManager.CalculateDistance(volunteerData.Latitude, volunteerData.Longitude, callData.Latitude, callData.Longitude),
                Status = BO.CallType.InTreatment
            };

            // Assign the call in progress to the volunteer
            volunteer.CurrentCall = callInProgress;
        }
        return volunteer;
    }

    public int FindVolunteerID(string name)
    {
        // Retrieve all volunteers from the DAL
        var volunteersFromDal = _dal.Volunteer.ReadAll();

        foreach (var volunteer in volunteersFromDal)
        {
            if (volunteer.Name == name)
            {
                return volunteer.Id;
            }
        }

        // If no volunteer is found, throw an exception or return a default value
        throw new BlDoesNotExistException($"Volunteer with name {name} not found.");

    }



    /// <summary>
    /// Retrieves a list of volunteers filtered by call type and optionally sorted by specified fields.
    /// </summary>
    /// <param name="callType">Filter to include only volunteers with the specified current call type.</param>
    /// <param name="sortByField">
    /// Optional sorting field for the results. Defaults to sorting by volunteer ID if not specified.
    /// Supported fields include ID, Name, IsActive, CompletedAssignmentsCount, CancelledCallsCount, ExpiredCallsCount, and CurrentCallType.
    /// </param>
    /// <returns>
    /// A collection of <see cref="BO.VolunteerInList"/> objects representing the filtered and sorted list of volunteers.
    /// </returns>
    /// <exception cref="LogicException">
    /// Thrown when:
    /// - An invalid sorting field is provided.
    /// - Other unexpected errors occur during the retrieval or processing of the volunteer list.
    /// </exception>
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
    /// 
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
                case BO.VolunteerInListSortFields.CallType:
                    // Sort volunteers by the type of the current call they are assigned to
                    volunteersFromDal = volunteersFromDal.OrderBy(v =>
                    {
                        // Retrieve the assignment for the current volunteer
                        var currentAssignment = _dal.Assignment.ReadAll()
                            .FirstOrDefault(a => a.VolunteerId == v.Id && a.EndTime == null);

                        // Retrieve the associated call type, or default to BO.CallType.None if no active assignment
                        return currentAssignment == null
                            ? BO.CallType.None
                            : _dal.Call.ReadAll()
                                .Where(c => c.Id == currentAssignment.CallId)
                                .Select(c => (BO.CallType?)c.CallType)
                                .FirstOrDefault() ?? BO.CallType.None;
                    });
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

        // Convert the sorted list to a list of BO.VolunteerInList
        IEnumerable<DO.Volunteer> volunteers = volunteersFromDal.ToList(); // Convert to List to preserve sorting
        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

        return volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            Name = v.Name,
            IsActive = v.IsActive,
            CompletedAssignmentsCount = assignments.Count(call => call.VolunteerId == v.Id && Enum.Equals(call.EndType, DO.EndType.Completed)),
            CancelledCallsCount = assignments.Count(call => call.VolunteerId == v.Id && Enum.Equals(call.EndType, DO.EndType.SelfCanceled)),
            ExpiredCallsCount = assignments.Count(call => call.VolunteerId == v.Id && Enum.Equals(call.EndType, DO.EndType.Expired)),
            CurrentCallId = assignments.Where(call => call.VolunteerId == v.Id).Select(call => call.CallId).FirstOrDefault(),
            CurrentCallType = assignments.Where(call => call.VolunteerId == v.Id).Select(call =>
            {
                if (call.EndType == null)
                {
                    return BO.CallType.None; // Default value when EndType is null
                }

                switch ((BO.EndType)call.EndType)
                {
                    case BO.EndType.Completed:
                        return BO.CallType.Completed;
                    case BO.EndType.SelfCanceled:
                        return BO.CallType.SelfCanceled;
                    case BO.EndType.Expired:
                        return BO.CallType.Expired;
                    case BO.EndType.AdminCanceled:
                        return BO.CallType.AdminCanceled;
                    default:
                        throw new InvalidOperationException("Unknown EndType");
                }
            }).FirstOrDefault()
        });
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
        AdminManager.ThrowOnSimulatorIsRunning();
        var currentVolunteer = _dal.Volunteer.Read(updatedVolunteer.Id);

        // Validate that the requester is either the volunteer themselves or an admin
        if (!VolunteerManager.IsRequesterAuthorizedToCancel(requesterId, updatedVolunteer.Id))
        {
            throw new LogicException("The requester is not authorized to update this volunteer.");
        }

        if (string.IsNullOrWhiteSpace(updatedVolunteer.Id.ToString()))
            throw new BlNullPropertyException("Volunteer ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(updatedVolunteer.Name))
            throw new BlNullPropertyException("Volunteer name cannot be empty.");

        if (string.IsNullOrWhiteSpace(updatedVolunteer.Phone))
            throw new BlNullPropertyException("Volunteer phone number cannot be empty.");

        if (string.IsNullOrWhiteSpace(updatedVolunteer.Email))
            throw new BlNullPropertyException("Volunteer email cannot be empty.");

        if (string.IsNullOrWhiteSpace(updatedVolunteer.Password))
            throw new BlNullPropertyException("Volunteer password cannot be empty.");

        if (string.IsNullOrWhiteSpace(updatedVolunteer.Address))
            throw new BlNullPropertyException("Volunteer address cannot be empty.");
        if (!updatedVolunteer.Latitude.HasValue || string.IsNullOrWhiteSpace(updatedVolunteer.Latitude.ToString()))
            throw new BlNullPropertyException("Volunteer Latitude cannot be empty.");

        if (!updatedVolunteer.Longitude.HasValue || string.IsNullOrWhiteSpace(updatedVolunteer.Longitude.ToString()))
            throw new BlNullPropertyException("Volunteer Longitude cannot be empty.");

        // Check all fields
        if (!VolunteerManager.ValidName(updatedVolunteer.Name)) // Validate name
            throw new BlInvalidValueException("Invalid name.");

        if (!VolunteerManager.ValidPhone(updatedVolunteer.Phone)) // Validate phone
            throw new BlInvalidValueException("Invalid phone number.");

        if (!VolunteerManager.ValidPassword(updatedVolunteer.Password)) // Validate Password
            throw new BlInvalidValueException("The password must be at least 8 characters long and must contain a capital letter, a lowercase letter, and one digit");

        if (!VolunteerManager.ValidEmail(updatedVolunteer.Email)) // Validate email
            throw new BlInvalidValueException("Invalid email.");

        if (!VolunteerManager.ValidAddress(updatedVolunteer.Address)) // Validate address
            throw new BlInvalidValueException("Invalid address, Please enter the address in this template: number, street , city ");

        // Ensure coordinates are not null and valid
        if (!updatedVolunteer.Latitude.HasValue || !updatedVolunteer.Longitude.HasValue)
            throw new BlNullPropertyException("Coordinates cannot be null.");

        // Validate coordinates with partial address
        var closestCoordinates = VolunteerManager.GetClosestCoordinates(updatedVolunteer.Address, updatedVolunteer.Latitude.Value, updatedVolunteer.Longitude.Value);
        if (closestCoordinates == null)
            throw new BlInvalidValueException("Coordinates do not match the address.");

        string encryptedPassword = AesEncryptionHelper.Encrypt(updatedVolunteer.Password);
        updatedVolunteer.Password = encryptedPassword;

        if (currentVolunteer == null)
            throw new BlDoesNotExistException("Volunteer with the given ID does not exist.");

        // Check which fields have changed
        var updatedVolunteerDO = new DO.Volunteer
        {
            Id = updatedVolunteer.Id,
            Name = updatedVolunteer.Name,
            Phone = updatedVolunteer.Phone,
            Email = updatedVolunteer.Email,
            Password = updatedVolunteer.Password,
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
        VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteerDO.Id);
        VolunteerManager.Observers.NotifyListUpdated();
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
        AdminManager.ThrowOnSimulatorIsRunning();
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
        VolunteerManager.Observers.NotifyListUpdated();
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
        AdminManager.ThrowOnSimulatorIsRunning();
        // Check for null values
        if (volunteer == null)
            throw new BlNullPropertyException("Volunteer object cannot be null.");

        if (string.IsNullOrWhiteSpace(volunteer.Id.ToString()))
            throw new BlNullPropertyException("Volunteer ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(volunteer.Name))
            throw new BlNullPropertyException("Volunteer name cannot be empty.");

        if (string.IsNullOrWhiteSpace(volunteer.Phone))
            throw new BlNullPropertyException("Volunteer phone number cannot be empty.");

        if (string.IsNullOrWhiteSpace(volunteer.Email))
            throw new BlNullPropertyException("Volunteer email cannot be empty.");

        if (string.IsNullOrWhiteSpace(volunteer.Password))
            throw new BlNullPropertyException("Volunteer password cannot be empty.");

        if (string.IsNullOrWhiteSpace(volunteer.Address))
            throw new BlNullPropertyException("Volunteer address cannot be empty.");

        if (!volunteer.Latitude.HasValue || string.IsNullOrWhiteSpace(volunteer.Latitude.ToString()))
            throw new BlNullPropertyException("Volunteer Latitude cannot be empty.");

        if (!volunteer.Longitude.HasValue || string.IsNullOrWhiteSpace(volunteer.Longitude.ToString()))
            throw new BlNullPropertyException("Volunteer Longitude cannot be empty.");


        // Validate all fields
        if (!VolunteerManager.ValidId(volunteer.Id.ToString()))
            throw new BlInvalidValueException("Invalid ID.");

        if (!VolunteerManager.ValidName(volunteer.Name))
            throw new BlInvalidValueException("Invalid name.");

        if (!VolunteerManager.ValidPhone(volunteer.Phone))
            throw new BlInvalidValueException("Invalid phone number.");

        if (!VolunteerManager.ValidEmail(volunteer.Email))
            throw new BlInvalidValueException("Invalid email.");

        if (!VolunteerManager.ValidPassword(volunteer.Password))
            throw new BlInvalidValueException("The password must be at least 8 characters long and must contain a capital letter, a lowercase letter, and one digit");

        if (!VolunteerManager.ValidAddress(volunteer.Address)) // Validate address
            throw new BlInvalidValueException("Invalid address.");

        if (!double.TryParse(volunteer.Latitude.ToString(), out double latitude))
            throw new BlInvalidValueException("Invalid latitude value. Must be a valid number.");
        if (!double.TryParse(volunteer.Longitude.ToString(), out double longitude))
            throw new BlInvalidValueException("Invalid longitude value. Must be a valid number.");

        // Ensure coordinates are not null and valid
        if (!volunteer.Latitude.HasValue || !volunteer.Longitude.HasValue)
            throw new BlNullPropertyException("Coordinates cannot be null.");

        // Validate coordinates with partial address
        var closestCoordinates = VolunteerManager.GetClosestCoordinates(volunteer.Address, volunteer.Latitude.Value, volunteer.Longitude.Value);
        if (closestCoordinates == null)
            throw new BlInvalidValueException("Coordinates do not match the address.");

        // Create a new DO.Volunteer object
        var newVolunteer = new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Password = volunteer.Password,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude.Value,
            Longitude = volunteer.Longitude.Value,
            Role = (DO.Role)volunteer.Role, // Convert BO.Role to DO.Role
            IsActive = volunteer.IsActive
        };

        // Check if the volunteer already exists by ID
        var existingVolunteer = _dal.Volunteer.Read(volunteer.Id);
        if (existingVolunteer != null)
            throw new BlInvalidValueException($"Volunteer with ID {volunteer.Id} already exists.");

        try
        {
            // Attempt to add the volunteer to the database
            _dal.Volunteer.Create(newVolunteer);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            // Catch and rethrow exceptions to the presentation layer
            throw new LogicException("Failed to add volunteer.", ex);
        }
    }
    private static bool IsEncrypted(string password)
    {
        try
        {
            Convert.FromBase64String(password);
            return true;
        }
        catch
        {
            return false;
        }
    }



    /// <summary>
    /// Retrieves all current calls assigned to a volunteer.
    /// </summary>
    /// <param name="volunteerId">The unique ID of the volunteer.</param>
    /// <returns>A list of <see cref="BO.CallInProgress"/> objects representing all current calls.</returns>
    public List<BO.CallInProgress> GetCurrentCallsForVolunteer(int volunteerId)
    {
        var assignments = _dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteerId && a.EndTime == null); // Active assignments only

        var currentCalls = new List<BO.CallInProgress>();

        foreach (var assignment in assignments)
        {
            var callData = _dal.Call.Read(assignment.CallId);

            var callInProgress = new BO.CallInProgress
            {
                Id = assignment.Id,
                CallId = callData.Id,
                CallType = (BO.CallType)callData.CallType,
                GeneralDescription = callData.Description,
                Address = callData.Address,
                StartTime = callData.StartTime,
                EstimatedCompletionTime = assignment.EndTime,
                AssignmentStartTime = assignment.StartTime,
                Distance = VolunteerManager.CalculateDistance(
                    _dal.Volunteer.Read(volunteerId).Latitude,
                    _dal.Volunteer.Read(volunteerId).Longitude,
                    callData.Latitude,
                    callData.Longitude),
                Status = (BO.CallType)callData.CallType
            };

            currentCalls.Add(callInProgress);
        }

        return currentCalls;
    }



    // ...
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
     VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


}


