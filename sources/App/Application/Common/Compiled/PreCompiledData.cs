using System.Text.RegularExpressions;

namespace Application.Common.Compiled;

public static partial class PreCompiledData
{
    [GeneratedRegex("^\\+?[1-9]\\d{1,14}$", RegexOptions.Compiled)]
    public static partial Regex PhoneRegex();

    [GeneratedRegex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    public static partial Regex EmailRegex();
}
