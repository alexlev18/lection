using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapleElectionSim.App.ViewModels;

/// <summary>
/// Basic async-aware command implementation.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Func<object?, bool>? _canExecute;
    private readonly Func<object?, Task>? _executeAsync;
    private readonly Action<object?>? _execute;

    public RelayCommand(Action execute)
    {
        _execute = _ => execute();
    }

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public RelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null)
    {
        _executeAsync = executeAsync;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public async void Execute(object? parameter)
    {
        if (_executeAsync != null)
        {
            await _executeAsync(parameter);
        }
        else
        {
            _execute?.Invoke(parameter);
        }
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
