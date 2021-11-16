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
            ImageContent = new ImageViewModel();

            ShowImageDetails = new AsyncCommand(ShowDetails);
            RemoveItem = new AsyncCommand(RemoveItemData, CanRemoveItemData);

            ShowImageDetails.PropertyChanged += CanRemoveItemChanged;

            ShowNamesList();
        }

        #region Properties
        
        public Dispatcher Dispatcher { get; set; }

        public ObservableCollection<ImageInfo> ImagesInfo { get; set; }

        public ObservableCollection<RecognizedObject> RecognizedObjects { get; set; }

        public ImageViewModel ImageContent { get; set; }

        #endregion

        #region Commands

        public AsyncCommand ShowImageDetails { get; }

        public AsyncCommand RemoveItem { get; }

        #endregion

        #region CommandsHandlers

        #region ShowImageDetails Handlers

        private void ShowDetails(object parameter) =>
            Dispatcher?.Invoke(() =>
            {
                ImageContent.SourceChanged(DbReaderRecorder.SelectImageContent((ImageInfo)parameter));
                RecognizedObjects.Clear();
                foreach (var obj in DbReaderRecorder.SelectRecognizedObjects((ImageInfo)parameter))
                    RecognizedObjects.Add(obj);
            });

        #endregion

        #region RemoveItem Handlers

        private void RemoveItemData(object parameter) =>
            Dispatcher?.Invoke(() =>
            {
                /*RecognizedObjects.Clear();
                ImageContent = null;
                ImagesInfo.Remove((ImageInfo)parameter);
                DbReaderRecorder.RemoveItem((ImageInfo)parameter);*/
            });

        private bool CanRemoveItemData(object parameter) => parameter != null;

        #endregion

        #endregion

        public void ShowNamesList()
        {
            ImagesInfo.Clear();
            foreach (var imageInfo in DbReaderRecorder.SelectImagesInfo())
                ImagesInfo.Add(imageInfo);
        }

        private void CanRemoveItemChanged(object sender, PropertyChangedEventArgs e) =>
            Dispatcher?.Invoke(() => { RemoveItem?.RaiseCanExecuteChanged(); });
    }
}
