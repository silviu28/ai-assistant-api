using System.Reflection;
using System.Runtime.CompilerServices;

namespace HCI.AIAssistant.API.Services;
public class ParametricFunctions : IParametricFunctions
{
    public bool ObjectExistsAndHasNoNullPublicProperties(Object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            if (value == null)
            {
                return false;
            }
        }

        return true;
    }

    public string GetCallerTrace(
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        return $"[{filePath}:{lineNumber}] MemberName: {memberName}";
    }
}
