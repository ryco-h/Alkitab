using System.ComponentModel;

public class ToggleState : INotifyPropertyChanged
{
    private string? _target = "";

    public string? Target
    {
        get => _target;
        set
        {
            if (_target != value)
            {
                _target = value;
                OnPropertyChanged(nameof(Target));
            }
        }
    }

    private string? _pasalText;

    public string? PasalText
    {
        get => _pasalText;
        set
        {
            if (_pasalText != value)
            {
                _pasalText = value;
                OnPropertyChanged(nameof(PasalText));
            }
        }
    }

    private string? _pasal;

    public string? Pasal
    {
        get => _pasal;
        set
        {
            if (_pasal != value)
            {
                _pasal = value;
                OnPropertyChanged(nameof(Pasal));
            }
        }
    }

    private string? _ayatText;

    public string? AyatText
    {
        get => _ayatText;
        set
        {
            if (_ayatText != value)
            {
                _ayatText = value;
                OnPropertyChanged(nameof(AyatText));
            }
        }
    }

    private string? _ayat;

    public string? Ayat
    {
        get => _ayat;
        set
        {
            if (_ayat != value)
            {
                _ayat = value;
                OnPropertyChanged(nameof(Ayat));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}