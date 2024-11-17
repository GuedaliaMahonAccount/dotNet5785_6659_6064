namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{

    public void Create(Volunteer item)
    {
        // Check if the volunteer with the same ID already exists
        if (Read(item.Id) != null)
        {
            throw new Exception($"Volunteer with ID {item.Id} already exists");
        }

        // Add the volunteer if ID is unique
        DataSource.Volunteers.Add(item);
    }

    public Volunteer? Read(int id)
    {
        // Look for the volunteer by ID and return it if found, otherwise return null
        return DataSource.Volunteers.Find(v => v.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        //Create a copy of each item in the volunteer list
        var volunteerCopy = DataSource.Volunteers
            .Select(v => new Volunteer(v.Id, v.Name, v.Phone, v.Email, v.IsActive, v.Role, v.DistanceType, v.Password, v.Address, v.Latitude, v.Longitude, v.MaxDistance))
            .ToList();

        //Return the copy
        return volunteerCopy;
    }

    public void Update(Volunteer item)
    {
        //cheque if the volunteer exists
        var existingVolunteer = Read(item.Id);
        if (existingVolunteer == null)
        {
            throw new Exception($"Volunteer with ID {item.Id} does not exist");
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
            throw new Exception($"Volunteer with ID {id} does not exist");
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
