using System.IO;


namespace UIApp.ViewModel.FileEntities
{
    public class DirectoryViewModel : FileEntityViewModel
    {
        public DirectoryViewModel(string name) : base(name) { }

        public DirectoryViewModel(DirectoryInfo directoryInfo) : base(directoryInfo.Name) => 
            FullName = directoryInfo.FullName;

    }
}

