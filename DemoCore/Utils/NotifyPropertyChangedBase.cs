using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Com.PhilChuang.Utils;

namespace Demo.Utils
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected event PropertyChangedEventHandler PropertyChangedInternal = delegate { };

        protected virtual void RaisePropertyChanged<T> (Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged (propertyExpression.GetPropertyName ());
        }

        protected virtual void RaisePropertyChanged ([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged (new PropertyChangedEventArgs (propertyName));
        }

        protected virtual void RaisePropertyChanged (PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                var handler = PropertyChanged;
                handler (this, args);
            }
            RaisePropertyChangedInternal (args);
        }

        protected virtual void RaisePropertyChangedInternal ([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChangedInternal (new PropertyChangedEventArgs (propertyName));
        }

        protected virtual void RaisePropertyChangedInternal (PropertyChangedEventArgs args)
        {
            if (PropertyChangedInternal != null)
            {
                var handler = PropertyChangedInternal;
                handler (this, args);
            }
        }
    }
}