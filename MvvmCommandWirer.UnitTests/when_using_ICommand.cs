using System;
using System.Windows.Input;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public abstract class when_using_ICommand<TCommand> : MvvmCommandWirer_UnitTests_Base
        where TCommand : ICommand
    {
        protected TCommand myCommand;
        protected Object myCommandParameter;

        protected bool myCanExecuteExpected;
        protected bool? myCanExecuteResult;

        protected abstract void InitializeCommand ();

        protected override void Because_of ()
        {
            try
            {
                InitializeCommand ();

                myCanExecuteResult = myCommand.CanExecute (myCommandParameter);
                myCommand.Execute (myCommandParameter);
            }
            catch (Exception ex)
            {
                m_BecauseOfException = ex;
            }
        }

        protected abstract void AssertCanExecuteWasCalled ();
        protected abstract void AssertExecuteWasCalled ();

        [Test]
        public void then_CanExecute_should_be_called ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            AssertCanExecuteWasCalled ();
        }

        [Test]
        public void then_CanExecute_result_should_match ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            Assert.IsNotNull (myCanExecuteResult);
            Assert.AreEqual (myCanExecuteExpected, myCanExecuteResult.Value);
        }

        [Test]
        public void then_Execute_should_be_called ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            AssertExecuteWasCalled ();
        }
    }
}