using System.Net;

namespace LaboratorioMVC.Services
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ApiException(HttpStatusCode statusCode)
            : base($"La API respondió con el código {(int)statusCode}.")
        {
            StatusCode = statusCode;
        }
    }
}