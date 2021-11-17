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


        public MainWindow()
        {
            MainViewModel = new MainViewModel();
            InitializeComponent();
            DataContext = MainViewModel;
            Loaded += MainWindow_Loaded;
        }
        
        public MainViewModel MainViewModel { get; set; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new StorageWindow().Show(); // show DataStorage content after MainWindow loaded
        }

        private void SelectOnlySpecified(object sender, FilterEventArgs args) => 
            args.Accepted = MainViewModel.SelectSpecified(args.Item);
    }
}
