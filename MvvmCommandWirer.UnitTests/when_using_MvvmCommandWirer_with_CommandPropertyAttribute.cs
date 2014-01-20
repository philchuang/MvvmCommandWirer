using System;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Microsoft.Practices.Prism.Commands;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public class when_using_MvvmCommandWirer_with_invalid_CommandPropertyAttribute_on_method :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_invalid_CommandPropertyAttribute_on_method.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand ()
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
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandPropertyAttribute must be applied to a property, not method \"{0}\"."
                                                                              .FormatWith (Extensions.GetMethodName (() => myWireTarget.FooCommand ())));
        }
    }
}