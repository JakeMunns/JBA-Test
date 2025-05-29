namespace DataImport.Conversion;

public interface IDataConverter
{
    /// <summary>
    /// Processes precipitation data, extracting records and identifying errors.
    /// </summary>
    /// <param name="file">The file to be processed.</param>
    /// <param name="errors">An output parameter that contains a collection of error messages encountered during processing.</param>
    /// <returns>A list of <see cref="PrecipitationRecord"/> objects representing the parsed precipitation data.</returns>
    List<PrecipitationRecord> ImportData(string file, out IEnumerable<string> errors);
}