using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;

namespace UIApp.ViewModel.Commands
{
    public interface IAsyncCommand : ICommand, INotifyPropertyChanged
    {
        Task ExecuteAsync(object parameter);
    }
}
