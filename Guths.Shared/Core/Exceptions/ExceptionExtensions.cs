using Guths.Shared.Helpers;
using Guths.Shared.Web.Handlers;

namespace Guths.Shared.Core.Exceptions;

public static class ExceptionExtensions
{
    private const string ErrorCodeKey = "ErrorCode";

    public static void AddErrorCode(this Exception exception)
    {
        var errorCode = ErrorCodeGeneratorHelper.GenerateErrorCode();
        if (exception.Data.Contains(ErrorCodeKey))
        {
            exception.Data[ErrorCodeKey] = errorCode;
        }
        else
        {
            exception.Data.Add(ErrorCodeKey, errorCode);
        }
    }

    public static string GetErrorCode(this Exception exception)
    {
        if (exception.Data.Contains(ErrorCodeKey) && exception.Data[ErrorCodeKey] is string errorCode)
        {
            return errorCode;
        }

        return GlobalExceptionHandler.UnhandledExceptionMsg;
    }
}
