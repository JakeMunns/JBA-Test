namespace DataImport.Conversion;

public static class DataConverterFactory
{
    public static IDataConverter CreateStringImporter()
    {
        return new StringDataConverter();
    }

    public static IDataConverter CreateFileImporter()
    {
        return new FileDataConverter();
    }
}