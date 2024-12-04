using BlApi;
using System;
using System.Collections.Generic;

public interface IBl
{
    ICall Call { get; }
    IVolunteer Volunteer { get; }
    IAdmin Admin { get; }
}
