namespace DalTest;

using DalApi;
using DO;
using System;

public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();



    public static void Do(IDal dal)
    {
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();
    }

}
