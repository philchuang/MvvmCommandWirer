using System;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Microsoft.Practices.Prism.Commands;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    // the successful cases for CommandExecuteMethodAttribute are in when_using_MvvmCommandWirer_simple.cs

    public class when_using_MvvmCommandWirer_invalid_with_CommandExecuteMethodAttribute_on_property :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_invalid_with_CommandExecuteMethodAttribute_on_property.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo
            { get { throw new Exception ("This code should be unreachable"); } }

            [CommandExecuteMethod]
            internal String Foo
            { get { throw new Exception ("This code should be unreachable"); } }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandExecuteMethodAttribute target \"{0}\" must be a method."
                                                                              .FormatWith (Extensions.GetPropertyName (() => myWireTarget.Foo)));
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandExecuteMethodAttribute_method_has_param :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandExecuteMethodAttribute_method_has_param.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo ()
            { throw new Exception ("This code should be unreachable"); }

            [CommandExecuteMethod]
            internal void Foo (String parameter)
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandExecuteMethodAttribute target \"{0}\" must be parameterless because CommandProperty.ParameterType was not defined."
                                                                              .FormatWith (Extensions.GetMethodName (() => myWireTarget.Foo (null))));
        }
    }


    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandExecuteMethodAttribute_method_has_wrong_param :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandExecuteMethodAttribute_method_has_wrong_param.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo (String parameter)
            { throw new Exception ("This code should be unreachable"); }

            [CommandExecuteMethod]
            internal void Foo (int? parameter)
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandExecuteMethodAttribute target \"{0}\" parameter type \"{1}\" does not match CommandProperty.ParameterType \"{2}\"."
                                                                              .FormatWith (Extensions.GetMethodName (() => myWireTarget.Foo (null)),
                                                                                           typeof (int?).Name,
                                                                                           typeof (String).Name));
        }
    }
}