namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Volunteer item)
    {
        // Check if the volunteer with the same ID already exists
        if (Read(item.Id) != null)
        {
            throw new DalAlreadyExistsException($"Volunteer with ID {item.Id} already exists.");
        }

        // Add the volunteer if ID is unique
        DataSource.Volunteers.Add(item);
    }

    public Volunteer? Read(int id)
    {
        // Look for the volunteer by ID and return it if found, otherwise return null
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        // Return the first volunteer that matches the filter, or null if none match
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        // Create a copy of each item in the volunteer list
        var volunteerCopy = DataSource.Volunteers
            .Select(v => new Volunteer(
                v.Id, v.Name, v.Phone, v.Email, v.IsActive, v.Role,
                v.DistanceType, v.Password, v.Address, v.Latitude,
                v.Longitude, v.MaxDistance))
            .ToList();

        // Apply the filter if provided, otherwise return all volunteers
        return filter != null ? volunteerCopy.Where(filter) : volunteerCopy;
    }
    // Eli Amar  pas- Z8mQ7xW4rB
    public void Update(Volunteer item)
    {
        // Check if the volunteer exists
        var existingVolunteer = Read(item.Id);
        if (existingVolunteer == null)
        {
            throw new DalDoesNotExistException($"Volunteer with ID {item.Id} does not exist.");
        }

        // Update by removing the old entry and adding the updated one
        DataSource.Volunteers.Remove(existingVolunteer);
        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        // Check if the volunteer exists
        var volunteer = Read(id);
        if (volunteer == null)
        {
            throw new DalDoesNotExistException($"Volunteer with ID {id} does not exist.");
        }

        // Remove the volunteer from the list
        DataSource.Volunteers.Remove(volunteer);
    }

    public void DeleteAll()
    {
        // Clear the list of volunteers
        DataSource.Volunteers.Clear();
    }

}
