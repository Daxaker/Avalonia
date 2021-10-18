using System.Reactive;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using Blazor_Desktop;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace AvaloniaDockApplication
{
	public class GraphicEngineHostControl : NativeControlHost
	{
		private GraphicEngine _graphicEngine;
		private Timer _timer;
		private Sdl2Window _window;

		protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
		{
			var wci = new WindowCreateInfo
			{
				X = 0, Y = 0, WindowWidth = 200, WindowHeight = 200
			};
			_window = VeldridStartup.CreateWindow(ref wci);
			_window.BorderVisible = false;
			_window.Resizable = true;

			_graphicEngine = new GraphicEngine(_window);
			_timer = new Timer(32);
			_timer.Elapsed += timerOnElapsed;
			_timer.Start();
			BoundsProperty.Changed.AddClassHandler<GraphicEngineHostControl>(OnResize);
			return new PlatformHandle(_window.Handle, "HWND");
		}

		private void OnResize(GraphicEngineHostControl ths, AvaloniaPropertyChangedEventArgs e)
		{
			_graphicEngine.Resize((uint)Bounds.Width, (uint)Bounds.Height);
		}

		private void timerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.UIThread.Post(() =>
				_graphicEngine.Render()
			);
		}

		protected override void DestroyNativeControlCore(IPlatformHandle control)
		{
			_timer.Stop();
			_timer.Elapsed -= timerOnElapsed;
			_window.Close();
		}
	}
}
