namespace UIApp.ViewModel.FileEntities
{
    public abstract class FileEntityViewModel : BaseViewModel
    {
        protected FileEntityViewModel() { }
        
        protected FileEntityViewModel(string name) => Name = name;

        public string Name { get; set; }

        public string FullName { get; set; }
    }
}

