using System.IO;
using UIApp.ViewModel.FileEntities;


namespace UIApp.ViewModel.History
{
    public class DirectoryNode
    {
        #region Constructors
        
        public DirectoryNode(string directoryName) =>
            CurrentNode = new DirectoryViewModel(directoryName) { FullName = directoryName};

        public DirectoryNode(DirectoryViewModel directory) => CurrentNode = directory;

        #endregion

        #region Properties

        public DirectoryViewModel CurrentNode { get; set; }

        public DirectoryNode NextNode { get; set; }

        public DirectoryNode PrevNode { get; set; }

        #endregion

        #region Public Methods

        public override string ToString() => CurrentNode.FullName;

        #endregion
    }
}
