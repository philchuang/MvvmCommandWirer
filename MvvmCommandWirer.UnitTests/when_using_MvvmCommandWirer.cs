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
    public abstract class when_using_MvvmCommandWirer<TCommand, TWireTarget> : when_using_ICommand<TCommand>
        where TCommand : ICommand
        where TWireTarget : WireTargetBase
    {
        protected TWireTarget myWireTarget;
        protected IList<CommandWirer> myWireAllResults;

        protected virtual TWireTarget CreateWireTarget ()
        { return Activator.CreateInstance<TWireTarget> (); }

        protected override void Establish_context ()
        {
            base.Establish_context ();

            myWireTarget = CreateWireTarget ();
        }

        protected override void AssertCanExecuteWasCalled () { Assert.IsTrue (myWireTarget.CanExecuteCalled); }
        protected override void AssertExecuteWasCalled () { Assert.IsTrue (myWireTarget.ExecuteCalled); }
    }

    public abstract class when_using_MvvmCommandWirer_with_exception<TCommand, TWireTarget> : when_using_MvvmCommandWirer<TCommand, TWireTarget>
        where TCommand : ICommand
        where TWireTarget : WireTargetBase
    {
        protected override TCommand CreateCommand ()
        {
            myWireAllResults = CommandWirer.WireAll (myWireTarget);
            Assert.Fail ("This line should be unreachable");
            return default(TCommand);
        }

        protected override void AssertCanExecuteWasCalled () { Assert.Fail ("This line should be unreachable"); }
        protected override void AssertExecuteWasCalled () { Assert.Fail ("This line should be unreachable"); }
    }

    public abstract class when_using_MvvmCommandWirer_successfully<TCommand, TWireTarget> : when_using_MvvmCommandWirer<TCommand, TWireTarget>
        where TCommand : ICommand
        where TWireTarget : WireTargetBase
    {
        protected abstract TCommand GetCommandFromWireTarget ();

        protected override TCommand CreateCommand ()
        {
            myWireAllResults = CommandWirer.WireAll (myWireTarget);
            return GetCommandFromWireTarget ();
        }

        protected override void AssertCanExecuteWasCalled () { Assert.IsTrue (myWireTarget.CanExecuteCalled); }
        protected override void AssertExecuteWasCalled () { Assert.IsTrue (myWireTarget.ExecuteCalled); }

        protected abstract void AssertWireAllResultsMatch ();

        [Test]
        public void then_WireAll_results_should_match ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            AssertWireAllResultsMatch ();
        }
    }

    public abstract class WireTargetBase
    {
        public bool CanExecuteReturnValue { get; set; }

        public bool CanExecuteCalled { get; set; }

        public Object CanExecuteParameter { get; set; }

        public bool ExecuteCalled { get; set; }
     
        public Object ExecuteParameter { get; set; }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple : when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple.ViewModel>
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

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_nonpublic : when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_nonpublic.ViewModel>
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

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_public_CanExecute_property : when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_public_CanExecute_property.ViewModel>
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

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_nonpublic_CanExecute_property : when_using_MvvmCommandWirer_successfully<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_simple_with_nonpublic_CanExecute_property.ViewModel>
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

    public class when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute : when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_invalid_CommandInstantiationMethodAttribute.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandInstantiationMethodAttribute]
            internal DelegateCommand InstantiateFooCommand { get { return null; } }

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

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandInstantiationMethodAttribute must be applied to a method, not property \"{0}\"."
                                                                              .FormatWith (Extensions.GetPropertyName (() => myWireTarget.InstantiateFooCommand)));
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
            {
                get { return null; }
            }

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

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandInitializationMethodAttribute must be applied to a method, not property \"{0}\"."
                                                                              .FormatWith (Extensions.GetPropertyName (() => myWireTarget.InitializeFooCommand)));
        }
    }
}