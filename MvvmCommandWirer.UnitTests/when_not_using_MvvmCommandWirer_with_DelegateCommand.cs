using System;
using Demo.Utils;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public class when_not_using_MvvmCommandWirer_with_parameterless_DelegateCommand : when_using_ICommand<DelegateCommand>
    {
        protected Action m_Execute;
        protected Func<bool> m_CanExecute;

        protected bool m_CanExecuteCalled;
        protected bool m_ExecuteCalled;

        protected override void Establish_context ()
        {
            base.Establish_context ();

            myCanExecuteExpected = true;
            m_CanExecute = () => {
                               m_CanExecuteCalled = true;
                               return myCanExecuteExpected;
                           };
            m_Execute = () => {
                            m_ExecuteCalled = true;
                        };
        }

        protected override DelegateCommand CreateCommand ()
        {
            return new DelegateCommand (m_Execute, m_CanExecute);
        }

        protected override void AssertCanExecuteWasCalled () { Assert.IsTrue (m_CanExecuteCalled); }
        protected override object GetCanExecuteParameter () { return null; }
        protected override void AssertExecuteWasCalled () { Assert.IsTrue (m_ExecuteCalled); }
        protected override object GetExecuteParameter () { return null; }
    }

    public class when_not_using_MvvmCommandWirer_with_DelegateCommand_String : when_using_ICommand<DelegateCommand<String>>
    {
        private Action<String> m_Execute;
        private Func<String, bool> m_CanExecute;

        private bool m_CanExecuteCalled;
        private Object m_CanExecuteParameter;
        private bool m_ExecuteCalled;
        private Object m_ExecuteParameter;

        protected override void Establish_context ()
        {
            base.Establish_context ();

            myCommandParameter = "Hello world!";

            myCanExecuteExpected = true;
            m_CanExecute = s => {
                               m_CanExecuteCalled = true;
                               m_CanExecuteParameter = s;
                               return myCanExecuteExpected;
                           };
            m_Execute = s => {
                            m_ExecuteCalled = true;
                            m_ExecuteParameter = s;
                        };
        }

        protected override DelegateCommand<String> CreateCommand ()
        {
            return new DelegateCommand<String> (m_Execute, m_CanExecute);
        }

        protected override void AssertCanExecuteWasCalled ()
        {
            Assert.IsTrue (m_CanExecuteCalled);
            Assert.AreSame (myCommandParameter, m_CanExecuteParameter);
        }

        protected override object GetCanExecuteParameter () { return m_CanExecuteParameter; }

        protected override void AssertExecuteWasCalled ()
        {
            Assert.IsTrue (m_ExecuteCalled);
            Assert.AreSame (myCommandParameter, m_ExecuteParameter);
        }

        protected override object GetExecuteParameter () { return m_ExecuteParameter; }
    }
}