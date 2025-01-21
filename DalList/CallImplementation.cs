namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        // Add the call if ID is unique
        DataSource.Calls.Add(item);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        // Look for the call by ID and return it if found, otherwise return null
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        // Return the first call that matches the filter, or null if none match
        return DataSource.Calls.FirstOrDefault(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        // Create a copy of each item in the call list
        var callCopy = DataSource.Calls
            .Select(c => new Call(c.Id, c.CallType, c.Address, c.Latitude, c.Longitude, c.StartTime, c.Description, c.DeadLine))
            .ToList();

        // Apply the filter if provided, otherwise return all calls
        return filter != null ? callCopy.Where(filter) : callCopy;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        // Check if the call exists
        var existingCall = Read(item.Id);
        if (existingCall == null)
        {
            throw new DalDoesNotExistException($"Call with ID {item.Id} does not exist.");
        }

        // Update by removing the old entry and adding the updated one
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Add(item);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        // Check if the call exists
        var call = Read(id);
        if (call == null)
        {
            throw new DalDoesNotExistException($"Call with ID {id} does not exist.");
        }

        // Remove the call from the list
        DataSource.Calls.Remove(call);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        // Clear the list of calls
        DataSource.Calls.Clear();
    }
}
