using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataImport.Conversion;
using DataImport.Dal;
using Microsoft.EntityFrameworkCore;

namespace DataImport.App;

public partial class ViewDataWindow : Window
{
    public ViewDataWindow()
    {
        InitializeComponent();
        Loaded += ViewDataWindow_Loaded;
    }

    private async void ViewDataWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            LoadingOverlay.Visibility = Visibility.Visible;
            DataGrid.ItemsSource = null;

            await Task.Delay(100);
            
            List<PrecipitationRecord> records;
            await using (var db = new DataImportDbContext())
            {
                records = await db.PrecipitationRecords.ToListAsync();
            }

            DataGrid.ItemsSource = records;
            LoadingOverlay.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            LoadingOverlay.Visibility = Visibility.Collapsed;
            MessageBox.Show("Failed to load data:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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