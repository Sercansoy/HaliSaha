using HaliSaha.Core.Exceptions;
namespace HaliSaha.Core.Exceptions
{
    public class DomainException(string message) : Exception(message);
}
