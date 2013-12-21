using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Com.PhilChuang.Utils.MvvmCommandWirer;
using Demo.Utils;

namespace Demo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected event PropertyChangedEventHandler PropertyChangedInternal = delegate { };

        protected void RaisePropertyChanged ([CallerMemberName] String propertyName = null)
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
                RaisePropertyChanged ();
            }
        }

        private void Foo ()
        { if (CanFoo) Output = "Foo!"; }

        // ---------------- OLD WAY TO DO PARAMETERIZED COMMAND --------------------------------------------------------

        public ICommand BarCommand { get; private set; }

        private bool CanBar (String barParameter)
        { return !String.IsNullOrWhiteSpace (barParameter); }

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

        // ---------------- NEW WAY TO DO PARAMETERLESS COMMAND --------------------------------------------------------

        [CommandProperty (commandType: typeof (DelegateCommand))]
        public ICommand Foo2Command { get; private set; } // TODO figure out how to tell Resharper that the set method will get called

        [CommandOnInitializeMethod]
        private void InitializeFoo2Command () // TODO figure out how to tell Resharper that this method will get called
        {
            PropertyChangedInternal += (sender, args) => {
                                           if (args.PropertyName == "CanFoo")
                                               ((DelegateCommand) Foo2Command).InvalidateCanExecuteChanged ();
                                       };
        }

        [CommandCanExecuteMethod]
        public bool CanFoo2
        { get { return myCanFoo; } }

        [CommandExecuteMethod]
        private void Foo2 () // TODO figure out how to tell Resharper that this method will get called
        { if (CanFoo2) Output = "Foo2!"; }

        // ---------------- NEW WAY TO DO PARAMETERIZED COMMAND --------------------------------------------------------

        [CommandProperty (commandType: typeof (DelegateCommand<String>), paramType: typeof (String))]
        public ICommand Bar2Command { get; private set; } // TODO figure out how to tell Resharper that the set method will get called

        [CommandCanExecuteMethod]
        private bool CanBar2 (String barParameter)
        { return CanBar (barParameter); }

        [CommandExecuteMethod]
        private void Bar2 (String barParameter) // TODO figure out how to tell Resharper that this method will get called
        { if (CanBar2 (barParameter)) Output = "Bar2! + " + barParameter; }

        // ---------------- DEMO PROPERTIES ----------------------------------------------------------------------------

        private String myBarParameter;
        public String BarParameter
        {
            get { return myBarParameter; }
            set
            {
                myBarParameter = value;
                RaisePropertyChanged ();
            }
        }

        private String myOutput;
        public String Output
        {
            get { return myOutput; }
            set
            {
                myOutput = String.Format ("{0:o} {1}", DateTime.Now, value);
                RaisePropertyChanged ();
            }
        }
    }
}