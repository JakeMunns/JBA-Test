using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DataImport.Conversion;
using DataImport.Dal;

namespace DataImport.App;

public partial class ResultsWindow : Window
{
    private readonly IEnumerable data;
    private const int BatchSize = 100;

    public ResultsWindow(object data)
    {
        InitializeComponent();
        this.data = data as IEnumerable<PrecipitationRecord> ??
                    throw new ArgumentException("Data must be of type IEnumerable<PrecipitationRecord>", nameof(data));

        ResultsDataGrid.ItemsSource = this.data;
    }

    private async void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            UploadButton.IsEnabled = false;
            UploadProgressBar.Visibility = Visibility.Visible;
            UploadProgressBar.IsIndeterminate = false;
            UploadProgressBar.Value = 0;

            await Dispatcher.Yield(DispatcherPriority.Background);

            var records = data.Cast<PrecipitationRecord>().ToList();
            var repo = new DataImportRepository();

            var progress = new Progress<int>(percent =>
            {
                UploadProgressBar.Value = percent;
            });

            await repo.PrecipitationRepository.BulkInsertAsync(records, progress);

            UploadProgressBar.Visibility = Visibility.Collapsed;
            MessageBox.Show("Data uploaded to the database.", "Upload", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            UploadProgressBar.Visibility = Visibility.Collapsed;
            MessageBox.Show("Upload failed:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            UploadButton.IsEnabled = true;
        }
    }

    private void Header_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}