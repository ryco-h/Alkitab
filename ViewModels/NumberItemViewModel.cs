using System.Windows.Input;

namespace Alkitab.ViewModels;

public class NumberItemViewModel
{
    public string Value { get; }
    public ICommand ClickCommand { get; }

    public NumberItemViewModel(string value, ICommand command)
    {
        Value = value;
        ClickCommand = command;
    }
}