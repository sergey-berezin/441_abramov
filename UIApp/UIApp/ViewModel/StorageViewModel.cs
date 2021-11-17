using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Threading;
using System.ComponentModel;
using Entities;
using DataStorage;
using UIApp.ViewModel.Commands;
using UIApp.ViewModel.FileEntities;

namespace UIApp.ViewModel
{
    public class StorageViewModel : BaseViewModel
    {

        public StorageViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            ImagesInfo = new ObservableCollection<ImageInfo>();
            RecognizedObjects = new ObservableCollection<RecognizedObject>();
            ImageContent = new ObservableCollection<ImageViewModel>();

            ShowImageDetails = new AsyncCommand(ShowDetails, CanShowDetails);
            RemoveItem = new AsyncCommand(RemoveItemData, CanRemoveItemData);
            UpdateContentView = new AsyncCommand(UpdateContent);

            ShowImageDetails.CanExecuteChanged += RemoveItem_CanExecuteChanged;

            ShowNamesList();
        }        

        #region Properties

        public Dispatcher Dispatcher { get; set; }

        public ObservableCollection<ImageInfo> ImagesInfo { get; set; }

        public ObservableCollection<RecognizedObject> RecognizedObjects { get; set; }

        public ObservableCollection<ImageViewModel> ImageContent { get; set; }

        #endregion

        #region Commands

        public AsyncCommand ShowImageDetails { get; }

        public AsyncCommand RemoveItem { get; }

        public AsyncCommand UpdateContentView { get; }

        #endregion

        #region CommandsHandlers

        #region ShowImageDetails Handlers

        private void ShowDetails(object parameter) =>
            Dispatcher?.Invoke(() =>
            {
                ClearItemInfoView();
                ImageContent.Add(new ImageViewModel());
                ImageContent[0].SourceChanged(DbReaderRecorder.SelectImageContent((ImageInfo)parameter));
                foreach (var obj in DbReaderRecorder.SelectRecognizedObjects((ImageInfo)parameter))
                    RecognizedObjects.Add(obj);
            });

        private bool CanShowDetails(object parameter) => parameter != null;

        #endregion

        #region RemoveItem Handlers

        private void RemoveItemData(object parameter) =>
            Dispatcher?.Invoke(() =>
            {
                DbReaderRecorder.RemoveItem((ImageInfo)parameter);
                ClearItemInfoView();
                ImagesInfo.Remove((ImageInfo)parameter);
            });

        private bool CanRemoveItemData(object parameter) => parameter != null;

        #endregion

        #region UpdateContentView Handlers

        private void UpdateContent(object parameter) =>
            Dispatcher?.Invoke(() => { ShowNamesList(); });

        #endregion

        #endregion

        private void ClearItemInfoView()
        {
            RecognizedObjects.Clear();
            ImageContent.Clear();
        }

        private void ShowNamesList()
        {
            ImagesInfo.Clear();
            foreach (var imageInfo in DbReaderRecorder.SelectImagesInfo())
                ImagesInfo.Add(imageInfo);
        }

        private void RemoveItem_CanExecuteChanged(object sender, EventArgs e) =>
            Dispatcher?.Invoke(() => { RemoveItem?.RaiseCanExecuteChanged(); });
    }
}
