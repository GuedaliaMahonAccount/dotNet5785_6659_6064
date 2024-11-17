namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        // Check if the call with the same ID already exists
        if (Read(item.Id) != null)
        {
            throw new Exception($"Call with ID {item.Id} already exists.");
        }

        // Add the call if ID is unique
        DataSource.Calls.Add(item);
    }

    public Call? Read(int id)
    {
        // Look for the call by ID and return it if found, otherwise return null
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
    }

    public Call? Read(Func<Call, bool> filter)
    {
        // Return the first call that matches the filter, or null if none match
        return DataSource.Calls.FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        // Create a copy of each item in the call list
        var callCopy = DataSource.Calls
            .Select(c => new Call(c.Id, c.CallType, c.Address, c.Latitude, c.Longitude, c.StartTime, c.Description, c.DeadLine))
            .ToList();

        // Apply the filter if provided, otherwise return all calls
        return filter != null ? callCopy.Where(filter) : callCopy;
    }

    public void Update(Call item)
    {
        // Check if the call exists
        var existingCall = Read(item.Id);
        if (existingCall == null)
        {
            throw new Exception($"Call with ID {item.Id} does not exist.");
        }

        // Update by removing the old entry and adding the updated one
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Add(item);
    }

    public void Delete(int id)
    {
        // Check if the call exists
        var call = Read(id);
        if (call == null)
        {
            throw new Exception($"Call with ID {id} does not exist.");
        }

        // Remove the call from the list
        DataSource.Calls.Remove(call);
    }

    public void DeleteAll()
    {
        // Clear the list of calls
        DataSource.Calls.Clear();
    }
}
