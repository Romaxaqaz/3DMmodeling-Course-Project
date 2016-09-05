using System.Windows.Input;

namespace _3DCourseProject.Common
{
    public interface IChangedCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
