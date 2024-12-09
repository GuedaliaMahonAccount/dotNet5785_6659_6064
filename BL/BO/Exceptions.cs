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

    public BlNullPropertyException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message) : base(message) { }

    public BlInvalidValueException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }

    public BlDeletionImpossibleException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidIdException : Exception
{
    public BlInvalidIdException(string? message) : base(message) { }

    public BlInvalidIdException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidNameException : Exception
{
    public BlInvalidNameException(string? message) : base(message) { }

    public BlInvalidNameException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidPhoneNumberException : Exception
{
    public BlInvalidPhoneNumberException(string? message) : base(message) { }

    public BlInvalidPhoneNumberException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidEmailException : Exception
{
    public BlInvalidEmailException(string? message) : base(message) { }

    public BlInvalidEmailException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidPasswordException : Exception
{
    public BlInvalidPasswordException(string? message) : base(message) { }

    public BlInvalidPasswordException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidAddressException : Exception
{
    public BlInvalidAddressException(string? message) : base(message) { }

    public BlInvalidAddressException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidCoordinateException : Exception
{
    public BlInvalidCoordinateException(string? message) : base(message) { }

    public BlInvalidCoordinateException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidRoleException : Exception
{
    public BlInvalidRoleException(string? message) : base(message) { }

    public BlInvalidRoleException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidDistanceException : Exception
{
    public BlInvalidDistanceException(string? message) : base(message) { }

    public BlInvalidDistanceException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlInvalidDateException : Exception
{
    public BlInvalidDateException(string? message) : base(message) { }

    public BlInvalidDateException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlUnknownTypeException : Exception
{
    public BlUnknownTypeException(string? message) : base(message) { }

    public BlUnknownTypeException(string? message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
public class BlArgumentNullException : Exception
{
    public BlArgumentNullException(string? message) : base(message) { }

    public BlArgumentNullException(string? message, Exception innerException)
        : base(message, innerException) { }
}

public class LogicException : Exception
{
    public LogicException() { }

    public LogicException(string message) : base(message) { }

    public LogicException(string message, Exception innerException) : base(message, innerException) { }
}
