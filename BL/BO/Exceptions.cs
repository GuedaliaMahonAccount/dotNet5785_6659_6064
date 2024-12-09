namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
}

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message) : base(message) { }
}

[Serializable]
public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidIdException : Exception
{
    public BlInvalidIdException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidNameException : Exception
{
    public BlInvalidNameException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidPhoneNumberException : Exception
{
    public BlInvalidPhoneNumberException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidEmailException : Exception
{
    public BlInvalidEmailException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidPasswordException : Exception
{
    public BlInvalidPasswordException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidAddressException : Exception
{
    public BlInvalidAddressException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidCoordinateException : Exception
{
    public BlInvalidCoordinateException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidRoleException : Exception
{
    public BlInvalidRoleException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidDistanceException : Exception
{
    public BlInvalidDistanceException(string? message) : base(message) { }
}

[Serializable]
public class BlInvalidDateException : Exception
{
    public BlInvalidDateException(string? message) : base(message) { }
}

[Serializable]
public class BlUnknownTypeException : Exception
{
    public BlUnknownTypeException(string? message) : base(message) { }
}

[Serializable]
public class BlArgumentNullException : Exception
{
    public BlArgumentNullException(string? message) : base(message) { }
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
