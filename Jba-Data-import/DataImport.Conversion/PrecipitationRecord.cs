namespace DataImport.Conversion;

public record PrecipitationRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Xref { get; set; }
    public int Yref { get; set; }
    public DateTime Date { get; set; }
    public int Value { get; set; }
}