namespace MyTranslationApp.Application.Exceptions;

public class ProviderException : Exception
{
    public string ErrorCode { get; }

    public ProviderException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public ProviderException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
