namespace DO;

public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}

public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}

/// <summary>
/// when he get an option taht not exist in the switch
/// </summary>
public class UnknownType : Exception
{
    public UnknownType(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an invalid ID is provided.
/// </summary>
public class InvalidIdException : Exception
{
    public InvalidIdException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided name is invalid (e.g., missing first or last name).
/// </summary>
public class InvalidNameException : Exception
{
    public InvalidNameException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the phone number does not meet the required format.
/// </summary>
public class InvalidPhoneNumberException : Exception
{
    public InvalidPhoneNumberException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided email is not in a valid format.
/// </summary>
public class InvalidEmailException : Exception
{
    public InvalidEmailException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided password does not meet the security requirements.
/// </summary>
public class InvalidPasswordException : Exception
{
    public InvalidPasswordException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the address is invalid or empty.
/// </summary>
public class InvalidAddressException : Exception
{
    public InvalidAddressException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided latitude or longitude is invalid.
/// </summary>
public class InvalidCoordinateException : Exception
{
    public InvalidCoordinateException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the specified role is not recognized.
/// </summary>
public class InvalidRoleException : Exception
{
    public InvalidRoleException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided maximum distance is invalid.
/// </summary>
public class InvalidDistanceException : Exception
{
    public InvalidDistanceException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when a date or time input is not valid.
/// </summary>
public class InvalidDateException : Exception
{
    public InvalidDateException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an unknown option is selected in a menu or switch statement.
/// </summary>
public class UnknownTypeException : Exception
{
    public UnknownTypeException(string? message) : base(message) { }
}
