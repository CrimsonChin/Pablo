using MahApps.Metro.Controls.Dialogs;
using Pablo.ViewModels;

namespace Pablo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(DialogCoordinator.Instance);
        }
    }
}
