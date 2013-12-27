using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Demo.Utils;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            public bool CanFoo ()
            {
                CanExecuteCalled = true;
                return CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            public void Foo ()
            {
                ExecuteCalled = true;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.FooCommand)), wirer.CommandProperty);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.CanFoo ())), wirer.CanExecuteMethod);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.Foo ())), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_nonpublic : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_nonpublic.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo ()
            {
                CanExecuteCalled = true;
                return CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            internal void Foo ()
            {
                ExecuteCalled = true;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.FooCommand)), wirer.CommandProperty);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.CanFoo ()), BindingFlags.NonPublic | BindingFlags.Instance), wirer.CanExecuteMethod);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.Foo ()), BindingFlags.NonPublic | BindingFlags.Instance), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_public_CanExecute_property : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_public_CanExecute_property.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            public bool CanFoo
            {
                get
                {
                    CanExecuteCalled = true;
                    return CanExecuteReturnValue;
                }
            }

            [CommandExecuteMethod]
            internal void Foo ()
            {
                ExecuteCalled = true;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.FooCommand)), wirer.CommandProperty);
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.CanFoo)).GetGetMethod (), wirer.CanExecuteMethod);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.Foo ()), BindingFlags.NonPublic | BindingFlags.Instance), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_nonpublic_CanExecute_property : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_nonpublic_CanExecute_property.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo
            {
                get
                {
                    CanExecuteCalled = true;
                    return CanExecuteReturnValue;
                }
            }

            [CommandExecuteMethod]
            internal void Foo ()
            {
                ExecuteCalled = true;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.FooCommand)), wirer.CommandProperty);
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.CanFoo), BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod (true), wirer.CanExecuteMethod);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.Foo ()), BindingFlags.NonPublic | BindingFlags.Instance), wirer.ExecuteMethod);
        }
    }
}