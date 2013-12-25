using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Demo.Utils;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public abstract class when_using_MvvmCommandWirer<TCommand, TWireTarget> : when_using_ICommand<TCommand>
        where TCommand : ICommand
        where TWireTarget : WireTargetBase
    {
        protected TWireTarget myWireTarget;

        protected virtual TWireTarget CreateWireTarget ()
        { return Activator.CreateInstance<TWireTarget> (); }

        protected override void Establish_context ()
        {
            base.Establish_context ();

            myWireTarget = CreateWireTarget ();
        }

        protected abstract TCommand GetCommandFromWireTarget ();

        protected override void InitializeCommand ()
        {
            CommandWirer.WireAll (myWireTarget);
            myCommand = GetCommandFromWireTarget ();
        }

        protected override void AssertCanExecuteWasCalled () { Assert.IsTrue (myWireTarget.CanExecuteCalled); }
        protected override void AssertExecuteWasCalled () { Assert.IsTrue (myWireTarget.ExecuteCalled); }
    }

    public abstract class WireTargetBase
    {
        public bool CanExecuteReturnValue { get; set; }

        public bool CanExecuteCalled { get; set; }

        public Object CanExecuteParameter { get; set; }

        public bool ExecuteCalled { get; set; }
     
        public Object ExecuteParameter { get; set; }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_case : when_using_MvvmCommandWirer<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_case.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            private bool CanFoo ()
            {
                CanExecuteCalled = true;
                return CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            private void Foo ()
            {
                ExecuteCalled = true;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_case_with_CanExecute_property : when_using_MvvmCommandWirer<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_case.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            private bool CanFoo
            {
                get
                {
                    CanExecuteCalled = true;
                    return CanExecuteReturnValue;
                }
            }

            [CommandExecuteMethod]
            private void Foo ()
            {
                ExecuteCalled = true;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }
    }
}
