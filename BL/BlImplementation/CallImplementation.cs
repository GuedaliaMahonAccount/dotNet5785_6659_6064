using BlApi;
using Helpers;

namespace BlImplementation
{

    internal class CallImplementation : ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;


        /// <summary>
        /// Adds a new call to the system. 
        /// Validates the input, ensures the address and coordinates are correct, and converts the BO.Call object to DO.Call before saving it in the database.
        /// </summary>
        /// <param name="newCall">The call object containing the details of the new call to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided call object is null.</exception>
        /// <exception cref="BO.LogicException">
        /// Thrown when:
        /// - The address is empty or invalid.
        /// - The address and coordinates do not match.
        /// - The call already exists in the system.
        /// - Other unexpected errors occur during the addition process.
        /// </exception>
        public void AddCall(BO.Call newCall)
        {
            try
            {
                // Validate the input (you can add more validations as required)
                if (newCall == null)
                    throw new ArgumentNullException(nameof(newCall), "Call cannot be null.");

                if (string.IsNullOrWhiteSpace(newCall.Address))
                    throw new BO.LogicException("Address cannot be empty.");

                if (newCall.Latitude == null || newCall.Longitude == null)
                    throw new BO.LogicException("Latitude and Longitude cannot be null.");

                // Validate the address and coordinates
                CallManager.ValidAddress(newCall.Address);
                CallManager.AreCoordinatesMatching(newCall.Address, newCall.Latitude.Value, newCall.Longitude.Value);

                // Convert BO.Call to DO.Call
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

        /// <summary>
        /// Cancels an assignment based on the requester's authorization and the assignment's current status. 
        /// Updates the assignment with the cancellation details if the operation is valid.
        /// </summary>
        /// <param name="requesterId">The ID of the person requesting the cancellation (can be a volunteer or an admin).</param>
        /// <param name="assignmentId">The ID of the assignment to be canceled.</param>
        /// <exception cref="BO.LogicException">
        /// Thrown when:
        /// - No assignment is found with the provided ID.
        /// - The requester is not authorized to cancel the assignment.
        /// - The assignment is already completed or expired.
        /// - Other unexpected errors occur during the cancellation process.
        /// </exception>
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

        /// <summary>
        /// Marks an assignment as completed by the assigned volunteer. 
        /// Validates the assignment's status and the volunteer's authorization before updating the assignment's completion details.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer completing the assignment.</param>
        /// <param name="assignmentId">The ID of the assignment to be completed.</param>
        /// <exception cref="BO.LogicException">
        /// Thrown when:
        /// - The assignment is not found in the system.
        /// - The volunteer is not authorized to complete the assignment.
        /// - The assignment is already completed, canceled, or expired.
        /// - Other unexpected errors occur during the completion process.
        /// </exception>
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

        /// <summary>
        /// Deletes a call from the system. 
        /// Ensures the call is in an open state and has not been assigned to any volunteer before deletion.
        /// </summary>
        /// <param name="callId">The ID of the call to be deleted.</param>
        /// <exception cref="BO.LogicException">
        /// Thrown when:
        /// - The call is not found in the system.
        /// - The call is not in the 'Open' status.
        /// - The call has been assigned to one or more volunteers.
        /// - Other unexpected errors occur during the deletion process.
        /// </exception>
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

        /// <summary>
        /// Retrieves detailed information about a specific call, including its assignments and volunteer information.
        /// </summary>
        /// <param name="callId">The ID of the call to retrieve.</param>
        /// <returns>A <see cref="BO.Call"/> object containing the call details and a list of its assignments.</returns>
        /// <exception cref="BO.LogicException">
        /// Thrown when:
        /// - The call is not found in the system.
        /// - Other unexpected errors occur during the retrieval process.
        /// </exception>
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

        /// <summary>
        /// Retrieves a list of calls with aggregated assignment and volunteer data.
        /// Includes filtering and sorting options, and uses `select new` to construct the result objects.
        /// </summary>
        /// <param name="filterField">Optional field to filter calls by.</param>
        /// <param name="filterValue">Value to filter calls by.</param>
        /// <param name="sortField">Optional sorting field.</param>
        /// <returns>A collection of calls with summarized details.</returns>
        public IEnumerable<BO.CallInList> GetCallList(Enum? filterField, object? filterValue, Enum? sortField)
        {
            try
            {
                var callsDO = _dal.Call.ReadAll();
                var assignmentsDO = _dal.Assignment.ReadAll();

                var callsInList = from call in callsDO
                                  let relatedAssignments = assignmentsDO.Where(a => a.CallId == call.Id)
                                  let latestAssignment = relatedAssignments
                                      .OrderByDescending(a => a.StartTime)
                                      .FirstOrDefault()
                                  select new BO.CallInList
                                  {
                                      CallId = call.Id,
                                      CallType = (BO.CallType)call.CallType,
                                      StartTime = call.StartTime,
                                      LeftTimeToExpire = latestAssignment?.EndTime.HasValue == true
                                          ? latestAssignment.EndTime.Value - call.StartTime
                                          : TimeSpan.Zero,
                                      LastVolunteerName = latestAssignment != null
                                          ? _dal.Volunteer.Read(latestAssignment.VolunteerId)?.Name ?? "Unknown"
                                          : "None",
                                      LeftTimeTocomplete = latestAssignment?.EndTime.HasValue == true
                                          ? DateTime.Now - latestAssignment.EndTime.Value
                                          : TimeSpan.Zero,
                                      Status = latestAssignment == null
                                          ? BO.CallType.Open
                                          : latestAssignment.EndTime == null
                                              ? BO.CallType.InTreatment
                                              : BO.CallType.Closed,
                                      AssignmentCount = relatedAssignments.Count()
                                  };

                if (filterField != null && filterValue != null)
                {
                    callsInList = filterField switch
                    {
                        BO.CallFilterField.CallType => callsInList.Where(call => call.CallType.Equals(filterValue)),
                        BO.CallFilterField.Status => callsInList.Where(call => call.Status.Equals(filterValue)),
                        _ => callsInList
                    };
                }

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
                        BO.CallSortField.Duration => callsInList.OrderBy(call => call.LeftTimeToExpire),
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

        /// <summary>
        /// Retrieves the quantities of calls grouped by their type. 
        /// Uses LINQ `group by` to count the occurrences of each call type.
        /// </summary>
        /// <returns>An array of integers representing the count of calls for each call type.</returns>
        public int[] GetCallQuantities()
        {
            try
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
            catch (Exception ex)
            {
                throw new BO.LogicException("An error occurred while retrieving call quantities.", ex);
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
        /// <exception cref="BO.LogicException">
        /// Thrown when an error occurs during the retrieval or processing of the closed calls.
        /// </exception>
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
                            Address = call.Address ?? "No description provided",
                            OpenTime = call.StartTime,
                            StartAssignementTime = assignment.StartTime, // EndTime is guaranteed to be non-null
                            EndTime = assignment.EndTime,
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
                        BO.ClosedCallSortField.StartTime => closedCalls.OrderBy(c => c.OpenTime),
                        BO.ClosedCallSortField.EndTime => closedCalls.OrderBy(c => c.EndTime),
                        BO.ClosedCallSortField.ResolutionTime => closedCalls.OrderBy(c => c.StartAssignementTime),
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

        /// <summary>
        /// Retrieves a list of open calls that are either "Open" or "OpenAtRisk" for a specific volunteer.
        /// Includes filtering by call type and sorting options. Uses `let` to compute distances between locations.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer.</param>
        /// <param name="callType">Optional filter for the call type.</param>
        /// <param name="sortField">Optional sorting field.</param>
        /// <returns>A collection of open calls with details.</returns>
        public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, Enum? callType, Enum? sortField)
        {
            try
            {
                var callsDO = _dal.Call.ReadAll();
                var volunteerDO = _dal.Volunteer.Read(volunteerId);

                if (volunteerDO == null)
                    throw new BO.LogicException($"Volunteer with ID {volunteerId} not found.");

                var openCallsWithDetails = from call in callsDO
                                           where call.CallType == DO.CallType.Open || call.CallType == DO.CallType.OpenAtRisk
                                           let distance = CallManager.CalculateDistance(
                                               volunteerDO.Latitude ?? 0, volunteerDO.Longitude ?? 0,
                                               call.Latitude, call.Longitude
                                           )
                                           select new BO.OpenCallInList
                                           {
                                               Id = call.Id,
                                               CallType = (BO.CallType)call.CallType,
                                               Description = call.Description ?? "No description provided",
                                               Address = call.Address ?? "Unknown",
                                               StartTime = call.StartTime,
                                               MaxEndTime = call.DeadLine ?? DateTime.MaxValue,
                                               Distance = distance
                                           };

                if (callType != null)
                {
                    openCallsWithDetails = openCallsWithDetails.Where(c => c.CallType.Equals(callType));
                }

                if (sortField == null)
                {
                    openCallsWithDetails = openCallsWithDetails.OrderBy(c => c.Id);
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

        /// <summary>
        /// Assigns a volunteer to a specific call. 
        /// Ensures that the call is open, unassigned, and not expired before creating the assignment.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer to be assigned to the call.</param>
        /// <param name="callId">The ID of the call to be assigned to the volunteer.</param>
        /// <exception cref="BO.LogicException">
        /// Thrown when:
        /// - The call does not exist.
        /// - The volunteer does not exist.
        /// - The call is already assigned or in progress.
        /// - The call is expired.
        /// - Other unexpected errors occur during the assignment process.
        /// </exception>
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

        /// <summary>
        /// Updates the details of an existing call in the system.
        /// Validates the input and ensures the call details are consistent before updating the database.
        /// </summary>
        /// <param name="call">The updated call object containing the new details.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided call object is null.</exception>
        /// <exception cref="BO.LogicException">
        /// Thrown when:
        /// - The call's address is empty or invalid.
        /// - The call's deadline is earlier than or equal to its start time.
        /// - The latitude and longitude do not match the address.
        /// - The call does not exist in the system.
        /// - Other unexpected errors occur during the update process.
        /// </exception>
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
                if (call.Latitude == null || call.Longitude == null)
                    throw new BO.LogicException("Latitude and Longitude cannot be null.");

                if (!CallManager.AreCoordinatesMatching(call.Address, call.Latitude.Value, call.Longitude.Value))
                    throw new BO.LogicException("Invalid address coordinates (latitude or longitude).");

                if (!CallManager.ValidAddress(call.Address))
                    throw new BO.LogicException("Invalid address format.");

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
