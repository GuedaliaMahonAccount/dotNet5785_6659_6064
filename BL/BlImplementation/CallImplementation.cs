using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;

namespace BlImplementation
{
    internal class CallImplementation : BlApi.ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


        /// <summary>
        /// Adds a new call to the system. 
        /// Validates the input, ensures the address and coordinates are correct, and converts the BO.Call object to DO.Call before saving it in the database.
        /// </summary>
        /// <param name="newCall">The call object containing the details of the new call to be added.</param>
        /// <exception cref="BlArgumentNullException">Thrown when the provided call object is null.</exception>
        /// <exception cref="BlInvalidAddressException">Thrown when the address is empty or invalid.</exception>
        /// <exception cref="BlInvalidCoordinateException">Thrown when latitude and longitude are null or do not match the address.</exception>
        /// <exception cref="BlDoesNotExistException">Thrown when the call already exists in the system.</exception>
        public async Task AddCallAsync(BO.Call newCall)
        {
            AdminManager.ThrowOnSimulatorIsRunning();

            await _semaphore.WaitAsync(); // Acquire the semaphore
            try
            {
                if (newCall == null)
                    throw new BlArgumentNullException("Call cannot be null.");

                if (string.IsNullOrWhiteSpace(newCall.Address))
                    throw new BlInvalidAddressException("Address cannot be empty.");

                // Call the asynchronous ValidAddressAsync method
                if (!await VolunteerManager.ValidAddressAsync(newCall.Address))
                    throw new BlInvalidValueException("Invalid address.");

                // Ensure coordinates are not null and valid
                if (!newCall.Latitude.HasValue || !newCall.Longitude.HasValue)
                    throw new BlNullPropertyException("Coordinates cannot be null.");


                // Validate coordinates with partial address
                var closestCoordinates = await VolunteerManager.GetClosestCoordinatesAsync(newCall.Address, newCall.Latitude.Value, newCall.Longitude.Value);
                if (closestCoordinates == null)
                    throw new BlInvalidValueException("Coordinates do not match the address.");

                var callDO = new DO.Call
                (
                Id: _dal.Config.NextCallId,
                    CallType: (DO.CallType)newCall.CallType,
                    Address: newCall.Address,
                    Latitude: newCall.Latitude.Value,
                    Longitude: newCall.Longitude.Value,
                    StartTime: newCall.StartTime,
                    Description: newCall.Description,
                    DeadLine: newCall.DeadLine
                );

                // Add the call to the database
                _dal.Call.Create(callDO);
                CallManager.Observers.NotifyListUpdated();
            }
            finally
            {
                _semaphore.Release(); // Release the semaphore
            }

            // Notify nearby volunteers asynchronously
            await Helpers.CallManager.NotifyNearbyVolunteersAsync(newCall);
        }

        /// <summary>
        /// Cancels an assignment based on the requester's authorization and the assignment's current status. 
        /// Updates the assignment with the cancellation details if the operation is valid.
        /// </summary>
        /// <param name="requesterId">The ID of the person requesting the cancellation (can be a volunteer or an admin).</param>
        /// <param name="assignmentId">The ID of the assignment to be canceled.</param>
        /// <exception cref="BlDoesNotExistException">Thrown when no assignment is found with the provided ID.</exception>
        /// <exception cref="BlInvalidRoleException">Thrown when the requester is not authorized to cancel the assignment.</exception>
        /// <exception cref="BlInvalidValueException">Thrown when the assignment is already completed or expired.</exception>
        public void CancelCall(int requesterId, int assignmentId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
            {
                var assignmentDO = _dal.Assignment.Read(assignmentId)
                    ?? throw new BlDoesNotExistException($"No assignment found with ID: {assignmentId}");

                if (requesterId != assignmentDO.VolunteerId)
                    throw new BlInvalidRoleException("Requester is not authorized to cancel this assignment.");

                if (assignmentDO.EndType != null && (BO.EndType)assignmentDO.EndType == BO.EndType.Completed)
                    throw new BlInvalidValueException("Cannot cancel an assignment that is already completed.");

                var cancellationType = requesterId == assignmentDO.VolunteerId
                    ? DO.EndType.SelfCanceled
                    : DO.EndType.AdminCanceled;

                var updatedAssignment = assignmentDO with
                {
                    EndTime = DateTime.Now,
                    EndType = cancellationType
                };
                _dal.Assignment.Update(updatedAssignment);
            }
                CallManager.Observers.NotifyItemUpdated(assignmentId);
                CallManager.Observers.NotifyListUpdated();
        }

        /// <summary>
        /// Marks an assignment as completed by the assigned volunteer. 
        /// Validates the assignment's status and the volunteer's authorization before updating the assignment's completion details.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer completing the assignment.</param>
        /// <param name="assignmentId">The ID of the assignment to be completed.</param>
        /// <exception cref="BlDoesNotExistException">Thrown when the assignment is not found in the system.</exception>
        /// <exception cref="BlInvalidRoleException">Thrown when the volunteer is not authorized to complete the assignment.</exception>
        /// <exception cref="BlInvalidValueException">Thrown when the assignment is already completed, canceled, or expired.</exception>
        public void CompleteCall(int volunteerId, int assignmentId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
            {
                var assignmentDO = _dal.Assignment.Read(assignmentId)
                    ?? throw new BlDoesNotExistException($"No assignment found with ID: {assignmentId}");

                if (volunteerId != assignmentDO.VolunteerId)
                    throw new BlInvalidRoleException("Only the assigned volunteer can complete this assignment.");

                if (assignmentDO.EndType != null && (BO.EndType)assignmentDO.EndType == BO.EndType.Completed)
                    throw new BlInvalidValueException("Cannot complete an assignment that is already completed or canceled.");

                var updatedAssignment = assignmentDO with
                {
                    EndTime = DateTime.Now,
                    EndType = BO.EndType.Completed
                };

                _dal.Assignment.Update(updatedAssignment);
            }
                CallManager.Observers.NotifyItemUpdated(assignmentId);
                CallManager.Observers.NotifyListUpdated();
            
        }

        /// <summary>
        /// Deletes a call from the system. 
        /// Ensures the call is in an open state and has not been assigned to any volunteer before deletion.
        /// </summary>
        /// <param name="callId">The ID of the call to be deleted.</param>
        /// <exception cref="BlDoesNotExistException">Thrown when the call is not found in the system.</exception>
        /// <exception cref="BlInvalidValueException">Thrown when the call is not in the 'Open' status.</exception>
        /// <exception cref="BlDeletionImpossibleException">Thrown when the call has been assigned to one or more volunteers.</exception>
        public void DeleteCall(int callId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
            {
                var callDO = _dal.Call.Read(callId)
                    ?? throw new BlDoesNotExistException($"No call found with ID: {callId}");

                if (callDO.CallType != DO.CallType.Open)
                    throw new BlInvalidValueException("Cannot delete a call that is not in 'Open' status.");

                var assignments = _dal.Assignment.ReadAll()
                    .Where(assignment => assignment.CallId == callId);

                if (assignments.Any())
                    throw new BlDeletionImpossibleException("Cannot delete a call that has been assigned to a volunteer.");

                _dal.Call.Delete(callId);
            }
                CallManager.Observers.NotifyListUpdated();
            
        }

        /// <summary>
        /// Retrieves detailed information about a specific call, including its assignments and volunteer information.
        /// </summary>
        /// <param name="callId">The ID of the call to retrieve.</param>
        /// <returns>A <see cref="BO.Call"/> object containing the call details and a list of its assignments.</returns>
        /// <exception cref="BlDoesNotExistException">Thrown when the call is not found in the system.</exception>
        public BO.Call GetCallDetails(int callId)
        {
            lock (AdminManager.BlMutex)
            {
                var callDO = _dal.Call.Read(callId)
                ?? throw new BlDoesNotExistException($"No call found with ID: {callId}");

                var assignmentsDO = _dal.Assignment.ReadAll()
                    .Where(assignment => assignment.CallId == callId);

                var assignmentsBO = assignmentsDO.Select(assignment =>
                {
                    var volunteer = _dal.Volunteer.Read(assignment.VolunteerId);

                    return new BO.CallAssignInList
                    {
                        VolunteerId = assignment.VolunteerId,
                        VolunteerName = volunteer?.Name ?? "Unknown",
                        StartTime = assignment.StartTime,
                        EndTime = assignment.EndTime,
                        EndType = assignment.EndType != null
                            ? (BO.EndType)assignment.EndType
                            : BO.EndType.Completed
                    };
                }).ToList();

                return new BO.Call
                {
                    Id = callDO.Id,
                    CallType = (BO.CallType)callDO.CallType,
                    Address = callDO.Address,
                    Latitude = callDO.Latitude,
                    Longitude = callDO.Longitude,
                    StartTime = callDO.StartTime,
                    Description = callDO.Description,
                    DeadLine = callDO.DeadLine,
                    Assignments = assignmentsBO
                };
            }
        }

        /// <summary>
        /// Retrieves a list of calls with aggregated assignment and volunteer data.
        /// Includes filtering and sorting options, and uses `select new` to construct the result objects.
        /// </summary>
        /// <param name="filterField">Optional field to filter calls by.</param>
        /// <param name="filterValue">Value to filter calls by.</param>
        /// <param name="sortField">Optional sorting field.</param>
        /// <returns>A collection of calls with summarized details.</returns>
        /// <summary>
        /// Retrieves a list of calls with aggregated assignment and volunteer data.
        /// Includes filtering and sorting options, and uses `select new` to construct the result objects.
        /// </summary>
        /// <param name="filterField">Optional field to filter calls by.</param>
        /// <param name="filterValue">Value to filter calls by.</param>
        /// <param name="sortField">Optional sorting field.</param>
        /// <returns>A collection of calls with summarized details.</returns>
        public IEnumerable<BO.CallInList> GetCallList(BO.CallType? callType = null, BO.CallSortField? sortByField = null)
        {
            lock (AdminManager.BlMutex)
            {
                // Retrieve all calls from the DAL
                var callsFromDal = _dal.Call.ReadAll();

                // Filter by CallType if specified
                if (callType.HasValue)
                {
                    callsFromDal = callsFromDal.Where(c =>
                    {
                        if (Enum.IsDefined(typeof(BO.CallType), (BO.CallType)c.CallType))
                        {
                            return (BO.CallType)c.CallType == callType.Value;
                        }
                        return false;
                    });
                }

                // Apply sorting based on the specified field
                if (sortByField.HasValue)
                {
                    switch (sortByField.Value)
                    {
                        case BO.CallSortField.CallType:
                            callsFromDal = callsFromDal.OrderBy(c => c.CallType);
                            break;
                        case BO.CallSortField.StartTime:
                            callsFromDal = callsFromDal.OrderBy(c => c.StartTime);
                            break;
                        case BO.CallSortField.Duration:
                            callsFromDal = callsFromDal.OrderBy(c => c.DeadLine.HasValue
                                ? c.DeadLine.Value - c.StartTime
                                : TimeSpan.MaxValue);
                            break;
                        default:
                            throw new LogicException("Invalid sort field provided.");
                    }
                }
                else
                {
                    // Default sorting by ID
                    callsFromDal = callsFromDal.OrderBy(c => c.Id);
                }

                // Convert the sorted list to a list of BO.CallInList
                IEnumerable<DO.Call> calls = callsFromDal.ToList(); // Convert to List to preserve sorting
                IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

                return calls.Select(c => new BO.CallInList
                {
                    CallId = c.Id,
                    CallType = (BO.CallType)c.CallType,
                    StartTime = c.StartTime,
                    LeftTimeTocomplete =
            c.CallType == DO.CallType.Completed ||
            c.CallType == DO.CallType.SelfCanceled ||
            c.CallType == DO.CallType.Expired ||
            c.CallType == DO.CallType.AdminCanceled
            ? TimeSpan.Zero
            : (c.DeadLine.HasValue ? c.DeadLine.Value - DateTime.Now : (TimeSpan?)null),
                    LastVolunteerName = assignments
            .Where(a => a.CallId == c.Id)
            .OrderByDescending(a => a.EndTime)
            .Select(a => _dal.Volunteer.Read(a.VolunteerId).Name)
            .FirstOrDefault(),
                    Status = (BO.CallType)c.CallType,
                    AssignmentCount = assignments.Count(a => a.CallId == c.Id)
                });
            }
        }

        /// <summary>
        /// Retrieves the quantities of calls grouped by their type. 
        /// Uses LINQ `group by` to count the occurrences of each call type.
        /// </summary>
        /// <returns>An array of integers representing the count of calls for each call type.</returns>
        public int[] GetCallQuantities()
        {
            lock (AdminManager.BlMutex)
            {
                var callsDO = _dal.Call.ReadAll();

                var groupedQuantities = from call in callsDO
                                        group call by (int)call.CallType into callGroup
                                        select new { CallType = callGroup.Key, Count = callGroup.Count() };

                int maxStatus = Enum.GetValues(typeof(DO.CallType)).Cast<int>().Max();

                var quantities = new int[maxStatus + 1];
                foreach (var group in groupedQuantities)
                {
                    quantities[group.CallType] = group.Count;
                }

                return quantities;
            }
        }

        /// <summary>
        /// Retrieves a list of closed calls handled by a specific volunteer, including optional filtering and sorting.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer whose closed calls are to be retrieved.</param>
        /// <param name="callType">Optional filter for the call type.</param>
        /// <param name="sortField">Optional sorting field for the results.</param>
        /// <returns>
        /// A collection of <see cref="BO.ClosedCallInList"/> objects containing details of the closed calls handled by the volunteer.
        /// </returns>
        public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volunteerId, Enum? callType, Enum? sortField)
        {
            lock (AdminManager.BlMutex)
            {
                // Retrieve all assignments and calls from the DAL
                var assignmentsDO = _dal.Assignment.ReadAll();
                var callsDO = _dal.Call.ReadAll();

                // Filter assignments based on volunteerId and closed status
                var closedAssignments = assignmentsDO
                    .Where(a => (volunteerId == 0 || a.VolunteerId == volunteerId) && a.EndTime != null)
                    .ToList();

                // Join assignments with calls to get closed call details
                var closedCalls = closedAssignments
                    .Join(
                        callsDO,
                        assignment => assignment.CallId,
                        call => call.Id,
                        (assignment, call) => new BO.ClosedCallInList
                        {
                            Id = call.Id,
                            CallType = (BO.CallType)call.CallType,
                            Address = call.Address ?? "No address provided",
                            OpenTime = call.StartTime,
                            StartAssignementTime = assignment.StartTime,
                            EndTime = assignment.EndTime,
                            EndType = (BO.EndType)assignment.EndType!
                        })
                    .ToList();


                // Apply optional call type filtering
                if (callType != null)
                {
                    closedCalls = closedCalls.Where(c => c.CallType.Equals(callType)).ToList();
                }

                // Apply optional sorting
                closedCalls = sortField switch
                {
                    BO.ClosedCallSortField.StartTime => closedCalls.OrderBy(c => c.OpenTime).ToList(),
                    BO.ClosedCallSortField.EndTime => closedCalls.OrderBy(c => c.EndTime).ToList(),
                    BO.ClosedCallSortField.ResolutionTime => closedCalls.OrderBy(c => c.StartAssignementTime).ToList(),
                    _ => closedCalls.OrderBy(c => c.Id).ToList()
                };

                return closedCalls;
            }
        }

        /// <summary>
        /// Retrieves a list of open calls that are either "Open" or "OpenAtRisk" for a specific volunteer.
        /// Includes filtering by call type and sorting options. Uses `let` to compute distances between locations.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer.</param>
        /// <param name="callType">Optional filter for the call type.</param>
        /// <param name="sortField">Optional sorting field.</param>
        /// <returns>A collection of open calls with details.</returns>
        /// <summary>
        /// Retrieves a list of open calls that are either "Open" or "OpenAtRisk".
        /// Includes filtering by call type and sorting options. Uses `let` to compute distances between locations.
        /// </summary>
        /// <param name="callType">Optional filter for the call type.</param>
        /// <param name="sortField">Optional sorting field.</param>
        /// <returns>A collection of open calls with details.</returns>
        public IEnumerable<OpenCallInList> GetOpenCalls(int volunteerId, Enum? callType, Enum? sortField)
        {
            lock (AdminManager.BlMutex)
            {
                // Retrieve all calls from the DAL
                var callsDO = _dal.Call.ReadAll();
                //retrieve volunteer info
                var volunteer = _dal.Volunteer.Read(volunteerId);

                // Filter calls to include only open ones (or "open in risk")
                var openCalls = callsDO
                    .Where(call => call.CallType == DO.CallType.Open || call.CallType == DO.CallType.OpenAtRisk)
                    .Select(call => new BO.OpenCallInList
                    {
                        Id = call.Id,
                        CallType = (BO.CallType)call.CallType,
                        Address = call.Address ?? "No address provided",
                        Description = call.Description ?? "No description provided",
                        StartTime = call.StartTime,
                        MaxEndTime = call.DeadLine,
                        Distance = CallManager.CalculateDistance(
                                    call.Latitude,
                                    call.Longitude,
                                    volunteer.Latitude ?? 0,
                                    volunteer.Longitude ?? 0,
                                    (BO.DistanceType)volunteer.DistanceType)
                    })
                    .ToList();
            
                // Apply optional call type filtering
                if (callType != null)
                {
                    openCalls = openCalls.Where(c => c.CallType.Equals(callType)).ToList();
                }

                // Apply optional sorting
                openCalls = sortField switch
                {
                    BO.ClosedCallSortField.StartTime => openCalls.OrderBy(c => c.StartTime).ToList(),
                    BO.ClosedCallSortField.EndTime => openCalls.OrderBy(c => c.MaxEndTime).ToList(),
                    BO.ClosedCallSortField.Distance => openCalls.OrderBy(c => c.Distance).ToList(),
                    _ => openCalls.OrderBy(c => c.Id).ToList()
                };

                return openCalls;
            }
        }

        /// <summary>
        /// Assigns a volunteer to a specific call. 
        /// Ensures that the call is open, unassigned, and not expired before creating the assignment.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer to be assigned to the call.</param>
        /// <param name="callId">The ID of the call to be assigned to the volunteer.</param>
        /// <exception cref="BlDoesNotExistException">Thrown when the call or volunteer does not exist.</exception>
        /// <exception cref="BlInvalidValueException">Thrown when the call is already assigned or expired.</exception>
        public void selectionCall(int volunteerId, int callId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
            {
                var callDO = _dal.Call.Read(callId)
                    ?? throw new BlDoesNotExistException($"No call found with ID: {callId}");

                var volunteerDO = _dal.Volunteer.Read(volunteerId)
                    ?? throw new BlDoesNotExistException($"No volunteer found with ID: {volunteerId}");

                // Check if the volunteer already has an active call
                var activeAssignments = _dal.Assignment.ReadAll()
                    .Where(a => a.VolunteerId == volunteerId && a.EndTime == null);
                if (activeAssignments.Any())
                    throw new BlInvalidValueException("The volunteer already has an active call and cannot take another.");

                var callAssignments = _dal.Assignment.ReadAll()
                    .Where(a => a.CallId == callId && a.EndTime == null);
                if (callAssignments.Any())
                    throw new BlInvalidValueException("This call is already assigned to a volunteer and in progress.");

                if (callDO.DeadLine != null && callDO.DeadLine < DateTime.Now)
                    throw new BlInvalidValueException("This call has expired and can no longer be assigned.");

                var newAssignment = new DO.Assignment
                (
                    Id: _dal.Config.NextAssignmentId,
                    CallId: callId,
                    VolunteerId: volunteerId,
                    StartTime: DateTime.Now,
                    EndTime: null,
                    EndType: null
                );

                _dal.Assignment.Create(newAssignment);

                // Update the call type based on its current state
                var updatedCallType = callDO.CallType == DO.CallType.OpenAtRisk
                    ? DO.CallType.InTreatmentAtRisk
                    : DO.CallType.InTreatment;

                var updatedCall = callDO with { CallType = updatedCallType };

                _dal.Call.Update(updatedCall);

            }
                CallManager.Observers.NotifyItemUpdated(callId);
                CallManager.Observers.NotifyListUpdated();
        }

        /// <summary>
        /// Updates the details of an existing call in the system.
        /// Validates the input and ensures the call details are consistent before updating the database.
        /// </summary>
        /// <param name="call">The updated call object containing the new details.</param>
        /// <exception cref="BlArgumentNullException">Thrown when the provided call object is null.</exception>
        /// <exception cref="BlInvalidAddressException">Thrown when the address is empty or invalid.</exception>
        /// <exception cref="BlInvalidValueException">
        /// Thrown when:
        /// - The call's deadline is earlier than or equal to its start time.
        /// - The latitude and longitude are invalid or do not match the address.
        /// </exception>
        /// <exception cref="BlDoesNotExistException">Thrown when the call does not exist in the system.</exception>
        public async Task UpdateCallAsync(BO.Call call)
        {
            AdminManager.ThrowOnSimulatorIsRunning();

            await _semaphore.WaitAsync(); // Acquire the semaphore
            try
            {
                if (call == null)
                    throw new BlArgumentNullException("Call cannot be null.");

                if (string.IsNullOrWhiteSpace(call.Address))
                    throw new BlInvalidAddressException("Address cannot be empty or whitespace.");

                if (call.StartTime >= call.DeadLine)
                    throw new BlInvalidValueException("The deadline must be later than the start time.");

                if (!await VolunteerManager.ValidAddressAsync(call.Address))
                    throw new BlInvalidValueException("Invalid address.");

                // Ensure coordinates are not null and valid
                if (!call.Latitude.HasValue || !call.Longitude.HasValue)
                    throw new BlNullPropertyException("Coordinates cannot be null.");

                // Validate coordinates with partial address
                var closestCoordinates = await VolunteerManager.GetClosestCoordinatesAsync(call.Address, call.Latitude.Value, call.Longitude.Value);
                if (closestCoordinates == null)
                    throw new BlInvalidValueException("Coordinates do not match the address.");

                var callDO = new DO.Call
                (
                    Id: call.Id,
                    CallType: (DO.CallType)call.CallType,
                    Address: call.Address,
                    Latitude: call.Latitude.Value,
                    Longitude: call.Longitude.Value,
                    StartTime: call.StartTime,
                    Description: call.Description,
                    DeadLine: call.DeadLine
                );

                // Update the call in the DAL
                _dal.Call.Update(callDO);
                CallManager.Observers.NotifyItemUpdated(callDO.Id);
                CallManager.Observers.NotifyListUpdated();
            }
            finally
            {
                _semaphore.Release(); // Release the semaphore
            }

            // Notify nearby volunteers asynchronously if needed
            await Helpers.CallManager.NotifyNearbyVolunteersAsync(call);
        }

        /// <summary>
        /// get the history of the call assignement by id
        public IEnumerable<BO.CallAssignInList> GetAssignmentsByCallId(int callId)
        {
            lock (AdminManager.BlMutex)
            {
                // Retrieve all assignments related to the call ID
                var assignments = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId);

                // Transform the data to match the CallAssignInList model
                return assignments.Select(a =>
                {
                    var volunteer = _dal.Volunteer.Read(a.VolunteerId);
                    return new CallAssignInList
                    {
                        VolunteerId = a.VolunteerId,
                        VolunteerName = volunteer?.Name ?? "Unknown",
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        EndType = a.EndType != null ? (BO.EndType)a.EndType : null
                    };
                }).ToList();
            }
        }

        /// <summary>
        /// Retrieves the assignment ID for a specific call and volunteer.
        /// </summary>
        public int GetAssignmentIdByCallId(int callId, int volunteerId)
        {
            lock (AdminManager.BlMutex)
            {
                var assignment = _dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.CallId == callId && a.VolunteerId == volunteerId);

                if (assignment == null)
                    throw new BlDoesNotExistException($"No assignment found for CallId: {callId} and VolunteerId: {volunteerId}");

                return assignment.Id;
            }
        }


        ///<sumary>
        ///observable
        /// </sumary>
        public void AddObserver(Action listObserver) =>
                CallManager.Observers.AddListObserver(listObserver);
        public void AddObserver(int id, Action observer) =>
                CallManager.Observers.AddObserver(id, observer);
        public void RemoveObserver(Action listObserver) =>
                CallManager.Observers.RemoveListObserver(listObserver);
        public void RemoveObserver(int id, Action observer) =>
                CallManager.Observers.RemoveObserver(id, observer);



        public IEnumerable<BO.Call> CallHistoryByVolunteerId(int volunteerId)
        {
            lock (AdminManager.BlMutex)
            {
                try
                {
                    // Fetch all assignments for the volunteer
                    var assignments = _dal.Assignment.ReadAll()
                        .Where(a => a.VolunteerId == volunteerId)
                        .Select(a => a.CallId);

                    // Retrieve all calls associated with the volunteer's assignments
                    var calls = _dal.Call.ReadAll()
                        .Where(c => assignments.Contains(c.Id))
                        .Select(c => new BO.Call
                        {
                            Id = c.Id,
                            CallType = (BO.CallType)c.CallType,
                            StartTime = c.StartTime,
                            Description = c.Description,
                            DeadLine = c.DeadLine,
                            Address = c.Address
                        });

                    return calls;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to retrieve call history.", ex);
                }
            }
        }


        public IEnumerable<int> GetCallIdsByVolunteer(int volunteerId)
        {
            lock (AdminManager.BlMutex)
            {
                // Assuming _dal is properly initialized and accessible
                var callHistory = _dal.Assignment.ReadAll(call => call.VolunteerId == volunteerId);

                // Extracting call IDs from the call history
                var callIds = callHistory.Select(call => call.Id);

                return callIds;
            }
        }


        public double _CalculateDistance(int callId, int volunteerId)
        {
            lock (AdminManager.BlMutex)
            {
                BO.Call _call = GetCallDetails(callId);
                DO.Volunteer _volunteer = _dal.Volunteer.Read(volunteerId);

                double volunteerLatitude = _volunteer.Latitude ?? throw new System.ArgumentNullException(nameof(_volunteer.Latitude));
                double volunteerLongitude = _volunteer.Longitude ?? throw new System.ArgumentNullException(nameof(_volunteer.Longitude));
                double callLatitude = _call.Latitude ?? throw new System.ArgumentNullException(nameof(_call.Latitude));
                double callLongitude = _call.Longitude ?? throw new System.ArgumentNullException(nameof(_call.Longitude));
                return CallManager.CalculateDistance(volunteerLatitude, volunteerLongitude, callLatitude, callLongitude);
            }
        }
    }
}
