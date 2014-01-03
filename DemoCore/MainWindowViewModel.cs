using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Com.PhilChuang.Utils;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Demo.Utils;

namespace Demo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected event PropertyChangedEventHandler PropertyChangedInternal = delegate { };

        protected void RaisePropertyChanged<T> (Expression<Func<T>> propGet)
        {
            RaisePropertyChanged (propGet.GetPropertyName ());
        }

        protected void RaisePropertyChanged (String propertyName)
        {
            PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
            PropertyChangedInternal (this, new PropertyChangedEventArgs (propertyName));
        }

        // ---------------- OLD WAY TO DO PARAMETERLESS COMMAND --------------------------------------------------------

        public ICommand FooCommand { get; private set; }

        private bool myCanFoo;
        public bool CanFoo
        {
            get { return myCanFoo; }
            set
            {
                myCanFoo = value;
                RaisePropertyChanged (() => CanFoo);
            }
        }

        private void Foo ()
        { if (CanFoo) Output = "Foo!"; }

        // ---------------- OLD WAY TO DO PARAMETERIZED COMMAND --------------------------------------------------------

        public ICommand BarCommand { get; private set; }

        private bool CanBar (String barParameter)
        { return !barParameter.IsNullOrBlank (); }

        private void Bar (String barParameter)
        { if (CanBar (barParameter)) Output = "Bar! + " + barParameter; }

        // ---------------- WIRE UP COMMANDS IN CONSTRUCTOR ------------------------------------------------------------

        public MainWindowViewModel ()
        {
            // ------------ OLD WAY TO INITIALIZE ----------------------------------------------------------------------
            FooCommand = new DelegateCommand (Foo, () => CanFoo);
            PropertyChangedInternal += (sender, args) => {
                                           if (args.PropertyName == "CanFoo")
                                               ((DelegateCommand) FooCommand).InvalidateCanExecuteChanged ();
                                       };

            BarCommand = new DelegateCommand<String> (Bar, CanBar);
            PropertyChangedInternal += (sender, args) => {
                                           if (args.PropertyName == "BarParameter")
                                               ((DelegateCommand) BarCommand).InvalidateCanExecuteChanged ();
                                       };

            // ------------ NEW WAY TO INITIALIZE ----------------------------------------------------------------------
            CommandWirer.WireAll (this);
        }

        // TODO figure out how to tell Resharper that the methods will get called via reflection

        // ---------------- NEW WAY TO DO PARAMETERLESS COMMAND --------------------------------------------------------

        [CommandProperty (commandType: typeof (DelegateCommand))]
        public ICommand Foo2Command { get; private set; }

        [CommandInitializationMethod]
        private void InitializeFoo2Command (ICommand command) // CommandInitializationMethod can pass the instantiated Command
        {
            PropertyChangedInternal += (sender, args) => {
                                           if (args.PropertyName == "CanFoo")
                                               ((DelegateCommand) command).InvalidateCanExecuteChanged ();
                                       };
        }

        [CommandCanExecuteMethod] // CommandCanExecuteMethod can be used on Property
        public bool CanFoo2
        { get { return myCanFoo; } }

        [CommandExecuteMethod]
        private void Foo2 ()
        { if (CanFoo2) Output = "Foo2!"; }

        // ---------------- NEW WAY TO DO PARAMETERLESS COMMAND WITH DEFAULT CANEXECUTE --------------------------------

        [CommandProperty (commandType: typeof (DelegateCommand))]
        public ICommand Foo3Command { get; private set; }

        // no CommandCanExecuteMethod here, so CommandWirer only returns true

        [CommandExecuteMethod]
        private void Foo3 ()
        { Output = "Foo3!"; }

        // ---------------- NEW WAY TO DO PARAMETERIZED COMMAND --------------------------------------------------------

        [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
        public ICommand Bar2Command { get; private set; }

        [CommandInitializationMethod]
        private void InitializeBar2Command (DelegateCommand<String> command) // method parameter can be anything that implements ICommand
        {
            PropertyChangedInternal += (sender, args) => {
                                           if (args.PropertyName == "BarParameter")
                                               command.InvalidateCanExecuteChanged ();
                                       };
        }

        [CommandCanExecuteMethod]
        private bool CanBar2 (String barParameter)
        { return CanBar (barParameter); }

        [CommandExecuteMethod]
        private void Bar2 (String barParameter)
        { if (CanBar2 (barParameter)) Output = "Bar2! + " + barParameter; }

        // ---------------- NEW WAY TO DO PARAMETERIZED COMMAND WITH INSTANTIATION METHOD ------------------------------

        [CommandProperty (paramType: typeof (String))]
        public ICommand Bar3Command { get; private set; }

        [CommandInstantiationMethod] // custom instantiation method instead of relying on CommandWirer's reflection logic
        private ICommand InstantiateBar3Command (Action<String> execute, Func<String, bool> canExecute)
        { return new DelegateCommand<String> (execute, canExecute); }

        [CommandInitializationMethod]
        private void InitializeBar3Command () // method parameter is optional
        {
            PropertyChangedInternal += (sender, args) => {
                                           if (args.PropertyName == "BarParameter")
                                               ((DelegateCommand) Bar3Command).InvalidateCanExecuteChanged ();
                                       };
        }

        [CommandCanExecuteMethod]
        private bool CanBar3 (String barParameter)
        { return CanBar (barParameter); }

        [CommandExecuteMethod]
        private void Bar3 (String barParameter)
        { if (CanBar3 (barParameter)) Output = "Bar3! + " + barParameter; }

        // ---------------- DEMO PROPERTIES ----------------------------------------------------------------------------

        private String myBarParameter;
        public String BarParameter
        {
            get { return myBarParameter; }
            set
            {
                myBarParameter = value;
                RaisePropertyChanged (() => BarParameter);
            }
        }

        private String myOutput;
        public String Output
        {
            get { return myOutput; }
            set
            {
                myOutput = String.Format ("{0:o} {1}", DateTime.Now, value);
                RaisePropertyChanged (() => Output);
            }
        }
    }
}