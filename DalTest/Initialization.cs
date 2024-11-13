namespace DalTest;

using DalApi;
using DO;
using System;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer;
    private static ICall? s_dalCall;
    private static IAssignment? s_dalAssignment;
    private static IConfig? s_dalConfig;
    private static readonly Random s_rand = new();

    private static void createVolunteers()
    {
        string[] volunteerNames = { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof" };

        foreach (var name in volunteerNames)
        {
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null);

            DateTime start = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);
            DateTime birthDate = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

            s_dalVolunteer!.Create(new Volunteer { Id = id, Name = name, BirthDate = birthDate });
        }
    }


    private static void CreateCalls()
    {
        string[] callTopics = { "Medical Assistance", "Food Delivery", "Emotional Support", "Shopping Help", "Technical Assistance" };
        foreach (var topic in callTopics)
        {
            int id;
            do
                id = s_rand.Next(1000, 9999);
            while (s_dalCall!.Read(id) != null);

            s_dalCall!.Create(new Call { Id = id, Topic = topic });
        }
    }

    private static void CreateAssignments()
    {
        var volunteerIds = s_dalVolunteer!.GetAllIds();
        var callIds = s_dalCall!.GetAllIds();

        foreach (var volunteerId in volunteerIds)
        {
            var callId = callIds[s_rand.Next(callIds.Count)];
            if (s_dalAssignment!.Read(volunteerId, callId) == null)
            {
                s_dalAssignment.Create(new Assignment { VolunteerId = volunteerId, CallId = callId });
            }
        }
    }

    public static void Do(IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("Config object can not be null!");

        Console.WriteLine("Resetting Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalVolunteer.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();

        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteers();

        Console.WriteLine("Initializing Calls list ...");
        CreateCalls();

        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignments();
    }
}
