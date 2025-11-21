using System.Runtime.CompilerServices;

namespace HCI.AIAssistant.API.Services;

public interface IParametricFunctions
{
    bool ObjectExistsAndHasNoNullPublicProperties(Object? obj);
    public string GetCallerTrace(
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);
}

