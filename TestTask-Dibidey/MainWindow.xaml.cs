using System.Windows;
using TestTask_Dibidey.ViewModels;

namespace TestTask_Dibidey
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void search_button_Click(object sender, RoutedEventArgs e)
        {
            var searchModal = new SearchWindow();
            if (searchModal.ShowDialog() == true)
            {
                var modal = (SearchWindowViewModel)searchModal.DataContext;
                var enteredText = modal.Text;
                _viewModel.Search(enteredText);
            }
        }

        private void download_csv_button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ExportToCSV();
        }

        private void streets_button_Click(object sender, RoutedEventArgs e)
        {
            var street = new StreetsWindow();
            street.ShowDialog();
        }
    }
}