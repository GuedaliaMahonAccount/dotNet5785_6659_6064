using BlApi;
using Helpers;

namespace BlImplementation
{

    internal class CallImplementation : ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void AddCall(BO.Call newCall)
        {
            try
            {
                // Validate the input (you can add more validations as required)
                if (newCall == null)
                    throw new ArgumentNullException(nameof(newCall), "Call cannot be null.");

                if (string.IsNullOrWhiteSpace(newCall.Address))
                    throw new BO.LogicException("Address cannot be empty.");

                // Convert BO.Call to DO.Call
                var callDO = new DO.Call
                (
                    Id: newCall.Id,
                    CallType: (DO.CallType)newCall.CallType,
                    Address: newCall.Address,
                    Latitude: newCall.Latitude,
                    Longitude: newCall.Longitude,
                    StartTime: newCall.StartTime,
                    Description: newCall.Description,
                    DeadLine: newCall.DeadLine
                );

                // Add the call using the DAL
                _dal.Call.Create(callDO);
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.LogicException("Call already exists in the system.", ex);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("Failed to add call.", ex);
            }
        }

        public void CancelCall(int requesterId, int assignmentId)
        {
            try
            {
                // Retrieve the assignment from DAL by assignmentId
                var assignmentDO = _dal.Assignment.Read(assignmentId);

                if (assignmentDO == null)
                    throw new BO.LogicException($"No assignment found with ID: {assignmentId}");

                // Check if the requester has the authorization to cancel
                if (!CallManager.IsRequesterAuthorizedToCancel(requesterId, assignmentDO.VolunteerId))
                {
                    throw new BO.LogicException("Requester is not authorized to cancel this assignment.");
                }

                // Check if the assignment is still open (not completed and not expired)
                if (assignmentDO.EndTime != null)
                {
                    throw new BO.LogicException("Cannot cancel an assignment that is already completed.");
                }

                if (assignmentDO.EndTime != null && assignmentDO.EndTime < DateTime.Now)
                {
                    throw new BO.LogicException("Cannot cancel an assignment that has expired.");
                }

                // Determine the cancellation type
                var cancellationType = requesterId == assignmentDO.VolunteerId
                    ? DO.EndType.SelfCanceled
                    : DO.EndType.AdminCanceled;

                // Update the assignment with the cancellation details
                var updatedAssignment = assignmentDO with
                {
                    EndTime = DateTime.Now, // System clock time
                    EndType = cancellationType
                };

                _dal.Assignment.Update(updatedAssignment);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.LogicException($"No assignment found with ID: {assignmentId}", ex);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while canceling the assignment.", ex);
            }
        }

        public void CompleteCall(int volunteerId, int assignmentId)
        {
            try
            {
                // Retrieve the assignment from the DAL by assignmentId
                var assignmentDO = _dal.Assignment.Read(assignmentId);

                if (assignmentDO == null)
                    throw new BO.LogicException($"No assignment found with ID: {assignmentId}");

                // Check if the volunteer has authorization to complete the assignment
                if (volunteerId != assignmentDO.VolunteerId)
                    throw new BO.LogicException("Only the assigned volunteer can complete this assignment.");

                // Check if the assignment is still open (not completed, not canceled, not expired)
                if (assignmentDO.EndTime != null)
                    throw new BO.LogicException("Cannot complete an assignment that is already completed or canceled.");

                if (assignmentDO.EndTime != null && assignmentDO.EndTime < DateTime.Now)
                    throw new BO.LogicException("Cannot complete an assignment that has expired.");

                // Update the assignment with completion details
                var updatedAssignment = assignmentDO with
                {
                    EndTime = DateTime.Now, // Set the end time to the current system time
                    EndType = BO.EndType.Completed // Set the end type to "Completed"
                };

                // Update the assignment in the DAL
                _dal.Assignment.Update(updatedAssignment);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.LogicException($"No assignment found with ID: {assignmentId}", ex);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while completing the assignment.", ex);
            }
        }

        public void DeleteCall(int callId)
        {
            try
            {
                // Retrieve the call from the DAL by callId
                var callDO = _dal.Call.Read(callId);

                if (callDO == null)
                    throw new BO.LogicException($"No call found with ID: {callId}");

                // Check if the call is open and has never been assigned to a volunteer
                if (callDO.CallType != DO.CallType.Open)
                {
                    throw new BO.LogicException("Cannot delete a call that is not in 'Open' status.");
                }

                // Check if any assignments exist for this call
                var assignments = _dal.Assignment.ReadAll()
                    .Where(assignment => assignment.CallId == callId);

                if (assignments.Any())
                {
                    throw new BO.LogicException("Cannot delete a call that has been assigned to a volunteer.");
                }

                // Delete the call from the DAL
                _dal.Call.Delete(callId);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.LogicException($"No call found with ID: {callId}", ex);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while deleting the call.", ex);
            }
        }

        public BO.Call GetCallDetails(int callId)
        {
            try
            {
                // Retrieve the call from the DAL by callId
                var callDO = _dal.Call.Read(callId);

                if (callDO == null)
                    throw new BO.LogicException($"No call found with ID: {callId}");

                // Retrieve assignments for the call
                var assignmentsDO = _dal.Assignment.ReadAll()
                    .Where(assignment => assignment.CallId == callId);

                // Map DO assignments to BO.CallAssignInList
                var assignmentsBO = assignmentsDO.Select(assignment =>
                {
                    var volunteer = _dal.Volunteer.Read(assignment.VolunteerId);

                    return new BO.CallAssignInList
                    {
                        VolunteerId = assignment.VolunteerId,
                        VolunteerName = volunteer?.Name ?? "Unknown", // Retrieve volunteer name or fallback to "Unknown"
                        StartTime = assignment.StartTime,
                        EndTime = assignment.EndTime,
                        EndType = assignment.EndType != null
                            ? (BO.EndType)assignment.EndType
                            : BO.EndType.Completed // Default to a valid EndType value, e.g., Completed
                    };
                }).ToList();

                // Map DO.Call to BO.Call
                var callBO = new BO.Call
                {
                    Id = callDO.Id,
                    CallType = (BO.CallType)callDO.CallType,
                    Address = callDO.Address,
                    Latitude = callDO.Latitude,
                    Longitude = callDO.Longitude,
                    StartTime = callDO.StartTime,
                    Description = callDO.Description,
                    DeadLine = callDO.DeadLine,
                    Assignments = assignmentsBO // Include the mapped assignments
                };

                return callBO;
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.LogicException($"No call found with ID: {callId}", ex);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while retrieving call details.", ex);
            }
        }

        public IEnumerable<BO.CallInList> GetCallList(Enum? filterField, object? filterValue, Enum? sortField)
        {
            try
            {
                // Retrieve all calls and assignments from the DAL
                var callsDO = _dal.Call.ReadAll();
                var assignmentsDO = _dal.Assignment.ReadAll();

                // Map calls to BO.CallInList with aggregated assignment data
                var callsInList = callsDO.Select(call =>
                {
                    var relatedAssignments = assignmentsDO.Where(a => a.CallId == call.Id);
                    var latestAssignment = relatedAssignments
                        .OrderByDescending(a => a.StartTime)
                        .FirstOrDefault();

                    return new BO.CallInList
                    {
                        CallId = call.Id,
                        CallType = (BO.CallType)call.CallType,
                        StartTime = call.StartTime,
                        Duration = latestAssignment?.EndTime.HasValue == true
                            ? latestAssignment.EndTime.Value - call.StartTime
                            : TimeSpan.Zero,
                        LastVolunteerName = latestAssignment != null
                            ? _dal.Volunteer.Read(latestAssignment.VolunteerId)?.Name ?? "Unknown"
                            : "None",
                        LastCompletionDuration = latestAssignment?.EndTime.HasValue == true
                            ? DateTime.Now - latestAssignment.EndTime.Value
                            : TimeSpan.Zero,
                        Status = latestAssignment == null
                            ? BO.CallType.Open
                            : latestAssignment.EndTime == null
                                ? BO.CallType.InTreatment
                                : BO.CallType.Closed,
                        AssignmentCount = relatedAssignments.Count()
                    };
                });

                // Apply filtering
                if (filterField != null && filterValue != null)
                {
                    callsInList = filterField switch
                    {
                        BO.CallFilterField.CallType => callsInList.Where(call => call.CallType.Equals(filterValue)),
                        BO.CallFilterField.Status => callsInList.Where(call => call.Status.Equals(filterValue)),
                        _ => callsInList
                    };
                }

                // Apply sorting
                if (sortField == null)
                {
                    callsInList = callsInList.OrderBy(call => call.CallId);
                }
                else
                {
                    callsInList = sortField switch
                    {
                        BO.CallSortField.CallType => callsInList.OrderBy(call => call.CallType),
                        BO.CallSortField.StartTime => callsInList.OrderBy(call => call.StartTime),
                        BO.CallSortField.Duration => callsInList.OrderBy(call => call.Duration),
                        _ => callsInList.OrderBy(call => call.CallId)
                    };
                }

                return callsInList;
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while retrieving the call list.", ex);
            }
        }

        public int[] GetCallQuantities()
        {
            try
            {
                // Retrieve all calls from the DAL
                var callsDO = _dal.Call.ReadAll();

                // Group calls by their CallType and count the occurrences
                var groupedQuantities = callsDO
                    .GroupBy(call => (int)call.CallType) // Group by CallType enum value
                    .ToDictionary(group => group.Key, group => group.Count());

                // Determine the maximum CallType value to ensure the array covers all statuses
                int maxStatus = Enum.GetValues(typeof(DO.CallType)).Cast<int>().Max();

                // Create an array of size maxStatus + 1 and populate it with quantities
                var quantities = new int[maxStatus + 1];
                foreach (var kvp in groupedQuantities)
                {
                    quantities[kvp.Key] = kvp.Value;
                }

                return quantities;
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while retrieving call quantities.", ex);
            }
        }

        public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volunteerId, Enum? callType, Enum? sortField)
        {
            try
            {
                // Retrieve all assignments and calls from the DAL
                var assignmentsDO = _dal.Assignment.ReadAll();
                var callsDO = _dal.Call.ReadAll();

                // Filter assignments for the specific volunteer and closed assignments (EndTime is not null)
                var closedAssignments = assignmentsDO
                    .Where(a => a.VolunteerId == volunteerId && a.EndTime != null);

                // Join assignments with calls to construct ClosedCallInList
                var closedCalls = closedAssignments
                    .Join(
                        callsDO,
                        assignment => assignment.CallId,
                        call => call.Id,
                        (assignment, call) => new BO.ClosedCallInList
                        {
                            Id = call.Id,
                            CallType = (BO.CallType)call.CallType,
                            Description = call.Description ?? "No description provided",
                            StartTime = call.StartTime,
                            EndTime = assignment.EndTime.Value, // EndTime is guaranteed to be non-null
                            ResolutionTime = assignment.EndTime,
                            EndType = (BO.EndType)assignment.EndType!
                        });

                // Apply filtering by CallType if provided
                if (callType != null)
                {
                    closedCalls = closedCalls.Where(c => c.CallType.Equals(callType));
                }

                // Apply sorting
                if (sortField == null)
                {
                    closedCalls = closedCalls.OrderBy(c => c.Id); // Default sorting by call ID
                }
                else
                {
                    closedCalls = sortField switch
                    {
                        BO.ClosedCallSortField.StartTime => closedCalls.OrderBy(c => c.StartTime),
                        BO.ClosedCallSortField.EndTime => closedCalls.OrderBy(c => c.EndTime),
                        BO.ClosedCallSortField.ResolutionTime => closedCalls.OrderBy(c => c.ResolutionTime),
                        _ => closedCalls.OrderBy(c => c.Id)
                    };
                }

                return closedCalls;
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while retrieving closed calls.", ex);
            }
        }

        public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, Enum? callType, Enum? sortField)
        {
            try
            {
                // Retrieve all calls and volunteer details from the DAL
                var callsDO = _dal.Call.ReadAll();
                var volunteerDO = _dal.Volunteer.Read(volunteerId);

                if (volunteerDO == null)
                    throw new BO.LogicException($"Volunteer with ID {volunteerId} not found.");

                // Filter open calls ("Open" or "OpenAtRisk" status)
                var openCalls = callsDO.Where(call =>
                    call.CallType == DO.CallType.Open || call.CallType == DO.CallType.OpenAtRisk);

                // Calculate distances and map to BO.OpenCallInList
                var openCallsWithDetails = openCalls.Select(call =>
                {
                    var distance = CallManager.CalculateDistance(
                        volunteerDO.Latitude ?? 0, volunteerDO.Longitude ?? 0,
                        call.Latitude, call.Longitude
                    );

                    return new BO.OpenCallInList
                    {
                        Id = call.Id,
                        CallType = (BO.CallType)call.CallType,
                        Description = call.Description ?? "No description provided",
                        Address = call.Address ?? "Unknown",
                        StartTime = call.StartTime,
                        MaxEndTime = call.DeadLine ?? DateTime.MaxValue,
                        Distance = distance
                    };
                });

                // Filter by callType if provided
                if (callType != null)
                {
                    openCallsWithDetails = openCallsWithDetails.Where(c => c.CallType.Equals(callType));
                }

                // Sort the results
                if (sortField == null)
                {
                    openCallsWithDetails = openCallsWithDetails.OrderBy(c => c.Id); // Default sorting by call ID
                }
                else
                {
                    openCallsWithDetails = sortField switch
                    {
                        BO.OpenCallSortField.StartTime => openCallsWithDetails.OrderBy(c => c.StartTime),
                        BO.OpenCallSortField.Distance => openCallsWithDetails.OrderBy(c => c.Distance),
                        _ => openCallsWithDetails.OrderBy(c => c.Id)
                    };
                }

                return openCallsWithDetails;
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while retrieving open calls.", ex);
            }
        }

        public void selectionCall(int volunteerId, int callId)
        {
            try
            {
                // Retrieve the call from the DAL by callId
                var callDO = _dal.Call.Read(callId);
                if (callDO == null)
                    throw new BO.LogicException($"No call found with ID: {callId}");

                // Retrieve the volunteer from the DAL by volunteerId
                var volunteerDO = _dal.Volunteer.Read(volunteerId);
                if (volunteerDO == null)
                    throw new BO.LogicException($"No volunteer found with ID: {volunteerId}");

                // Check if the call has already been assigned and is open
                var assignments = _dal.Assignment.ReadAll()
                    .Where(a => a.CallId == callId && a.EndTime == null);
                if (assignments.Any())
                    throw new BO.LogicException("This call is already assigned to a volunteer and in progress.");

                // Check if the call is expired
                if (callDO.DeadLine != null && callDO.DeadLine < DateTime.Now)
                    throw new BO.LogicException("This call has expired and can no longer be assigned.");

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
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.LogicException("Failed to find required data for assignment creation.", ex);
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.LogicException("This assignment already exists.", ex);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while selecting the call for treatment.", ex);
            }
        }

        public void UpdateCall(BO.Call call)
        {
            try
            {
                // Validate input
                if (call == null)
                    throw new ArgumentNullException(nameof(call), "Call cannot be null.");

                if (string.IsNullOrWhiteSpace(call.Address))
                    throw new BO.LogicException("Address cannot be empty or whitespace.");

                if (call.StartTime >= call.DeadLine)
                    throw new BO.LogicException("The deadline must be later than the start time.");

                // Validate latitude and longitude for the address
                if (!CallManager.IsValidCoordinate(call.Latitude, call.Longitude))
                    throw new BO.LogicException("Invalid address coordinates (latitude or longitude).");

                // Convert BO.Call to DO.Call
                var callDO = new DO.Call
                (
                    Id: call.Id,
                    CallType: (DO.CallType)call.CallType,
                    Address: call.Address,
                    Latitude: call.Latitude,
                    Longitude: call.Longitude,
                    StartTime: call.StartTime,
                    Description: call.Description,
                    DeadLine: call.DeadLine
                );

                // Update the call using the DAL
                _dal.Call.Update(callDO);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.LogicException($"Call with ID {call.Id} does not exist.", ex);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while updating the call.", ex);
            }
        }

    }
}
