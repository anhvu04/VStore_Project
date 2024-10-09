using VStore.Domain.Exceptions.DomainExceptions;

namespace VStore.Domain.Exceptions;

public class DataSavingException(string message) : DomainException(message)
{
}