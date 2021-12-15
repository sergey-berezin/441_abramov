using System.IO;


namespace UIApp.ViewModel.FileEntities
{
    public class FileViewModel : FileEntityViewModel
    {
        public FileViewModel() : base() { }

        public FileViewModel(string name) : base(name) { }

        public FileViewModel(FileInfo fileInfo) : base(fileInfo.Name) => 
            FullName = fileInfo.FullName;
    }
}
