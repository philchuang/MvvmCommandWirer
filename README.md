MvvmCommandWirer
================

Utility for attribute-based wiring-up of MVVM Commands

Wiring up Commands in MVVM apps is painful, because you'll often have 4 discrete blocks of code relating to a single command:

1) The Command property on the ViewModel
2) A Func<bool> or Predicate<T> which is referenced by Command.CanExecute
3) An Action or Action<T> which is referenced by Command.Execute
4) Initialization code which instantiates the Command and links it to the CanExecute/Execute delegates

While the first 3 blocks can be located contigiously (and thus easier to find/maintain), the initialization code is often all done in the constructor, or in a class-wide initialization method.

For instance:

// 1. Command Property
public ICommand FooCommand { get; private set; }

// 2. CanExecute
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

// 3. Execute
private void Foo ()
{ if (CanFoo) Output = "Foo!"; }

// ... dozens of lines skipped ...

public MyViewModel ()
{
  // 4. Initialization
  FooCommand = new DelegateCommand (Foo, () => CanFoo);
}
