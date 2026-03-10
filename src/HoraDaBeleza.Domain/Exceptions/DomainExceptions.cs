namespace HoraDaBeleza.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.") { }
}

public class BusinessException : DomainException
{
    public BusinessException(string message) : base(message) { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Unauthorized access.") : base(message) { }
}
