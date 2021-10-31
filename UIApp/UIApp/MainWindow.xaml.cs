using System.Windows;
using System.Windows.Data;
using UIApp.ViewModel;

namespace UIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {  

        public MainViewModel mainViewModel { get; set; }

        public MainWindow()
        {
            mainViewModel = new MainViewModel();
            InitializeComponent();
            DataContext = mainViewModel;
        }

        private void SelectOnlySpecified(object sender, FilterEventArgs args) => 
            args.Accepted = mainViewModel.SelectSpecified(args.Item);
    }
}
