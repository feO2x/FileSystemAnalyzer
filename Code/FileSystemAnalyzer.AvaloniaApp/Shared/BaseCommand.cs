using System;
using System.Windows.Input;

namespace FileSystemAnalyzer.AvaloniaApp.Shared;

public abstract class BaseCommand : ICommand
{
    public virtual bool CanExecute() => true;

    public abstract void Execute();
    
    bool ICommand.CanExecute(object? parameter) => CanExecute();

    void ICommand.Execute(object? parameter) => Execute();
    
    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;
}