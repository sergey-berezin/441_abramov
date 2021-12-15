using System;
using System.ComponentModel;
using UIApp.ViewModel.FileEntities;


namespace UIApp.ViewModel.History
{
    interface IDirectoryHistory : INotifyPropertyChanged
    {
        bool CanMoveBack { get; }
        bool CanMoveForward { get; }
        DirectoryNode CurrentDirectory { get; }

        void MoveBack();
        void MoveForward();
        void Add(DirectoryViewModel directory);
    }
}
