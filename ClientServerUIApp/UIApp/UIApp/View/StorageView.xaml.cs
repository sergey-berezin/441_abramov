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
    /// Логика взаимодействия для StorageView.xaml
    /// </summary>
    public partial class StorageView : UserControl
    {
        public StorageView()
        {
            StorageViewModel = new StorageViewModel();
            InitializeComponent();
            DataContext = StorageViewModel;
        }

        public StorageViewModel StorageViewModel { get; set; }

        public void UpdateView() => StorageViewModel.UpdateContent();
    }
}
