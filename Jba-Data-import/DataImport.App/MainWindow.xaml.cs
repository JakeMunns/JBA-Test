using System.IO;
using System.Windows;
using System.Windows.Input;
using DataImport.Conversion;
using DataImport.Dal;
using Microsoft.EntityFrameworkCore;

namespace DataImport.App;

public partial class MainWindow : Window
{
    private readonly IDataConverter converter;

    public MainWindow()
    {
        converter = DataConverterFactory.CreateStringImporter();
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await using var db = new DataImportDbContext();
        await db.Database.EnsureCreatedAsync();
    }

    private void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "All Files|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            FilePathTextBox.Text = dialog.FileName;
        }
    }

    private void FilePathTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        ProcessFileButton.IsEnabled =
            !string.IsNullOrWhiteSpace(FilePathTextBox.Text) && File.Exists(FilePathTextBox.Text);
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

    private void ProcessFileButton_Click(object sender, RoutedEventArgs e)
    {
        string filePath = FilePathTextBox.Text;

        if (!File.Exists(filePath))
        {
            MessageBox.Show("File does not exist.");
            return;
        }

        ProgressBar.Visibility = Visibility.Visible;
        StatusTextBlock.Text = string.Empty;
        ProcessFileButton.IsEnabled = false;
        SelectFileButton.IsEnabled = false;

        string fileContents = File.ReadAllText(filePath);

        // Await the import operation
        var data = converter.ImportData(fileContents, out var errors);

        ProgressBar.Visibility = Visibility.Collapsed;
        ProcessFileButton.IsEnabled = true;
        SelectFileButton.IsEnabled = true;

        if (errors != null && errors.Any())
        {
            StatusTextBlock.Text = "Import completed with errors:\n" + string.Join("\n", errors);
            MessageBox.Show("Import completed with errors.", "Result", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            StatusTextBlock.Text = "Import successful!";
            MessageBox.Show("Import successful!", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        if (data != null)
        {
            var resultsWindow = new ResultsWindow(data)
            {
                Owner = this
            };
            resultsWindow.ShowDialog();
        }
    }

    private void ViewDataButton_Click(object sender, RoutedEventArgs e)
    {
        var viewWindow = new ViewDataWindow
        {
            Owner = this
        };
        viewWindow.ShowDialog();
    }
}