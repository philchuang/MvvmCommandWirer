MvvmCommandWirer
================

What is it?
-----------
Utility for attribute-based wiring-up of MVVM Commands

Why would I want to use it?
---------------------------
Wiring up Commands in MVVM apps is painful, because you'll often have 4 discrete blocks of code relating to a single command:

1) The Command property on the ViewModel
2) A Func<bool> or Predicate<T> which is referenced by Command.CanExecute
3) An Action or Action<T> which is referenced by Command.Execute
4) Instantiation and Initialization code which instantiates the Command and links it to the CanExecute/Execute delegates

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

	// ... dozens of lines later ...

	public MyViewModel ()
	{
		// 4. Instantiation & Initialization for each individual ICommand
		FooCommand = new DelegateCommand (Foo, () => CanFoo);
		PropertyChanged +=
			(sender, args) =>
				if (args.PropertyName == "CanFoo")
					((DelegateCommand) Foo2Command).InvalidateCanExecuteChanged ();
	}

Could there be a cleaner way? That's the goal of this utility:

	// 1. Command Property
	[CommandProperty(commandType: typeof(DelegateCommand))]
	public ICommand FooCommand { get; private set; }

	// 4. Initialization
	[CommandOnInitializeMethod]
	private void InitializeFooCommand()
	{
		PropertyChanged +=
			(sender, args) =>
				if (args.PropertyName == "CanFoo")
					((DelegateCommand) Foo2Command).InvalidateCanExecuteChanged ();
	}

	private bool myCanFoo;
	// 2. CanExecute
	[CommandCanExecuteMethod]
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
	[CommandExecuteMethod]
	private void Foo ()
	{ if (CanFoo) Output = "Foo!"; }

	// ... dozens of lines later ...

	public MyViewModel ()
	{
		// 4. Instantiation of ALL ICommands using CommandWirer
		CommandWirer.WireAll (this);
	}

So now, all the code related to a specific Command can now be located in a single contigiuous block, instead of spread throughout the ViewModel file.

Status
------

This is very much a work-in-progress.

Implemented Features:
* Parameterless Commands
* CanExecute method is optional, will default to always return true
* CanExecute method can be a Method or a Property

TO DO:
* Parameterized Commands
* async/await handling

Shout-outs
----------

Thanks to [XAML BLOG](http://xamlblog.tumblr.com/post/46187145555/fixing-mvvm-part-1-commands) for the inspiration!
