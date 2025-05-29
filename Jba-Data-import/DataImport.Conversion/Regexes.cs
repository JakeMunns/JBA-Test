using System.Text.RegularExpressions;

namespace DataImport.Conversion;

internal partial class Regexes
{
    [GeneratedRegex(@"Grid[\s-]Ref=\s*(\d+),\s*(\d+)", RegexOptions.IgnoreCase)]
    public static partial Regex GridRefRegex();
    
    [GeneratedRegex(@"\[Years=(\d+)-(\d+)\]")]
    public static partial Regex YearsRegex();
}