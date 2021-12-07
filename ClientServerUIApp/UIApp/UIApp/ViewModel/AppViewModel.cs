using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Threading;
using System.ComponentModel;
using UIApp.ViewModel.Commands;
using UIApp.ViewModel.History;
using UIApp.ViewModel.FileEntities;
using UIApp.ViewModel.Collections;
using ClientLib;
using WebClassLib;


namespace UIApp.ViewModel
{
    public class AppViewModel : BaseViewModel
    {
        private WebProcessResult _extendedInfo = null;

        #region Constructor

        public AppViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            History = new DirectoryHistory("MyComputer");
            ImagesList = new ImagesCollection();
            RecognitionResults = new Dictionary<string, WebProcessResult>();
            UniqueCategories = new UniqueCategoriesObservable();
            SpecifiedCategories = new UniqueCategoriesObservable();

            EntitiesList = new ObservableCollection<FileEntityViewModel>();
            GetLogicalDrives();

            ExpandNode = new AsyncCommand(Expand, CanExpand);
            MoveBack = new AsyncCommand(Back, CanBack);
            MoveForward = new AsyncCommand(Forward, CanForward);
            StartProcess = new AsyncCommand(Start, CanStart);
            CancelProcess = new AsyncCommand(Cancel, CanCancel);
            ShowExtendedInfo = new AsyncCommand(ShowExtraInfo, CanShowExtraInfo);
            SelectCategory = new AsyncCommand(SelectObjectsCategory);

            History.PropertyChanged += History_PropertiesChanged;
            StartProcess.CanExecuteChanged += CanCancelChanged;
            StartProcess.PropertyChanged += History_PropertiesChanged;
            ImagesList.PropertyChanged += CanStartChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<FileEntityViewModel> EntitiesList { get; set; }

        public Dispatcher Dispatcher { get; set; }

        public DirectoryHistory History { get; }

        public ImagesCollection ImagesList { get; set; }

        public Dictionary<string, WebProcessResult> RecognitionResults { get; set; }

        public WebProcessResult ExtendedInfo
        {
            get => _extendedInfo;
            set
            {
                if (_extendedInfo != value)
                {
                    _extendedInfo = value;
                    OnPropertyChanged(nameof(ExtendedInfo));
                }
            }

        }

        public UniqueCategoriesObservable UniqueCategories { get; set; } // категории объектов со всех ихображений

        public UniqueCategoriesObservable SpecifiedCategories { get; set; } // выбранные категории объектов       

        #endregion

        #region Commands

        public AsyncCommand ExpandNode { get; }

        public AsyncCommand MoveBack { get; }

        public AsyncCommand MoveForward { get; }

        public AsyncCommand StartProcess { get; }

        public AsyncCommand CancelProcess { get; }

        public AsyncCommand ShowExtendedInfo { get; }

        public AsyncCommand SelectCategory { get; }

        #endregion

        #region CommandHandlers

        #region ExpandCommand Methods

        private void Expand(object parameter)
        {
            Dispatcher?.Invoke(() =>
            {

                var directory = parameter as DirectoryViewModel;                               

                OpenDirectory(directory);

                History.Add(directory);
            });
        }

        private bool CanExpand(object parameter) => !StartProcess.IsExecution && parameter is DirectoryViewModel;

        #endregion

        #region MoveBackCommand Methods

        private void Back(object parameter) =>
            Dispatcher?.Invoke(() =>
            {
                History.MoveBack();
                if (!History.IsOnRoot())
                    OpenDirectory(History.CurrentDirectory.CurrentNode);
                else
                    GetLogicalDrives();
            });

        private bool CanBack(object parameter) => History.CanMoveBack && !StartProcess.IsExecution;

        #endregion

        #region MoveForwardCommand Methods

        private void Forward(object parameter) =>
            Dispatcher?.Invoke(() =>
            {                
                History.MoveForward();
                OpenDirectory(History.CurrentDirectory.CurrentNode);
            });

        private bool CanForward(object parameter) => History.CanMoveForward && !StartProcess.IsExecution;

        #endregion

        #region StartCommand Methods

        private void Start(object parameter)
        {
            string imagesPath = History.CurrentDirectory.CurrentNode.FullName;
            using (var client = new Client())
            {
                foreach (var processResult in client.PostAsync(App.Uri, imagesPath))
                {
                    Dispatcher?.Invoke(() =>
                    {
                        var bitmap = new Bitmap(new MemoryStream(processResult.ByteBitmap));
                        ImagesList[processResult.ImageName].SourceChanged(bitmap);
                        
                        if (StartProcess.IsCanceled)
                            client.Cancel(App.Uri);

                        RecognitionResults[processResult.ImageName] = processResult;
                    });
                }
            }

            Dispatcher?.Invoke(() =>
            { 
                SelectUniqueCategories(); 
            });
        }

        private bool CanStart(object parameter) => !ImagesList.IsEmpty;

        #endregion

        #region CancelCommand Methods

        private void Cancel(object parameter)
        {
            Dispatcher?.Invoke(() => { StartProcess.Cancel(); });
        }

        private bool CanCancel(object parameter) => StartProcess.IsExecution;

        #endregion

        #region ShowExtraInfo Methods

        private void ShowExtraInfo(object parameter)
        {
            var image = (KeyValuePair<string, ImageViewModel>)parameter;
            var imageName = image.Key;
            if (RecognitionResults.Count > 0 && RecognitionResults.ContainsKey(imageName))
            {
                ExtendedInfo = RecognitionResults[imageName];
            }
        }

        private bool CanShowExtraInfo(object parameter) => parameter != null;

        #endregion

        #region SelectCategory Methods

        private void SelectObjectsCategory(object parameter) =>
            Dispatcher?.Invoke(() => 
            {
                SpecifiedCategories.Clear();
                IList items = (IList)parameter;
                var categories = items.Cast<string>();
                foreach (var category in categories)
                    SpecifiedCategories.Add(category);

                ImagesList.SimulateCollectionChanged(); // лайфхак для обновления представления коллекции :)
            });

        #endregion

        #endregion

        #region Public Methods

        public bool SelectSpecified(object argsItem)
        {
            var item = (KeyValuePair<string, ImageViewModel>)argsItem;
            bool result = true;
            if (SpecifiedCategories != null && SpecifiedCategories.Count > 0)
            {
                result = false;
                foreach (string category in SpecifiedCategories)
                {
                    if (RecognitionResults.ContainsKey(item.Key) &&
                        RecognitionResults[item.Key].RecognitionObjects.Select(obj => obj.Key).Contains(category))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Private Methods

        private void OpenDirectory(DirectoryViewModel directory)
        {
            EntitiesList.Clear();
            ImagesList.Clear();
            RecognitionResults.Clear();
            UniqueCategories.Clear();
            ExtendedInfo = null;
            
            foreach (var directoryName in Directory.GetDirectories(directory.FullName))
                EntitiesList.Add(new DirectoryViewModel(new DirectoryInfo(directoryName)));
            foreach (var fileName in Directory.GetFiles(directory.FullName))
            {
                var fileInfo = new FileInfo(fileName);
                var extensions = new ObservableCollection<string>() { ".png", ".jpg" };                
                if (extensions.Contains(fileInfo.Extension))
                {
                    var img = new ImageViewModel(fileInfo);
                    ImagesList.Add(fileInfo.Name, img);
                    OnPropertyChanged(nameof(ImagesList));
                    EntitiesList.Add(img);
                }
                else
                    EntitiesList.Add(new FileViewModel(fileInfo));               
            }
        }

        private void GetLogicalDrives()
        {
            EntitiesList.Clear();
            ImagesList.Clear();
            foreach (var logicalDrive in Directory.GetLogicalDrives())
                EntitiesList.Add(new DirectoryViewModel(new DirectoryInfo(logicalDrive)));
        }

        private void SelectUniqueCategories()
        {
            foreach (var processResult in RecognitionResults.Values)
            {
                foreach (var category in processResult.RecognitionObjects?.Select(obj => obj.Key))
                    UniqueCategories.Add(category);
            }
        }

        private void History_PropertiesChanged(object sender, PropertyChangedEventArgs e)
        {
            MoveBack?.RaiseCanExecuteChanged();
            MoveForward?.RaiseCanExecuteChanged();
        }

        private void CanCancelChanged(object sender, EventArgs e) =>
            Dispatcher?.Invoke(() => { CancelProcess?.RaiseCanExecuteChanged(); });

        private void CanStartChanged(object sender, PropertyChangedEventArgs e) =>
            Dispatcher?.Invoke(() => { StartProcess?.RaiseCanExecuteChanged(); });

        #endregion

    }
}