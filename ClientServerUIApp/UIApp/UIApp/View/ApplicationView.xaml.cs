using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIApp.ViewModel;

namespace UIApp.View
{
    /// <summary>
    /// Логика взаимодействия для ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : UserControl
    {
        public ApplicationView()
        {
            AppViewModel = new AppViewModel();
            AppViewModel.StartProcess.CanExecuteChanged += StartProcess_CanExecuteChanged;
            InitializeComponent();
            DataContext = AppViewModel;
        }

        public AppViewModel AppViewModel { get; set; }

        public EventHandler AppViewChanged;

        private void SelectOnlySpecified(object sender, FilterEventArgs args) =>
            args.Accepted = AppViewModel.SelectSpecified(args.Item);

        private void StartProcess_CanExecuteChanged(object sender, EventArgs e) =>
            AppViewChanged?.Invoke(sender, e);
    }
}
