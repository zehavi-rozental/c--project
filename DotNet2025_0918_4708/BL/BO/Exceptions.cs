namespace BO;

[Serializable]
public class BLException : Exception
{
    public BLException(string? message) : base(message) { }
    public BLException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BLIdNotFoundException : BLException
{
    public BLIdNotFoundException(string? message) : base(message) { }
    public BLIdNotFoundException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BLIdAlreadyExistsException : BLException
{
    public BLIdAlreadyExistsException(string? message) : base(message) { }
    public BLIdAlreadyExistsException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BLInvalidInputException : BLException
{
    public BLInvalidInputException(string? message) : base(message) { }
    public BLInvalidInputException(string? message, Exception innerException) : base(message, innerException) { }
}

[Serializable]
public class BLInsufficientStockException : BLException
{
    public BLInsufficientStockException(string? message) : base(message) { }
    public BLInsufficientStockException(string? message, Exception innerException) : base(message, innerException) { }
}
