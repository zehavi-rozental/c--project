using System;

namespace Dal
{
    public class IdNotFoundException : Exception
    {
        public IdNotFoundException() : base("ID not found")
        {
        }
    }

    public class IdAlreadyExistsException : Exception
    {
        public IdAlreadyExistsException(string v) : base("The ID already exists")
        {
        }
    }
}