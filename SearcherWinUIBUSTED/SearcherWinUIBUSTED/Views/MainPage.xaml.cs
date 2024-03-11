using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;

using SearcherWinUI.ViewModels;

namespace SearcherWinUI.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    /// <summary>
    /// The DispatcherQueue for running code which relies on the UI thread.
    /// </summary>
	public DispatcherQueue DispatcherQueue { get; }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();

        InitializeComponent();

        DispatcherQueue = DispatcherQueue.GetForCurrentThread();
	}
}
