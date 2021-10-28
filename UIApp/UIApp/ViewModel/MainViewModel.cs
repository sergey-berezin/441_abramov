using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Threading;
using System.ComponentModel;
using UIApp.ViewModel.Commands;
using UIApp.ViewModel.History;
using UIApp.ViewModel.FileEntities;
using UIApp.ViewModel.Collections;
using ParallelYOLOv4;


namespace UIApp.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private bool _directoryIsHandled = false;
        private ProcessResult _extendedInfo = null;

        #region Constructor

        public MainViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            History = new DirectoryHistory("MyComputer");
            ImagesList = new ImagesCollection();
            RecognitionResults = new Dictionary<string, ProcessResult>();
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
            ImagesList.PropertyChanged += CanStartChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<FileEntityViewModel> EntitiesList { get; set; }

        public Dispatcher Dispatcher { get; set; }

        public DirectoryHistory History { get; }
        
        public ImagesCollection ImagesList { get; set; }   
        
        public Dictionary<string, ProcessResult> RecognitionResults { get; set; }

        public ProcessResult ExtendedInfo
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

        private async void Start(object parameter)
        {
            //Dispatcher?.Invoke(() => { History.SimulateHistoryChanged(); });

            PictureProcessing pictureProcessing = new PictureProcessing();
            string imagesPath = History.CurrentDirectory.CurrentNode.FullName;
            await foreach (var processResult in pictureProcessing.ProcessImagesAsync(imagesPath))
            {
                Dispatcher?.Invoke(() =>
                {
                    ImagesList[processResult.ImageName].SourceChanged(processResult.Bitmap);
                    /*if (ImagesList.ContainsKey(processResult.ImageName))
                        ImagesList[processResult.ImageName].SourceChanged(processResult.Bitmap);
                    else
                        History.ProcessedDirectoryImages[imagesPath][processResult.ImageName].SourceChanged(processResult.Bitmap);*/
                    if (StartProcess.IsCanceled)
                        pictureProcessing.Cancel();

                    RecognitionResults[processResult.ImageName] = processResult;

                    _directoryIsHandled = true;
                });
            }

            Dispatcher?.Invoke(() =>
            { 
                SelectUniqueCategories(); 
                //History.SimulateHistoryChanged(); 
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
            if (RecognitionResults.Count > 0)
            {                
                var image = (KeyValuePair<string, ImageViewModel>)parameter;
                var imageName = image.Key;
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
                        RecognitionResults[item.Key].Categories.Select(cat => cat.ObjName).Contains(category))
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
            //SaveHandledImages(); // save ImagesList to History if images in prev directory was handled

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
                    if (!History.ProcessedDirectoryImages.ContainsKey(directory.FullName))
                        ImagesList.Add(fileInfo.Name, img);
                    else
                        ImagesList.Add(fileInfo.Name,
                            History.ProcessedDirectoryImages[directory.FullName][fileInfo.Name]);
                    OnPropertyChanged(nameof(ImagesList));
                    EntitiesList.Add(img);
                }
                else
                    EntitiesList.Add(new FileViewModel(fileInfo));               
            }
        }

        private void GetLogicalDrives()
        {
            //SaveHandledImages();

            EntitiesList.Clear();
            ImagesList.Clear();
            foreach (var logicalDrive in Directory.GetLogicalDrives())
                EntitiesList.Add(new DirectoryViewModel(new DirectoryInfo(logicalDrive)));
        }

        private void SaveHandledImages()
        {            
            if (_directoryIsHandled)
            {
                var imgFullName = ImagesList[0].FullName;
                var directoryName = imgFullName.Substring(0, imgFullName.LastIndexOf(Path.DirectorySeparatorChar));
                History.UpdateProcessedImagesCollection(directoryName, ImagesList);
            }
            _directoryIsHandled = false;
        }        

        private void SelectUniqueCategories()
        {
            foreach (var processResult in RecognitionResults.Values)
            {
                foreach (var category in processResult.Categories)
                    UniqueCategories.Add(category.ObjName);
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