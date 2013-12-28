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
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.CanFoo ()), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo ()), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_async :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_async.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            public ManualResetEvent FooAsyncMre = new ManualResetEvent (false);

            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            public bool CanFoo ()
            {
                CanExecuteCalled = true;
                return CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            public async void FooAsync ()
            {
                await Task.Delay (1);
                ExecuteCalled = true;
                FooAsyncMre.Set ();
            }
        }

        protected override void WaitUntilExecuteIsFinished () { myWireTarget.FooAsyncMre.WaitOne (); }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.CanFoo ()), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.FooAsync ()), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            public bool CanFoo (String parameter)
            {
                CanExecuteCalled = true;
                CanExecuteParameter = parameter;
                return CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            public void Foo (String parameter)
            {
                ExecuteCalled = true;
                ExecuteParameter = parameter;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand<String>) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.FooCommand)), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.CanFoo (null)), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo (null)), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_async :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_async.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            public ManualResetEvent FooAsyncMre = new ManualResetEvent (false);

            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            public bool CanFoo (String parameter)
            {
                CanExecuteCalled = true;
                CanExecuteParameter = parameter;
                return CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            public async void FooAsync (String parameter)
            {
                await Task.Delay (1);
                ExecuteCalled = true;
                ExecuteParameter = parameter;
                FooAsyncMre.Set ();
            }
        }

        protected override void WaitUntilExecuteIsFinished () { myWireTarget.FooAsyncMre.WaitOne (); }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand<String>) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.CanFoo (null)), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.FooAsync (null)), wirer.ExecuteMethod);
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
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.CanFoo ()), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo ()), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_nonpublic : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_nonpublic.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo (String parameter)
            {
                CanExecuteCalled = true;
                CanExecuteParameter = parameter;
                return CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            internal void Foo (String parameter)
            {
                ExecuteCalled = true;
                ExecuteParameter = parameter;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand<String>) myWireTarget.FooCommand; }

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

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_no_CanExecute :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_no_CanExecute.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandExecuteMethod]
            internal void Foo ()
            {
                ExecuteCalled = true;
            }
        }

        protected override void Establish_context ()
        {
            base.Establish_context ();
            myCanExecuteExpected = true;
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }

        public override void then_CanExecute_should_be_called ()
        {
            // don't need to test since there's nothing to call
        }

        public override void then_CanExecute_parameter_should_match ()
        {
            // don't need to test since there's nothing to call
        }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.IsNull (wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo ()), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_no_CanExecute :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_no_CanExecute.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandExecuteMethod]
            internal void Foo (String parameter)
            {
                ExecuteCalled = true;
                ExecuteParameter = parameter;
            }
        }

        protected override void Establish_context ()
        {
            base.Establish_context ();
            myCanExecuteExpected = true;
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand<String>) myWireTarget.FooCommand; }

        public override void then_CanExecute_should_be_called ()
        {
            // don't need to test since there's nothing to call
        }

        public override void then_CanExecute_parameter_should_match ()
        {
            // don't need to test since there's nothing to call
        }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.IsNull (wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo (null)), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_nonpublic_static :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_nonpublic_static.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public static ICommand FooCommand { get; set; }

            private static bool s_CanExecuteCalled;
            public override bool CanExecuteCalled
            {
                get { return s_CanExecuteCalled; }
                set { s_CanExecuteCalled = value; }
            }

            private static bool s_CanExecuteReturnValue;
            public override bool CanExecuteReturnValue
            {
                get { return s_CanExecuteReturnValue; }
                set { s_CanExecuteReturnValue = value; }
            }

            private static bool s_ExecuteCalled;
            public override bool ExecuteCalled
            {
                get { return s_ExecuteCalled; }
                set { s_ExecuteCalled = value; }
            }

            [CommandCanExecuteMethod]
            internal static bool CanFoo ()
            {
                s_CanExecuteCalled = true;
                return s_CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            internal static void Foo ()
            {
                s_ExecuteCalled = true;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) ViewModel.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => ViewModel.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetMethodInfo (() => ViewModel.CanFoo ()), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => ViewModel.Foo ()), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_nonpublic_static :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_nonpublic_static.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public static ICommand FooCommand { get; set; }

            private static bool s_CanExecuteCalled;
            public override bool CanExecuteCalled
            {
                get { return s_CanExecuteCalled; }
                set { s_CanExecuteCalled = value; }
            }

            private static Object s_CanExecuteParameter;
            public override Object CanExecuteParameter
            {
                get { return s_CanExecuteParameter; }
                set { s_CanExecuteParameter = value; }
            }

            private static bool s_CanExecuteReturnValue;
            public override bool CanExecuteReturnValue
            {
                get { return s_CanExecuteReturnValue; }
                set { s_CanExecuteReturnValue = value; }
            }

            private static bool s_ExecuteCalled;
            public override bool ExecuteCalled
            {
                get { return s_ExecuteCalled; }
                set { s_ExecuteCalled = value; }
            }

            private static Object s_ExecuteParameter;
            public override Object ExecuteParameter
            {
                get { return s_ExecuteParameter; }
                set { s_ExecuteParameter = value; }
            }

            [CommandCanExecuteMethod]
            internal static bool CanFoo (String parameter)
            {
                s_CanExecuteCalled = true;
                s_CanExecuteParameter = parameter;
                return s_CanExecuteReturnValue;
            }

            [CommandExecuteMethod]
            internal static void Foo (String parameter)
            {
                s_ExecuteCalled = true;
                s_ExecuteParameter = parameter;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand<String>) ViewModel.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => ViewModel.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetMethodInfo (() => ViewModel.CanFoo (null)), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => ViewModel.Foo (null)), wirer.ExecuteMethod);
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
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.CanFoo).GetGetMethod (), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo ()), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_with_public_CanExecute_property : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_with_public_CanExecute_property.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
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
            internal void Foo (String parameter)
            {
                ExecuteCalled = true;
                ExecuteParameter = parameter;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand<String>) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.CanFoo).GetGetMethod (), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo (null)), wirer.ExecuteMethod);
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
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.CanFoo).GetGetMethod (true), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo ()), wirer.ExecuteMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_with_nonpublic_CanExecute_property : 
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_simple_with_nonpublic_CanExecute_property.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
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
            internal void Foo (String parameter)
            {
                ExecuteCalled = true;
                ExecuteParameter = parameter;
            }
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand<String>) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.FooCommand), wirer.CommandProperty);
            Assert.AreEqual (Extensions.GetPropertyInfo (() => myWireTarget.CanFoo).GetGetMethod (true), wirer.CanExecuteMethod);
            Assert.AreEqual (Extensions.GetMethodInfo (() => myWireTarget.Foo (null)), wirer.ExecuteMethod);
        }
    }
}