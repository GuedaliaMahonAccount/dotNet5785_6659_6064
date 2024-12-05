using System;

namespace BO
{

    public class ArgumentNullException : Exception
    {
        public ArgumentNullException() { }

        public ArgumentNullException(string message)
            : base(message) { }

        public ArgumentNullException(string message, Exception inner)
            : base(message, inner) { }
    }
}

public class VolunteerNotFoundException : Exception
{
    public VolunteerNotFoundException(string message) : base(message) { }
}