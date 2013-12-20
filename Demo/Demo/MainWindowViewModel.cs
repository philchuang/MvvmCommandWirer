using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

        protected void RaisePropertyChanged ([CallerMemberName] String propertyName = null)
        {
            PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
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
                RaisePropertyChanged();
            }
        }

        private void Foo ()
        { if (CanFoo) Output = "Foo!"; }

        // ---------------- OLD WAY TO DO PARAMETERIZED COMMAND --------------------------------------------------------

        public ICommand BarCommand { get; private set; }

        private bool CanBar (bool enabled)
        { return enabled; }

        private void Bar (bool enabled)
        { if (CanBar(enabled)) Output = "Bar!"; }

        // ---------------- WIRE UP COMMANDS IN CONSTRUCTOR ------------------------------------------------------------

        public MainWindowViewModel()
        {
            // ------------ OLD WAY ------------------------------------------------------------------------------------
            FooCommand = new DelegateCommand(Foo, () => CanFoo);
            BarCommand = new DelegateCommand<bool>(Bar, CanBar);
            PropertyChanged += (sender, args) =>
                               {
                                   if (args.PropertyName == "CanFoo")
                                   {
                                       ((DelegateCommand) FooCommand).InvalidateCanExecuteChanged ();
                                   }
                                   else if (args.PropertyName == "BarParameter")
                                   {
                                       ((DelegateCommand) BarCommand).InvalidateCanExecuteChanged ();
                                   }
                               };

            // ------------ NEW WAY ------------------------------------------------------------------------------------
            CommandWirer.WireAll(this);
        }

        // ---------------- NEW WAY TO DO PARAMETERLESS COMMAND --------------------------------------------------------

        [CommandProperty(commandType: typeof(DelegateCommand))]
        public ICommand Foo2Command { get; private set; }

        [CommandOnInitializeMethod]
        private void InitializeFoo2Command() // TODO figure out how to tell Resharper that this method will get called
        {
            // TODO remove this once CommandCanExecuteMethod is adapted to auto-observe
            PropertyChanged +=
                (sender, args) =>
                {
                    if (args.PropertyName == "CanFoo")
                        ((DelegateCommand) Foo2Command).InvalidateCanExecuteChanged ();
                };
        }

        [CommandCanExecuteMethod]
        public bool CanFoo2
        {
            get { return myCanFoo; }
            set
            {
                CanFoo = value;
                RaisePropertyChanged();
            }
        }

        [CommandExecuteMethod]
        private void Foo2 () // TODO figure out how to tell Resharper that this method will get called
        { if (CanFoo2) Output = "Foo2!"; }

        // ---------------- NEW WAY TO DO PARAMETERIZED COMMAND --------------------------------------------------------

        [CommandProperty(commandType: typeof(DelegateCommand<bool>), paramType: typeof(bool))]
        public ICommand Bar2Command { get; private set; }

        [CommandCanExecuteMethod]
        private bool CanBar2 (bool enabled)
        { return enabled; }

        [CommandExecuteMethod]
        private void Bar2(bool enabled) // TODO figure out how to tell Resharper that this method will get called
        { if (CanBar2(enabled)) Output = "Bar2!"; }

        // ---------------- DEMO-SPECIFIC COMMANDS ---------------------------------------------------------------------

        private bool myBarParameter;
        public bool BarParameter
        {
            get { return myBarParameter; }
            set
            {
                myBarParameter = value;
                RaisePropertyChanged();
            }
        }

        private String myOutput;
        public String Output
        {
            get { return myOutput; }
            set
            {
                myOutput = value;
                RaisePropertyChanged();
            }
        }
    }
}