using System;
using System.Collections.Generic;

namespace DataImport.Conversion;

internal class StringDataConverter : IDataConverter
{
    public List<PrecipitationRecord> ImportData(string fileContents, out IEnumerable<string> errors)
    {
        var records = new List<PrecipitationRecord>();
        var errorsList = new List<string>();
        int? xref = null, yef = null;
        int? startYear = null;

        string[] lines = fileContents.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        // Find start year once
        foreach (var line in lines)
        {
            var yearsMatch = Regexes.YearsRegex().Match(line);
            if (!yearsMatch.Success) continue;
            
            startYear = int.Parse(yearsMatch.Groups[1].Value);
            break;
        }

        if (!startYear.HasValue)
            throw new FormatException("Could not find start year in the file header.");

        int year = startYear.Value;

        foreach (string line in lines)
        {
            var match = Regexes.GridRefRegex().Match(line);
            if (match.Success)
            {
                xref = int.Parse(match.Groups[1].Value);
                yef = int.Parse(match.Groups[2].Value);
                year = startYear.Value;
                continue;
            }

            if (!xref.HasValue || !yef.HasValue)
                continue;

            var trimmed = line.AsSpan().Trim();
            if (trimmed.Length == 0 || !char.IsDigit(trimmed[0]))
                continue;

            string[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int month = 0; month < values.Length; month++)
            {
                if (int.TryParse(values[month], out int value))
                {
                    records.Add(new PrecipitationRecord
                    {
                        Xref = xref.Value,
                        Yref = yef.Value,
                        Date = new DateTime(year, month + 1, 1),
                        Value = value
                    });
                }
                else
                {
                    errorsList.Add(
                        $"Invalid integer '{values[month]}' at Grid-ref= {xref},{yef}, Year={year}, Month={month + 1}");
                }
            }
            year++;
        }

        errors = errorsList;
        return records;
    }
}