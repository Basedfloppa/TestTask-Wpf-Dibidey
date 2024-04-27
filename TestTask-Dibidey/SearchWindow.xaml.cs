using TestTask_Dibidey.ViewModels;
using System.Windows;

namespace TestTask_Dibidey
{
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
            DataContext = new SearchWindowViewModel();
        }
    }
}