﻿using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;
using Whathecode.System.Windows.Input.CommandFactory;
using Whathecode.System.Windows.Input.InputController.Condition;


namespace Whathecode.System.Windows.Input.InputController.Trigger
{
	/// <summary>
	///   A class which triggers a <see cref="CommandFactory" /> command for the DataContext of a <see cref="FrameworkElement" />.
	///   This can be used to bind to data context commands from code-behind.
	/// </summary>
	/// <typeparam name = "TCommand">An enum used to identify the commands.</typeparam>
	/// <author>Steven Jeuris</author>
	public class CommandBindingTrigger<TCommand> : EventTrigger
	{
		readonly TCommand _desiredCommand;
		ICommand _command;
		readonly object _parameter;


		public CommandBindingTrigger( AbstractCondition condition, FrameworkElement element, TCommand command, object parameter = null )
			: base( condition )
		{
			Contract.Requires( condition != null && element != null );

			_desiredCommand = command;
			_parameter = parameter;

			ConditionsMet += TriggerAction;
			element.DataContextChanged += OnDataContextChanged;
		}


		void OnDataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
		{
			object dataContext = e.NewValue;

			_command = CommandFactory<TCommand>.GetCommand( dataContext, _desiredCommand );
		}


		void TriggerAction()
		{
			if ( _command != null && _command.CanExecute( _parameter ) )
			{
				_command.Execute( _parameter );
			}
		}
	}
}
