using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Com.PhilChuang.Utils.MvvmCommandWirer;
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
        protected override object GetCanExecuteParameter () { return myWireTarget.CanExecuteParameter; }
        protected override void AssertExecuteWasCalled () { Assert.IsTrue (myWireTarget.ExecuteCalled); }
        protected override object GetExecuteParameter () { return myWireTarget.ExecuteParameter; }
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
        public virtual bool CanExecuteReturnValue { get; set; }

        public virtual bool CanExecuteCalled { get; set; }

        public virtual Object CanExecuteParameter { get; set; }

        public virtual bool ExecuteCalled { get; set; }
     
        public virtual Object ExecuteParameter { get; set; }
    }
}