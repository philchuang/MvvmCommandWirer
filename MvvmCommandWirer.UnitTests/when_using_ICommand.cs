﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Com.PhilChuang.Utils;
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

        protected abstract TCommand CreateCommand ();

        protected override void Because_of ()
        {
            try
            {
                myCommand = CreateCommand ();

                myCanExecuteResult = myCommand.CanExecute (myCommandParameter);
                // invoke off the main thread just in case it's async
                Task.Run (() => myCommand.Execute (myCommandParameter)).Wait ();
                WaitUntilExecuteIsFinished ();
            }
            catch (Exception ex)
            {
                m_BecauseOfException = ex;
            }
        }

        protected virtual void WaitUntilExecuteIsFinished ()
        {
            // base impl does nothing
        }

        protected abstract void AssertCanExecuteWasCalled ();
        protected abstract Object GetCanExecuteParameter ();
        protected abstract void AssertExecuteWasCalled ();
        protected abstract Object GetExecuteParameter ();

        [Test]
        public void then_Command_should_be_created ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            Assert.IsNotNull (myCommand);
        }

        [Test]
        public virtual void then_CanExecute_should_be_called ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            AssertCanExecuteWasCalled ();
        }

        [Test]
        public virtual void then_CanExecute_parameter_should_match ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            Assert.AreSame (myCommandParameter, GetCanExecuteParameter ());
        }

        [Test]
        public virtual void then_CanExecute_result_should_match ()
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

        [Test]
        public void then_Execute_parameter_should_match ()
        {
            if (m_IsBecauseOfExceptionExpected) return;

            Assert.AreSame (myCommandParameter, GetExecuteParameter ());
        }
    }
}