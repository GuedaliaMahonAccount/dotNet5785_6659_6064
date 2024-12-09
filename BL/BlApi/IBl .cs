using BlApi;
using System;
using System.Collections.Generic;

namespace BlApi;

public interface IBl
{
    ICall Call { get; }
    IVolunteer Volunteer { get; }
    IAdmin Admin { get; }
}
