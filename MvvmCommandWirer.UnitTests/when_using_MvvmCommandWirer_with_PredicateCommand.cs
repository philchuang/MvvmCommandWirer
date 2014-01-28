using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Microsoft.Practices.Prism.Commands;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
	public class when_using_MvvmCommandWirer_with_PredicateCommand :
		when_using_MvvmCommandWirer_successfully<PredicateCommand, when_using_MvvmCommandWirer_with_PredicateCommand_ViewModel>
    {
		protected override PredicateCommand GetCommandFromWireTarget () { return (PredicateCommand) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
			Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.CanFoo (null)), wirer.CanExecuteMethod);
			Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo (null)), wirer.ExecuteMethod);
        }
    }

	public class when_using_MvvmCommandWirer_with_PredicateCommand_ViewModel : WireTargetBase
	{
		[CommandProperty (commandType: typeof (PredicateCommand), paramType: typeof (Object))]
		public ICommand FooCommand { get; set; }

		[CommandCanExecuteMethod]
		public bool CanFoo (Object parameter)
		{
			CanExecuteCalled = true;
			CanExecuteParameter = parameter;
			return CanExecuteReturnValue;
		}

		[CommandExecuteMethod]
		public void Foo (Object parameter)
		{
			ExecuteCalled = true;
			ExecuteParameter = parameter;
		}
	}

	public class PredicateCommand<T> : ICommand
	{
		private readonly Action<T> m_Execute;
		private readonly Predicate<T> m_CanExecute;

		public PredicateCommand (Action<T> execute, Predicate<T> canExecute)
		{
			m_CanExecute = canExecute;
			m_Execute = execute;
			execute.ThrowIfNull ("execute");
			canExecute.ThrowIfNull ("canExecute");
		} 

		public bool CanExecute (object parameter) { return CanExecute ((T) parameter); }

		public bool CanExecute (T parameter) { return m_CanExecute (parameter); }

		public void Execute (object parameter) { Execute ((T) parameter); }
		
		public void Execute (T parameter) { m_Execute (parameter); }

		public event EventHandler CanExecuteChanged = delegate { };

		public void RaiseCanExecuteChanged () { CanExecuteChanged (this, EventArgs.Empty); }
	}

	public class PredicateCommand : PredicateCommand<Object>
	{
		public PredicateCommand (Action<Object> execute, Predicate<Object> canExecute) : base (execute, canExecute)
		{
		}
	}
}