namespace DataImport.Dal;

public class DataImportRepository
{
    public PrecipitationRepository PrecipitationRepository { get; } = new();
}