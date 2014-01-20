using System;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Microsoft.Practices.Prism.Commands;

// ReSharper disable InconsistentNaming
namespace MvvmCommandWirer.UnitTests
{
    public class when_using_MvvmCommandWirer_with_invalid_CommandCanExecuteMethodAttribute_property_nonbool :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_invalid_CommandCanExecuteMethodAttribute_property_nonbool.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal String CanFoo
            { get { throw new Exception ("This code should be unreachable"); } }

            [CommandExecuteMethod]
            internal void Foo ()
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" must have a bool return type."
                                                                              .FormatWith (Extensions.GetPropertyName (() => myWireTarget.CanFoo)));
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_nonbool :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_nonbool.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal String CanFoo ()
            { throw new Exception ("This code should be unreachable"); }

            [CommandExecuteMethod]
            internal void Foo ()
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" must have a bool return type."
                                                                              .FormatWith (Extensions.GetMethodName (() => myWireTarget.CanFoo ())));
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_has_param :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_has_param.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo (String parameter)
            { throw new Exception ("This code should be unreachable"); }

            [CommandExecuteMethod]
            internal void Foo ()
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" must be parameterless because CommandProperty.ParameterType was not defined."
                                                                              .FormatWith (Extensions.GetMethodName (() => myWireTarget.CanFoo (null))));
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_nonbool :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand<String>, when_using_MvvmCommandWirer_with_parameterized_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_nonbool.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal String CanFoo (String parameter)
            { throw new Exception ("This code should be unreachable"); }

            [CommandExecuteMethod]
            internal void Foo (String parameter)
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" must have a bool return type."
                                                                              .FormatWith (Extensions.GetMethodName (() => myWireTarget.CanFoo (null))));
        }
    }

    public class when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_has_wrong_param :
        when_using_MvvmCommandWirer_with_exception<DelegateCommand, when_using_MvvmCommandWirer_with_parameterless_DelegateCommand_with_invalid_CommandCanExecuteMethodAttribute_method_has_wrong_param.ViewModel>
    {
        public class ViewModel : WireTargetBase
        {
            [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
            public ICommand FooCommand { get; set; }

            [CommandCanExecuteMethod]
            internal bool CanFoo (int? parameter)
            { throw new Exception ("This code should be unreachable"); }

            [CommandExecuteMethod]
            internal void Foo ()
            { throw new Exception ("This code should be unreachable"); }
        }

        protected override void Because_of ()
        {
            base.Because_of ();

            m_IsBecauseOfExceptionExpected = true;
            m_ExpectedBecauseOfException = new InvalidOperationException ("CommandCanExecuteMethodAttribute target \"{0}\" parameter type \"{1}\" does not match CommandProperty.ParameterType \"{2}\"."
                                                                              .FormatWith (Extensions.GetMethodName (() => myWireTarget.CanFoo (null)),
                                                                                           typeof (int?).Name,
                                                                                           typeof (String).Name));
        }
    }
}