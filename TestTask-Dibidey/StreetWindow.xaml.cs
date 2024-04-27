using TestTask_Dibidey.ViewModels;
using System.Windows;

namespace TestTask_Dibidey
{
    public partial class StreetsWindow : Window
    {
        public StreetsWindow()
        {
            InitializeComponent();
            DataContext = new StreetsWindowViewModel();
        }
    }
}