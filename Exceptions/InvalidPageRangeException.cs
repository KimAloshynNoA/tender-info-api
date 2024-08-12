using System.Net;

namespace TenderInfoAPI.Exceptions;

public class InvalidPageRangeException : ApiException
{
    public InvalidPageRangeException(string message)
        : base(message, (int)HttpStatusCode.BadRequest)
    {
    }
}
