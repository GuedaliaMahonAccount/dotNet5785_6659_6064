﻿namespace BlImplementation;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using BO;
using BlApi;
using DO;
using DalApi;


internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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
        // Encrypt the provided password for comparison
        string encryptedPassword = AesEncryptionHelper.Encrypt(password);

        lock (AdminManager.BlMutex)
        {
            // Retrieve all volunteers from the DAL
            var volunteersFromDal = _dal.Volunteer.ReadAll();

            // Find the volunteer with matching username and encrypted password
            var vol = volunteersFromDal.FirstOrDefault(v => v.Name == username && v.Password == encryptedPassword);

            if (vol == null)
            {
                // Volunteer not found or incorrect password
                throw new BlDoesNotExistException("Volunteer not found or incorrect password.");
            }
            return vol.Role.ToString();
        }
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
        lock (AdminManager.BlMutex)
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
                    Status = CallManager.GetStatusCall(callData.Id)
                };

                // Assign the call in progress to the volunteer
                volunteer.CurrentCall = callInProgress;
            }
            return volunteer;
        }
    }

    /// <summary>
    /// Retrieves the details of a specific volunteer, including general information and any current call in progress.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="BlDoesNotExistException"></exception>
    public int FindVolunteerID(string name)
    {
        lock (AdminManager.BlMutex)
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
        lock (AdminManager.BlMutex)
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
                            // Retrieve all assignments for the current volunteer
                            var assignmentsForVolunteer = _dal.Assignment.ReadAll()
                                .Where(a => a.VolunteerId == v.Id);

                            // Check for any active assignment
                            var currentAssignment = assignmentsForVolunteer.FirstOrDefault(a => a.EndTime == null);

                            if (currentAssignment == null)
                            {
                                // If no active assignment, return None
                                return BO.CallType.None;
                            }

                            // Retrieve the associated call type for the current assignment
                            var currentCallType = _dal.Call.ReadAll()
                                .Where(c => c.Id == currentAssignment.CallId)
                                .Select(c => (BO.CallType?)c.CallType)
                                .FirstOrDefault() ?? BO.CallType.None;


                            return BO.CallType.None;
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

            return volunteers.Select(v =>
            {
                // Retrieve all assignments for the current volunteer
                var assignmentsForVolunteer = _dal.Assignment.ReadAll()
                    .Where(a => a.VolunteerId == v.Id);

                // Find the current active assignment
                var currentAssignment = assignmentsForVolunteer.FirstOrDefault(a => a.EndTime == null);

                // Determine the CurrentCallType
                BO.CallType currentCallType = BO.CallType.None;

                if (currentAssignment != null)
                {
                    // Retrieve the call type associated with the current assignment
                    currentCallType = _dal.Call.ReadAll()
                        .Where(c => c.Id == currentAssignment.CallId)
                        .Select(c => (BO.CallType?)c.CallType)
                        .FirstOrDefault() ?? BO.CallType.None; // Default to None if null
                }

                // Map the volunteer to the BO.VolunteerInList
                return new BO.VolunteerInList
                {
                    Id = v.Id,
                    Name = v.Name,
                    IsActive = v.IsActive,
                    CompletedAssignmentsCount = assignments.Count(call => call.VolunteerId == v.Id && Enum.Equals(call.EndType, DO.EndType.Completed)),
                    CancelledCallsCount = assignments.Count(call => call.VolunteerId == v.Id && Enum.Equals(call.EndType, DO.EndType.SelfCanceled)),
                    ExpiredCallsCount = assignments.Count(call => call.VolunteerId == v.Id && Enum.Equals(call.EndType, DO.EndType.Expired)),
                    CurrentCallId = currentAssignment?.CallId ?? 0, // Assign 0 if no current assignment
                    CurrentCallType = currentCallType // Use the resolved CurrentCallType
                };
            });

        }
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
    /// <summary>
    /// Updates the details of an existing volunteer in the system.
    /// Validates the updated fields before saving.
    /// </summary>
    /// <param name="requesterId">The ID of the user (admin or the volunteer themselves) making the update request.</param>
    /// <param name="updatedVolunteer">A <see cref="BO.Volunteer"/> object containing the updated volunteer details.</param>
    /// <exception cref="BlInvalidValueException">
    /// Thrown when:
    /// - Validation for any field fails.
    /// - The coordinates do not match the provided address.
    /// - The volunteer does not exist in the database.
    /// </exception>
    public async Task UpdateVolunteerAsync(int requesterId, BO.Volunteer updatedVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        await _semaphore.WaitAsync();
        try
        {
            if (updatedVolunteer == null)
                throw new BlNullPropertyException("Volunteer object cannot be null.");

            lock (AdminManager.BlMutex)
            {
                var existingVolunteer = _dal.Volunteer.Read(updatedVolunteer.Id);
                if (existingVolunteer == null)
                    throw new BlDoesNotExistException($"Volunteer with ID {updatedVolunteer.Id} does not exist.");
            }

            // Validate the requester
            if (!VolunteerManager.IsRequesterAuthorizedToCancel(requesterId, updatedVolunteer.Id))
                throw new LogicException("The requester is not authorized to update this volunteer.");

            // Validate updated fields
            if (string.IsNullOrWhiteSpace(updatedVolunteer.Name))
                throw new BlNullPropertyException("Volunteer name cannot be empty.");

            if (!VolunteerManager.ValidName(updatedVolunteer.Name))
                throw new BlInvalidValueException("Invalid name.");

            if (!VolunteerManager.ValidPhone(updatedVolunteer.Phone))
                throw new BlInvalidValueException("Invalid phone number.");

            if (!VolunteerManager.ValidEmail(updatedVolunteer.Email))
                throw new BlInvalidValueException("Invalid email address.");

            if (!await VolunteerManager.ValidAddressAsync(updatedVolunteer.Address))
                throw new BlInvalidValueException("Invalid address format.");

            // Calculate latitude and longitude automatically from the address
            var coordinates = await VolunteerManager.GetCoordinatesFromAddressAsync(updatedVolunteer.Address);
            if (coordinates == null)
                throw new BlInvalidValueException("Unable to retrieve valid coordinates for the address.");

            updatedVolunteer.Latitude = coordinates.Value.latitude;
            updatedVolunteer.Longitude = coordinates.Value.longitude;

            if (!updatedVolunteer.Latitude.HasValue || !updatedVolunteer.Longitude.HasValue)
                throw new BlNullPropertyException("Volunteer coordinates cannot be null.");

            if (!VolunteerManager.ValidPassword(updatedVolunteer.Password))
                throw new BlInvalidValueException("Password must be at least 8 characters long, with a mix of uppercase, lowercase, and digits.");

            // Encrypt the password if it has changed
            string encryptedPassword = AesEncryptionHelper.Encrypt(updatedVolunteer.Password);

            // Update fields in the DO.Volunteer object
            var updatedVolunteerDO = new DO.Volunteer
            {
                Id = updatedVolunteer.Id,
                Name = updatedVolunteer.Name,
                Phone = updatedVolunteer.Phone,
                Email = updatedVolunteer.Email,
                Password = encryptedPassword,
                Address = updatedVolunteer.Address,
                Latitude = updatedVolunteer.Latitude.Value,
                Longitude = updatedVolunteer.Longitude.Value,
                MaxDistance = updatedVolunteer.MaxDistance.Value,
                Role = (DO.Role)updatedVolunteer.Role,
                IsActive = updatedVolunteer.IsActive,
                DistanceType = (DO.DistanceType)updatedVolunteer.DistanceType
            }; 

            // Save updates to the database
            lock (AdminManager.BlMutex)
            {
                _dal.Volunteer.Update(updatedVolunteerDO);
                VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.Id);
                VolunteerManager.Observers.NotifyListUpdated();
            }
        }
        finally
        {
            _semaphore.Release();
        }
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

        lock (AdminManager.BlMutex)
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
            var assignments = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == volunteerId);
            foreach (var assignment in assignments)
            {
                _dal.Assignment.Delete(assignment.Id);
            }
            VolunteerManager.Observers.NotifyListUpdated();
        }
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
    public async Task AddVolunteerAsync(BO.Volunteer newvolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        await _semaphore.WaitAsync();
        try
        {
            // Validate the volunteer object
            if (newvolunteer == null)
                throw new BlNullPropertyException("Volunteer object cannot be null.");

            if (string.IsNullOrWhiteSpace(newvolunteer.Name))
                throw new BlNullPropertyException("Volunteer name cannot be empty.");

            if (string.IsNullOrWhiteSpace(newvolunteer.Phone))
                throw new BlNullPropertyException("Volunteer phone number cannot be empty.");

            if (string.IsNullOrWhiteSpace(newvolunteer.Email))
                throw new BlNullPropertyException("Volunteer email cannot be empty.");

            if (string.IsNullOrWhiteSpace(newvolunteer.Password))
                throw new BlNullPropertyException("Volunteer password cannot be empty.");

            if (string.IsNullOrWhiteSpace(newvolunteer.Address))
                throw new BlNullPropertyException("Volunteer address cannot be empty.");

            // Calculate latitude and longitude automatically from the address
            var coordinates = await VolunteerManager.GetCoordinatesFromAddressAsync(newvolunteer.Address);
            if (coordinates == null)
                throw new BlInvalidValueException("Unable to retrieve valid coordinates for the address.");

            newvolunteer.Latitude = coordinates.Value.latitude;
            newvolunteer.Longitude = coordinates.Value.longitude;

            // Perform validation on individual fields
            if (!VolunteerManager.ValidName(newvolunteer.Name))
                throw new BlInvalidValueException("Invalid name.");

            if (!VolunteerManager.ValidPhone(newvolunteer.Phone))
                throw new BlInvalidValueException("Invalid phone number.");

            if (!VolunteerManager.ValidEmail(newvolunteer.Email))
                throw new BlInvalidValueException("Invalid email address.");

            if (!VolunteerManager.ValidPassword(newvolunteer.Password))
                throw new BlInvalidValueException("Password must be at least 8 characters long, with a mix of uppercase, lowercase, and digits.");

            if (!await VolunteerManager.ValidAddressAsync(newvolunteer.Address))
                throw new BlInvalidValueException("Invalid address format. Please ensure the address is valid.");

            // Encrypt the password
            string encryptedPassword = AesEncryptionHelper.Encrypt(newvolunteer.Password);

            // Create a new DO.Volunteer object
            var newVolunteer = new DO.Volunteer
            {
                Id = newvolunteer.Id,
                Name = newvolunteer.Name,
                Phone = newvolunteer.Phone,
                Email = newvolunteer.Email,
                Password = encryptedPassword,
                Address = newvolunteer.Address,
                Latitude = newvolunteer.Latitude.Value,
                Longitude = newvolunteer.Longitude.Value,
                Role = (DO.Role)newvolunteer.Role,
                IsActive = newvolunteer.IsActive,
                MaxDistance = newvolunteer.MaxDistance,
                DistanceType = (DO.DistanceType)newvolunteer.DistanceType
            };

            // Check if a volunteer with the same ID already exists
            var existingVolunteer = _dal.Volunteer.Read(newvolunteer.Id);
            if (existingVolunteer != null)
                throw new BlInvalidValueException($"Volunteer with ID {newvolunteer.Id} already exists.");

            // Save to database

            lock (AdminManager.BlMutex)
            {
                _dal.Volunteer.Create(newVolunteer);
                VolunteerManager.Observers.NotifyListUpdated();
            }

        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Retrieves all current calls assigned to a volunteer.
    /// </summary>
    /// <param name="volunteerId">The unique ID of the volunteer.</param>
    /// <returns>A list of <see cref="BO.CallInProgress"/> objects representing all current calls.</returns>
    public List<BO.CallInProgress> GetCurrentCallsForVolunteer(int volunteerId)
    {
        lock (AdminManager.BlMutex)
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
                    Status = CallManager.GetStatusCall(callData.Id)
                };

                currentCalls.Add(callInProgress);
            }

            return currentCalls;
        }
    }

    /// <summary>
    /// Retrieves the details of a specific volunteer, including general information and any current call in progress.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    /// <exception cref="BlDoesNotExistException"></exception>
    public BO.Volunteer _GetVolunteerDetails(int volunteerId)
    {
        lock (AdminManager.BlMutex)
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
                Password = AesEncryptionHelper.Decrypt(volunteerData.Password),
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
                    Status = CallManager.GetStatusCall(callData.Id)
                };

                // Assign the call in progress to the volunteer
                volunteer.CurrentCall = callInProgress;
            }
            return volunteer;
        }
    }

    /// <summary>
    /// Retrieves the details of a specific volunteer, including general information and any current call in progress.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BlDoesNotExistException"></exception>
    public int GetAdminId()
    {
        lock (AdminManager.BlMutex)
        {
            var admin = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Role == DO.Role.Admin);
            if (admin == null)
            {
                throw new BlDoesNotExistException("No admin found in the system.");
            }
            return admin.Id;
        }
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


