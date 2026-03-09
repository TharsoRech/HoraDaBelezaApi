namespace HoraDaBeleza.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entidade, object id)
        : base($"{entidade} com id '{id}' não foi encontrado.") { }
}

public class BusinessException : DomainException
{
    public BusinessException(string message) : base(message) { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Acesso não autorizado.") : base(message) { }
}
