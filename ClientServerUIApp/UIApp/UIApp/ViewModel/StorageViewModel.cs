using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading.Tasks;
using UIApp.ViewModel.Commands;
using UIApp.ViewModel.FileEntities;
using ClientLib;
using WebClassLib;

namespace UIApp.ViewModel
{
    public class StorageViewModel : BaseViewModel
    {

        public StorageViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            ImagesInfo = new ObservableCollection<WebImageInfo>();
            RecognizedObjects = new ObservableCollection<WebRecognizedObject>();
            ImageContent = new ObservableCollection<ImageViewModel>();

            ShowImageDetails = new AsyncCommand(ShowDetails, CanShowDetails);
            RemoveItem = new AsyncCommand(RemoveItemData, CanRemoveItemData);

            ShowImageDetails.CanExecuteChanged += RemoveItem_CanExecuteChanged;

            UpdateContent();
        }

        #region Properties

        public Dispatcher Dispatcher { get; set; }

        public ObservableCollection<WebImageInfo> ImagesInfo { get; set; }

        public ObservableCollection<WebRecognizedObject> RecognizedObjects { get; set; }

        public ObservableCollection<ImageViewModel> ImageContent { get; set; }

        #endregion

        #region Commands

        public AsyncCommand ShowImageDetails { get; }

        public AsyncCommand RemoveItem { get; }

        #endregion

        #region CommandsHandlers

        #region ShowImageDetails Handlers

        private void ShowDetails(object parameter)
        {
            int imageId = ((WebImageInfo)parameter).ImageInfoId;
            KeyValuePair<byte[], List<WebRecognizedObject>>? imageDetails = Client.Get(App.Uri, imageId);
            var bitmap = new System.Drawing.Bitmap(new System.IO.MemoryStream(imageDetails.Value.Key));
            Dispatcher?.Invoke(() =>
            {
                ClearItemInfoView();
                ImageContent.Add(new ImageViewModel());
                ImageContent[0].SourceChanged(bitmap);
                foreach (var obj in imageDetails.Value.Value)
                    RecognizedObjects.Add(obj);
            });
        }

        private bool CanShowDetails(object parameter) => parameter != null;

        #endregion

        #region RemoveItem Handlers

        private void RemoveItemData(object parameter)
        {
            var imgInfo = (WebImageInfo)parameter;
            Client.Delete(App.Uri, imgInfo.ImageInfoId);
            Dispatcher?.Invoke(() =>
            {
                ClearItemInfoView();
                ImagesInfo.Remove(imgInfo);
            });
        }

        private bool CanRemoveItemData(object parameter) => parameter != null;

        #endregion

        #endregion

        #region PublicMethods

        public async Task UpdateContent()
        {
            await Task.Factory.StartNew(() =>
            {
                var result = Client.Get(App.Uri);
                Dispatcher?.Invoke(() => { ShowNamesList(result); });
            });
        }

        #endregion

        #region PrivateMethods

        private void ClearItemInfoView()
        {
            RecognizedObjects.Clear();
            ImageContent.Clear();
        }

        private void ShowNamesList(List<WebImageInfo> result)
        {
            ImagesInfo.Clear();
            foreach (var imageInfo in result)
                ImagesInfo.Add(imageInfo);
        }

        private void RemoveItem_CanExecuteChanged(object sender, EventArgs e) =>
            Dispatcher?.Invoke(() => { RemoveItem?.RaiseCanExecuteChanged(); });

        #endregion
    }
}
