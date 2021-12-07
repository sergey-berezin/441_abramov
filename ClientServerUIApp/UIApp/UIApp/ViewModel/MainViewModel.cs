using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using UIApp.ViewModel.Commands;

namespace UIApp.ViewModel
{
    class MainViewModel : BaseViewModel
    {

        private AppViewModel _appViewModel;
        private StorageViewModel _storageViewModel;

        public MainViewModel() : base()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            ShowStorageView = new AsyncCommand(LoadStorageView);
            ShowApplicationView = new AsyncCommand(LoadAppView);

            _appViewModel = new AppViewModel();
            _storageViewModel = new StorageViewModel();
        }

        #region Properties

        public Dispatcher Dispatcher { get; set; }

        #endregion

        #region Commands

        public AsyncCommand ShowStorageView { get; }

        public AsyncCommand ShowApplicationView { get; }

        #endregion

        #region CommandHandlers

        #region ShowStorageView Methods

        private void LoadStorageView(object parameter) =>
            Dispatcher?.Invoke(() =>
            {
                
            });

        #endregion

        #region ShowApplicationView Methods

        private void LoadAppView(object parameter) =>
            Dispatcher?.Invoke(() => { } );

        #endregion

        #endregion

    }
}
