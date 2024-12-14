using BO;
using DO;
using Helpers;

namespace BlImplementation
{
    internal class CallImplementation : BlApi.ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        /// <summary>
        /// Adds a new call to the system. 
        /// Validates the input, ensures the address and coordinates are correct, and converts the BO.Call object to DO.Call before saving it in the database.
        /// </summary>
        /// <param name="newCall">The call object containing the details of the new call to be added.</param>
        /// <exception cref="BlArgumentNullException">Thrown when the provided call object is null.</exception>
        /// <exception cref="BlInvalidAddressException">Thrown when the address is empty or invalid.</exception>
        /// <exception cref="BlInvalidCoordinateException">Thrown when latitude and longitude are null or do not match the address.</exception>
        /// <exception cref="BlDoesNotExistException">Thrown when the call already exists in the system.</exception>
        public void AddCall(BO.Call newCall)
        {
            if (newCall == null)
                throw new BlArgumentNullException("Call cannot be null.");

            if (string.IsNullOrWhiteSpace(newCall.Address))
                throw new BlInvalidAddressException("Address cannot be empty.");

            if (newCall.Latitude == null || newCall.Longitude == null)
                throw new BlInvalidCoordinateException("Latitude and Longitude cannot be null.");

            CallManager.ValidAddress(newCall.Address);
            CallManager.AreCoordinatesMatching(newCall.Address, newCall.Latitude.Value, newCall.Longitude.Value);

            var callDO = new DO.Call
            (
                Id: newCall.Id,
                CallType: (DO.CallType)newCall.CallType,
                Address: newCall.Address,
                Latitude: newCall.Latitude.Value,
                Longitude: newCall.Longitude.Value,
                StartTime: newCall.StartTime,
                Description: newCall.Description,
                DeadLine: newCall.DeadLine
            );

            _dal.Call.Create(callDO);
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

        /// <summary>
        /// Retrieves detailed information about a specific call, including its assignments and volunteer information.
        /// </summary>
        /// <param name="callId">The ID of the call to retrieve.</param>
        /// <returns>A <see cref="BO.Call"/> object containing the call details and a list of its assignments.</returns>
        /// <exception cref="BlDoesNotExistException">Thrown when the call is not found in the system.</exception>
        public BO.Call GetCallDetails(int callId)
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
                LeftTimeToExpire = c.DeadLine.HasValue ? c.DeadLine.Value - DateTime.Now : null,
                LastVolunteerName = assignments.Where(a => a.CallId == c.Id).OrderByDescending(a => a.EndTime).Select(a => _dal.Volunteer.Read(a.VolunteerId).Name).FirstOrDefault(),
                LeftTimeTocomplete = c.DeadLine.HasValue ? c.DeadLine.Value - DateTime.Now : null,
                Status = (BO.CallType)c.CallType,
                AssignmentCount = assignments.Count(a => a.CallId == c.Id)
            });
        }

        /// <summary>
        /// Retrieves the quantities of calls grouped by their type. 
        /// Uses LINQ `group by` to count the occurrences of each call type.
        /// </summary>
        /// <returns>An array of integers representing the count of calls for each call type.</returns>
        public int[] GetCallQuantities()
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
            // Retrieve all calls from the DAL
            var callsDO = _dal.Call.ReadAll();
            //retrieve volunteer info
            var volunteer = _dal.Volunteer.Read(volunteerId);

            // Filter calls to include only open ones (or "open in risk")
            var openCalls = callsDO
                .Where(call => call.CallType is ((int)DO.CallType.Open) or (DO.CallType)(int)DO.CallType.OpenAtRisk)
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
            // Retrieve the call from the DAL by callId
            var callDO = _dal.Call.Read(callId)
                ?? throw new BlDoesNotExistException($"No call found with ID: {callId}");

            // Retrieve the volunteer from the DAL by volunteerId
            var volunteerDO = _dal.Volunteer.Read(volunteerId)
                ?? throw new BlDoesNotExistException($"No volunteer found with ID: {volunteerId}");

            // Check if the call has already been assigned and is open
            var assignments = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId && a.EndTime == null);
            if (assignments.Any())
                throw new BlInvalidValueException("This call is already assigned to a volunteer and in progress.");

            // Check if the call is expired
            if (callDO.DeadLine != null && callDO.DeadLine < DateTime.Now)
                throw new BlInvalidValueException("This call has expired and can no longer be assigned.");

            // Create a new assignment
            var newAssignment = new DO.Assignment
            (
                Id: 0, // Let the DAL auto-generate the ID if supported
                CallId: callId,
                VolunteerId: volunteerId,
                StartTime: DateTime.Now, // System time for assignment start
                EndTime: null,           // Not completed yet
                EndType: null            // No end type as it's not completed
            );


            // Add the new assignment to the DAL
            _dal.Assignment.Create(newAssignment);


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
        public void UpdateCall(BO.Call call)
        {
            if (call == null)
                throw new BlArgumentNullException("Call cannot be null.");

            if (string.IsNullOrWhiteSpace(call.Address))
                throw new BlInvalidAddressException("Address cannot be empty or whitespace.");

            if (call.StartTime >= call.DeadLine)
                throw new BlInvalidValueException("The deadline must be later than the start time.");

            if (call.Latitude == null || call.Longitude == null)
                throw new BlInvalidValueException("Latitude and Longitude cannot be null.");

            if (!CallManager.AreCoordinatesMatching(call.Address, call.Latitude.Value, call.Longitude.Value))
                throw new BlInvalidValueException("Invalid address coordinates (latitude or longitude).");

            if (!CallManager.ValidAddress(call.Address))
                throw new BlInvalidAddressException("Invalid address format.");

            // Convert BO.Call to DO.Call
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

            // Update the call using the DAL
            _dal.Call.Update(callDO);

        }

    }
}
