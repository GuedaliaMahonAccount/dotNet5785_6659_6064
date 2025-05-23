﻿namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        // Check if the assignment with the same ID already exists
        if (Read(item.Id) != null)
        {
            throw new DalAlreadyExistsException($"Assignment with ID {item.Id} already exists.");
        }

        // Check if the CallId exists
        if (!DataSource.Calls.Any(call => call.Id == item.CallId))
        {
            throw new DalDoesNotExistException($"Call with ID {item.CallId} does not exist.");
        }

        // Check if the VolunteerId exists
        if (!DataSource.Volunteers.Any(volunteer => volunteer.Id == item.VolunteerId))
        {
            throw new DalDoesNotExistException($"Volunteer with ID {item.VolunteerId} does not exist.");
        }

        // Add the assignment if all checks pass
        DataSource.Assignments.Add(item);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        // Look for the assignment by ID and return it if found, otherwise return null
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        // Return the first assignment that matches the filter, or null if none match
        return DataSource.Assignments.FirstOrDefault(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        // Create a copy of the assignment list
        var assignmentCopy = DataSource.Assignments
            .Select(a => new Assignment(a.Id, a.CallId, a.VolunteerId, a.StartTime, a.EndTime, a.EndType))
            .ToList();

        // Apply the filter if provided, otherwise return all assignments
        return filter != null ? assignmentCopy.Where(filter) : assignmentCopy;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        // Check if the assignment exists
        var existingAssignment = Read(item.Id);
        if (existingAssignment == null)
        {
            throw new DalDoesNotExistException($"Assignment with ID {item.Id} does not exist.");
        }

        // Update by removing the old entry and adding the updated one
        DataSource.Assignments.Remove(existingAssignment);
        DataSource.Assignments.Add(item);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        // Check if the assignment exists
        var assignment = Read(id);
        if (assignment == null)
        {
            throw new DalDoesNotExistException($"Assignment with ID {id} does not exist.");
        }

        // Remove the assignment from the list
        DataSource.Assignments.Remove(assignment);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        // Clear the list of assignments
        DataSource.Assignments.Clear();
    }
}
