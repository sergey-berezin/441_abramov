using System.ComponentModel;
using UIApp.ViewModel.FileEntities;


namespace UIApp.ViewModel.History
{
    public class DirectoryHistory : IDirectoryHistory
    {
        #region Private Fields

        private readonly DirectoryNode _head;
        
        private DirectoryNode _currentDirectory;

        #endregion

        #region Constructor

        public DirectoryHistory(string headName)
        {
            _head = new DirectoryNode(headName);
            CurrentDirectory = _head;
        }

        #endregion

        #region Properties

        public bool CanMoveBack => CurrentDirectory.PrevNode != null;

        public bool CanMoveForward => CurrentDirectory.NextNode != null;

        public DirectoryNode CurrentDirectory
        {
            get => _currentDirectory;
            private set
            {
                if (_currentDirectory != value)
                {
                    _currentDirectory = value;
                    RaiseHistoryChanged();
                }
            }
        }


        #endregion

        #region Event
        
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Methods

        public void Add(DirectoryViewModel directory)
        {
            var newNode = new DirectoryNode(directory);
            newNode.PrevNode = CurrentDirectory;
            CurrentDirectory.NextNode = newNode;

            MoveForward();
        }

        public void MoveBack() => CurrentDirectory = CurrentDirectory.PrevNode;

        public void MoveForward() => CurrentDirectory = CurrentDirectory.NextNode;

        public bool IsOnRoot() => CurrentDirectory.CurrentNode.FullName == _head.CurrentNode.FullName;

        public void SimulateHistoryChanged() => RaiseHistoryChanged();

        #endregion

        #region Private Methods

        private void RaiseHistoryChanged() => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDirectory)));

        #endregion
    }
}
