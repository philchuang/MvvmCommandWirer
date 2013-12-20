using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Demo.Utils
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = delegate { };

        protected readonly Action<Object> m_Execute;
        protected readonly Func<Object, bool> m_CanExecute;

        private bool? m_LastCanExecuteValue = null;

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            m_Execute = _ => execute();
            m_CanExecute = canExecute != null ? (Func<Object, bool>)(_ => canExecute()) : _ => true;

            RaiseCanExecuteChanged();
        }

        public DelegateCommand(Action<Object> execute, Func<Object, bool> canExecute)
        {
            m_Execute = execute;
            m_CanExecute = canExecute ?? (_ => true);

            RaiseCanExecuteChanged();
        }

        public bool CanExecute(object parameter)
        {
            var result = m_CanExecute(parameter);
            if (m_LastCanExecuteValue == null || m_LastCanExecuteValue.Value != result)
                m_LastCanExecuteValue = result;
            return result;
        }

        public void Execute(object parameter)
        {
            m_Execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public void InvalidateCanExecuteChanged()
        {
            m_LastCanExecuteValue = null;
            RaiseCanExecuteChanged();
        }
    }

    public class DelegateCommand<T> : DelegateCommand
    {
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
            : base(parameter => execute((T)(parameter ?? default(T))),
                    canExecute != null ? (Func<Object, bool>)(parameter => canExecute((T)(parameter ?? default(T)))) : _ => true)
        {
        }

        public bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        public void Execute(T parameter)
        {
            base.Execute(parameter);
        }
    }
}
