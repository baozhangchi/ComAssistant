using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ComAssistant.ViewModels;
using ComAssistant.Views;

namespace ComAssistant;

public class ViewManager : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param == null) return null;
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        //var type = Type.GetType(name);

        Control view = param switch
        {
            MainViewModel => new MainView(),
            ComDebugViewModel => new ComDebugView(),
            HistoryItemViewModel => new HistoryItemView(),
            _ => new TextBlock { Text = "Not Found: " + name }
        };

        view.DataContext = param;
        if (param is IViewModel viewModel)
        {
            viewModel.Attach(view);
        }
        return view;

    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}

public class WindowManager
{
    public void Show(object viewModel)
    {
        CreateWindow(viewModel).Show();
    }

    public Task ShowDialog(object viewModel, Window owner)
    {
        return CreateWindow(viewModel).ShowDialog(owner);
    }

    public Task ShowDialog<T>(object viewModel, Window owner)
    {
        return CreateWindow(viewModel, true).ShowDialog<T>(owner);
    }

    public Window CreateWindow(object viewModel, bool isDialog = false)
    {
        var view = new ViewManager().Build(viewModel);
        if (view is not Window window)
            throw new InvalidOperationException("The view model does not have a corresponding window.");
        if (isDialog)
        {

        }
        return window;

    }
}