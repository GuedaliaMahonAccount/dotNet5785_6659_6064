namespace BO;

/// <summary>
/// Exception thrown when an object does not exist in the BL layer.
/// </summary>
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a property value is null in the BL layer.
/// </summary>
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an invalid ID is provided in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidIdException : Exception
{
    public BlInvalidIdException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided name is invalid (e.g., missing first or last name) in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidNameException : Exception
{
    public BlInvalidNameException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the phone number does not meet the required format in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidPhoneNumberException : Exception
{
    public BlInvalidPhoneNumberException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided email is not in a valid format in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidEmailException : Exception
{
    public BlInvalidEmailException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided password does not meet the security requirements in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidPasswordException : Exception
{
    public BlInvalidPasswordException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the address is invalid or empty in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidAddressException : Exception
{
    public BlInvalidAddressException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided latitude or longitude is invalid in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidCoordinateException : Exception
{
    public BlInvalidCoordinateException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the specified role is not recognized in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidRoleException : Exception
{
    public BlInvalidRoleException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the provided maximum distance is invalid in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidDistanceException : Exception
{
    public BlInvalidDistanceException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when a date or time input is not valid in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidDateException : Exception
{
    public BlInvalidDateException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an unknown option is selected in a menu or switch statement in the BL layer.
/// </summary>
[Serializable]
public class BlUnknownTypeException : Exception
{
    public BlUnknownTypeException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an argument is null in the BL layer.
/// </summary>
[Serializable]
public class BlArgumentNullException : Exception
{
    public BlArgumentNullException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an object already exists in the BL layer.
/// </summary>
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when deletion is impossible in the BL layer.
/// </summary>
[Serializable]
public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an invalid value is provided in the BL layer.
/// </summary>
[Serializable]
public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when a file load or create operation fails in the BL layer.
/// </summary>
[Serializable]
public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }
}


public class LogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogicException"/> class.
    /// </summary>
    public LogicException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogicException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LogicException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogicException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
   public LogicException(string message, Exception innerException) : base(message, innerException) { }
}
