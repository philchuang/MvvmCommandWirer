using System;
using System.Reflection;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Microsoft.Practices.Prism.Commands;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInitializationMethodAttribute_no_parameter :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInitializationMethodAttribute_no_parameter.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            public bool InitializeFooCommandWasCalled { get; set; }

            public Object InitializeFooCommandParameter { get; set; }

            [CommandInitializationMethod]
            internal void InitializeFooCommand ()
            {
                InitializeFooCommandWasCalled = true;
            }

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
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.InitializeFooCommand ()), BindingFlags.NonPublic | BindingFlags.Instance), wirer.InitializationMethod);
        }

        [Test]
        public void then_initialization_method_should_be_called ()
        {
            Assert.IsTrue (myWireTarget.InitializeFooCommandWasCalled);
        }

        [Test]
        public void then_initialization_parameter_should_be_null ()
        {
            Assert.IsNull (myWireTarget.InitializeFooCommandParameter);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInitializationMethodAttribute_with_parameter :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInitializationMethodAttribute_with_parameter.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            public bool InitializeFooCommandWasCalled { get; set; }

            public Object InitializeFooCommandParameter { get; set; }

            [CommandInitializationMethod]
            internal void InitializeFooCommand (ICommand command)
            {
                InitializeFooCommandWasCalled = true;
                InitializeFooCommandParameter = command;
            }

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
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.InitializeFooCommand (null)), BindingFlags.NonPublic | BindingFlags.Instance), wirer.InitializationMethod);
        }

        [Test]
        public void then_initialization_method_should_be_called ()
        {
            Assert.IsTrue (myWireTarget.InitializeFooCommandWasCalled);
        }

        [Test]
        public void then_initialization_parameter_should_match_Command ()
        {
            Assert.AreSame (myCommand, myWireTarget.InitializeFooCommandParameter);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInitializationMethodAttribute_with_wrong_parameters :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInitializationMethodAttribute_with_wrong_parameters.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            public bool InitializeFooCommandWasCalled { get; set; }

            public Object InitializeFooCommandParameter { get; set; }

            [CommandInitializationMethod]
            internal void InitializeFooCommand (ICommand command, Object somethingElse)
            { throw new Exception ("This code should be unreachable"); }

            [CommandCanExecuteMethod]
            internal bool CanFoo ()
            { throw new Exception ("This code should be unreachable"); }

            [CommandExecuteMethod]
            internal void Foo ()
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException =
                new InvalidOperationException ("CommandInitializationMethodAttribute target \"{0}\" can have either no parameters or a single parameter for the ICommand instance."
                                                   .FormatWith (Extensions.GetMethodName (() => myWireTarget.InitializeFooCommand (null, null))));
        }
    }

    public class when_using_MvvmCommandWirer_with_invalid_CommandInitializationMethodAttribute_on_property :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_invalid_CommandInitializationMethodAttribute_on_property.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandInitializationMethod]
            internal DelegateCommand InitializeFooCommand
            { get { throw new Exception ("This code should be unreachable"); } }

            [CommandCanExecuteMethod]
            internal bool CanFoo
            { get { throw new Exception ("This code should be unreachable"); } }

            [CommandExecuteMethod]
            internal void Foo ()
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandInitializationMethodAttribute must be applied to a method, not property \"{0}\"."
                                                                              .FormatWith (Extensions.GetPropertyName (() => myWireTarget.InitializeFooCommand)));
        }
    }
}