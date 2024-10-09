namespace VStore.Domain.Exceptions.DomainExceptions;

public abstract class DomainException(string message) : Exception(message)
{
}