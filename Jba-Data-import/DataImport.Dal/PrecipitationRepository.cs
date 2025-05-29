using DataImport.Conversion;
using Microsoft.Data.Sqlite;

namespace DataImport.Dal;

public class PrecipitationRepository
{
    public async Task BulkInsertAsync(IEnumerable<PrecipitationRecord> records, IProgress<int>? progress = null)
    {
        var recordList = records as IList<PrecipitationRecord> ?? records.ToList();
        int total = recordList.Count;
        int processed = 0;

        await using var connection = new SqliteConnection("Data Source=dataimport.db");
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO PrecipitationRecords (Id, Xref, Yref, Date, Value)
        VALUES ($id, $xref, $yref, $date, $value);";
        command.Parameters.Add(new SqliteParameter("$id", System.Data.DbType.String));
        command.Parameters.Add(new SqliteParameter("$xref", System.Data.DbType.Int32));
        command.Parameters.Add(new SqliteParameter("$yref", System.Data.DbType.Int32));
        command.Parameters.Add(new SqliteParameter("$date", System.Data.DbType.String));
        command.Parameters.Add(new SqliteParameter("$value", System.Data.DbType.Int32));

        foreach (var record in recordList)
        {
            command.Parameters["$id"].Value = record.Id;
            command.Parameters["$xref"].Value = record.Xref;
            command.Parameters["$yref"].Value = record.Yref;
            command.Parameters["$date"].Value = record.Date.ToString("yyyy-MM-dd");
            command.Parameters["$value"].Value = record.Value;
            await command.ExecuteNonQueryAsync();

            processed++;
            if (progress != null && (processed % 100 == 0 || processed == total))
            {
                int percent = (int)((processed / (double)total) * 100);
                progress.Report(percent);
                // Yield to UI thread to allow progress bar update
                await Task.Yield();
            }
        }

        await transaction.CommitAsync();
    }
}