using System.Windows;
using System.Windows.Data;
using UIApp.ViewModel;
using UIApp.View;
using System.Windows.Controls;

namespace UIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {  
        public MainWindow()
        {
            StorageView = new StorageView();
            ApplicationView = new ApplicationView();
            ApplicationView.AppViewChanged += ChangeView; 
            InitializeComponent();
            MenuItem_Storage_Click(this, new RoutedEventArgs());
        }


        public StorageView StorageView { get; }

        public ApplicationView ApplicationView { get; }

        private void MenuItem_Recognition_Click(object sender, RoutedEventArgs e) => 
            OutputView.Content = ApplicationView;

        private void MenuItem_Storage_Click(object sender, RoutedEventArgs e) =>
            OutputView.Content = StorageView;

        private void ChangeView(object sender, System.EventArgs e) =>
            StorageView.UpdateView();
    }
}
