using System;
using System.Reflection;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Demo.Utils;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInstantiationMethodAttribute :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_CommandInstantiationMethodAttribute.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty]
            public ICommand FooCommand { get; set; }

            [CommandInstantiationMethodAttribute]
            internal DelegateCommand InstantiateFooCommand (Action execute, Func<bool> canExecute)
            { return new DelegateCommand (execute, canExecute); }

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
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.InstantiateFooCommand (null, null)), BindingFlags.NonPublic | BindingFlags.Instance), wirer.InstantiationMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_with_CommandInstantiationMethodAttribute :
        when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_with_CommandInstantiationMethodAttribute.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty(paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandInstantiationMethodAttribute]
            internal DelegateCommand<String> InstantiateFooCommand (Action<String> execute, Func<String, bool> canExecute)
            { return new DelegateCommand<String> (execute, canExecute); }

            [CommandCanExecuteMethod]
            internal bool CanFoo (String parameter)
            {
                CanExecuteCalled = true;
                CanExecuteParameter = parameter;
                return CanExecuteReturnValue;
            }

            public String FooParameter { get; set; }

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
            myCommandParameter = Guid.NewGuid ().ToString ();
        }

        protected override DelegateCommand GetCommandFromWireTarget () { return (DelegateCommand) myWireTarget.FooCommand; }

        protected override void AssertWireAllResultsMatch ()
        {
            Assert.IsNotNull (myWireAllResults);
            Assert.AreEqual (1, myWireAllResults.Count);

            var wirer = myWireAllResults[0];
            Assert.AreEqual (typeof (ViewModel).GetProperty (Extensions.GetPropertyName (() => myWireTarget.FooCommand)), wirer.CommandProperty);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.CanFoo (null)), BindingFlags.NonPublic | BindingFlags.Instance), wirer.CanExecuteMethod);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.Foo (null)), BindingFlags.NonPublic | BindingFlags.Instance), wirer.ExecuteMethod);
            Assert.AreEqual (typeof (ViewModel).GetMethod (Extensions.GetMethodName (() => myWireTarget.InstantiateFooCommand (null, null)), BindingFlags.NonPublic | BindingFlags.Instance), wirer.InstantiationMethod);
        }
    }

    public class when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute_on_property : 
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute_on_property.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandInstantiationMethod]
            internal DelegateCommand InstantiateFooCommand
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
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandInstantiationMethodAttribute must be applied to a method, not property \"{0}\"."
                                                                              .FormatWith (Extensions.GetPropertyName (() => myWireTarget.InstantiateFooCommand)));
        }
    }

    public class when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute_wrong_signature :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute_wrong_signature.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandInstantiationMethodAttribute]
            internal DelegateCommand InstantiateFooCommand ()
            { throw new Exception ("This code should be unreachable"); }

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
            m_ExpectedBecauseOfException =
                new InvalidOperationException ("CommandInstantiationMethodAttribute target \"{0}\" must have 2 parameters: (Action commandExecute, Func<bool> commandCanExecute)."
                                                   .FormatWith (Extensions.GetMethodName (() => myWireTarget.InstantiateFooCommand ())));
        }
    }

    public class when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute_void_return_type :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute_void_return_type.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandInstantiationMethodAttribute]
            internal void InstantiateFooCommand (Action execute, Func<bool> canExecute)
            { throw new Exception ("This code should be unreachable"); }

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
            m_ExpectedBecauseOfException =
                new InvalidOperationException ("CommandInstantiationMethodAttribute target \"{0}\" must have a return type"
                                                   .FormatWith (Extensions.GetMethodName (() => myWireTarget.InstantiateFooCommand (null, null))));
        }
    }
}