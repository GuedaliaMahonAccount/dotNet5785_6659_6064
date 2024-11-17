namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        // Check if the assignment with the same ID already exists
        if (Read(item.Id) != null)
        {
            throw new Exception($"Assignment with ID {item.Id} already exists.");
        }

        // Check if the CallId exists
        var callExists = DataSource.Calls.FirstOrDefault(call => call.Id == item.CallId);
        if (callExists == null)
        {
            throw new Exception($"Call with ID {item.CallId} does not exist.");
        }

        // Check if the VolunteerId exists
        var volunteerExists = DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.Id == item.VolunteerId);
        if (volunteerExists == null)
        {
            throw new Exception($"Volunteer with ID {item.VolunteerId} does not exist.");
        }

        // Add the assignment if all checks pass
        DataSource.Assignments.Add(item);
    }

    public Assignment? Read(int id)
    {
        // Look for the assignment by ID and return it if found, otherwise return null
        return DataSource.Assignments.Find(a => a.Id == id);
    }

    public List<Assignment> ReadAll()
    {
        // Step 1: Create a copy of each item in the assignment list
        var assignmentCopy = DataSource.Assignments
            .Select(a => new Assignment(a.Id, a.CallId, a.VolunteerId, a.StartTime, a.EndTime, a.EndType))
            .ToList();

        // Step 2: Return the copy
        return assignmentCopy;
    }

    public void Update(Assignment item)
    {
        // Check if the assignment exists
        var existingAssignment = Read(item.Id);
        if (existingAssignment == null)
        {
            throw new Exception($"Assignment with ID {item.Id} does not exist");
        }

        // Update by removing the old entry and adding the updated one
        DataSource.Assignments.Remove(existingAssignment);
        DataSource.Assignments.Add(item);
    }

    public void Delete(int id)
    {
        // Check if the assignment exists
        var assignment = Read(id);
        if (assignment == null)
        {
            throw new Exception($"Assignment with ID {id} does not exist");
        }

        // Remove the assignment from the list
        DataSource.Assignments.Remove(assignment);
    }

    public void DeleteAll()
    {
        // Clear the list of assignments
        DataSource.Assignments.Clear();
    }
}
