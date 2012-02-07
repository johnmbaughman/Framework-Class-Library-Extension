﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Whathecode.System.Runtime.InteropServices;


namespace Whathecode.System.Windows.Interop
{
	/// <summary>
	///   Information about an application window.
	/// </summary>
	/// <author>Steven Jeuris</author>
	public class WindowInfo
	{
		const int MaxClassnameLength = 128;	// TODO: Is there an actual maximum class name length?
		readonly Dictionary<User32.WindowState, WindowState> _windowStateMapping = new Dictionary<User32.WindowState, WindowState>
		{
			{ User32.WindowState.ShowNormal, WindowState.Open },
			{ User32.WindowState.Maximized, WindowState.Maximized },
			{ User32.WindowState.ShowMinimized, WindowState.Minimized }
		};

		readonly IntPtr _handle;


		/// <summary>
		///   Create a new WindowInfo object for the specified window handle.
		/// </summary>
		/// <param name="handle">The handle of the window to create a WindowInfo object for.</param>
		public WindowInfo( IntPtr handle )
		{
			_handle = handle;
		}


		/// <summary>
		///   Retrieves the name of the class to which the specified window belongs.
		/// </summary>
		public string GetClassName()
		{
			var buffer = new StringBuilder( MaxClassnameLength );
			if ( User32.GetClassName( _handle, buffer, buffer.Capacity ) == 0 )
			{
				MarshalHelper.ThrowLastWin32ErrorException();
			}

			return buffer.ToString();
		}

		/// <summary>
		///   Retrieves the process that created the window, when available.
		/// </summary>
		/// <returns>The process when available, null otherwise.</returns>
		public Process GetProcess()
		{
			int processId = 0;
			if ( User32.GetWindowThreadProcessId( _handle, ref processId ) == 0 )
			{
				MarshalHelper.ThrowLastWin32ErrorException();
			}

			return processId == 0
				? null
				: Process.GetProcessById(processId);
		}

		/// <summary>
		///   Retrieves the text in the window's title bar.
		/// </summary>
		public string GetTitle()
		{
			int length = User32.GetWindowTextLength( _handle );
			var buffer = new StringBuilder( length + 1 );
			if ( User32.GetWindowText( _handle, buffer, buffer.Capacity ) == 0 )
			{
				// The window might as well have no title/empty title,
				// but no exception will be thrown in that scenario since no error code is set.
				MarshalHelper.ThrowLastWin32ErrorException();
			}

			return buffer.ToString();
		}

		/// <summary>
		///   Retrieves the current state of the window.
		/// </summary>
		/// <returns></returns>
		public WindowState GetWindowState()
		{
			User32.WindowPlacement placement = GetWindowPlacement();
			var state = (User32.WindowState)placement.ShowCommand;

			return EnumHelper<User32.WindowState>.Convert( state, _windowStateMapping );
		}

		/// <summary>
		///   Retrieves placement information of a window in a User32.dll structure.
		/// </summary>
		User32.WindowPlacement GetWindowPlacement()
		{
			User32.WindowPlacement placement = User32.WindowPlacement.Default;
			if ( !User32.GetWindowPlacement( _handle, ref placement ) )
			{
				MarshalHelper.ThrowLastWin32ErrorException();
			}

			return placement;
		}

		/// <summary>
		///   Hides the window and activates another window.
		/// </summary>
		public void Hide()
		{
			User32.ShowWindow( _handle, User32.WindowState.Hide );
		}

		/// <summary>
		///   Verifies whether this window has been destroyed or not.
		/// </summary>
		/// <returns>True when the window no longer exists; false otherwise.</returns>
		public bool IsDestroyed()
		{
			return !User32.IsWindow( _handle );
		}

		/// <summary>
		///   Determines the visibility state of the window.
		/// </summary>
		public bool IsVisible()
		{
			return User32.IsWindowVisible( _handle );
		}

		/// <summary>
		///   Activates the window and displays it as a maximized window.
		/// </summary>
		public void Maximize()
		{
			User32.ShowWindow( _handle, User32.WindowState.ShowMaximized );
		}

		/// <summary>
		///   Minimizes the window and activates the next top-level window in the z-order.
		/// </summary>
		public void Minimize()
		{
			User32.ShowWindow( _handle, User32.WindowState.Minimize );
		}

		/// <summary>
		///   Activates the window and displays it in its current size and position.
		/// </summary>
		/// <param name="activate">
		///	  When set to true, the window will be brought to the front and activated.
		///   Otherwise it will stay in it's previous state, e.g. minimized.
		/// </param>
		public void Show( bool activate = true )
		{
			User32.ShowWindow(
				_handle,
				activate ? User32.WindowState.Show : User32.WindowState.ShowNoActivate );
		}

		public override bool Equals( object other )
		{
			var otherWindow = other as WindowInfo;
			if ( otherWindow == null )
			{
				return false;
			}

			return _handle == otherWindow._handle;
		}

		public bool Equals( WindowInfo other )
		{
			if ( ReferenceEquals( null, other ) )
			{
				return false;
			}

			return ReferenceEquals( this, other ) || other._handle.Equals( _handle );
		}

		public override int GetHashCode()
		{
			return _handle.GetHashCode();
		}
	}
}
