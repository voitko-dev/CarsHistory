using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CarsHistory.Items;

public class ObservableObject : INotifyPropertyChanged
{
    public bool IsModified { get; private set; } = false;

    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;

        field = value;
        IsModified = true;
        OnPropertyChanged(propName);
        return true;
    }

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void ResetModified() => IsModified = false;
}
