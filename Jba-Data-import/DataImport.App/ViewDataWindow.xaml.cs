using System.Windows;
using DataImport.Conversion;

namespace DataImport.App;

public partial class ViewDataWindow : Window
{
    public ViewDataWindow(List<PrecipitationRecord> records)
    {
        InitializeComponent();
        DataGrid.ItemsSource = records;
    }
}