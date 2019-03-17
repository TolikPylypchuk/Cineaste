using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MovieList.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string name = "")
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
