using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SearcherWinUI
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		[DllImport("user32.dll")]
		private static extern bool IsZoomed(IntPtr hwnd);

		public MainWindow()
		{
			this.InitializeComponent();
			this.Content = new Frame();
			(this.Content as Frame).Navigate(typeof(MainPage));
			this.SizeChanged += MainWindow_SizeChanged;
		}

		private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
		{
			var hWnd = WindowNative.GetWindowHandle(this);
			if (hWnd != IntPtr.Zero)
			{
				
			}
		}
	}
}
