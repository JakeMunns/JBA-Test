namespace DataImport.Conversion;

public class FileDataConverter : IDataConverter
{
    public List<PrecipitationRecord> ImportData(string file, out IEnumerable<string> errors)
    {
        string contents = File.ReadAllText(file);
        var stringDataImporter = new StringDataConverter();
        
        var records = stringDataImporter.ImportData(contents, out errors);
        
        return records;
    }
}