MvvmCommandWirer
================

What is it?
-----------
Portable Class Library for attribute-based wiring-up of MVVM Commands. Targets .NET 4.5, SL 4+, WP7+, Windows Store.

Why would I want to use it?
---------------------------
Wiring up Commands in MVVM apps is painful, because you'll often have 4 discrete blocks of code relating to a single command:

1. The Command property on the ViewModel
2. A `Func<bool>` or `Predicate<T>` which is referenced by Command.CanExecute
3. An `Action` or `Action<T>` which is referenced by Command.Execute
4. Instantiation and Initialization code which instantiates the Command and links it to the CanExecute/Execute delegates, and sets up relations to other properties.

While the first 3 blocks can be located contiguously (and thus easier to find/maintain), the Instantiation & Initialization code is often all done in the constructor, or in a class-wide initialization method.

This may be a minor annoyance to most, but it really bugs me - because I like to have all code that's related to each other located grouped together in the source code. I don't like having to jump back and forth inside of a many-hundreds-of-lines file - it takes a lot more concentration and effort while developing.

Plus, a the instantiation code is very often simple repetitive plumbing code. Why not try to automate it?

For instance:

For example, let's say I have a ViewModel with 2 Commands, Foo and Bar:

```c#
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

// 1. Command Property
public ICommand BarCommand { get; private set; }

// 2. CanExecute
public bool CanBar (String barParam)
{ return !String.IsNullOrEmpty (barParam); }

// 3. Execute
private void Bar ()
{ if (CanBar) Output = "Bar!"; }

// ... many lines later ...

public MyViewModel ()
{
	// 4. Instantiation & Initialization for each individual ICommand
	FooCommand = new DelegateCommand (Foo, () => CanFoo);
	PropertyChanged +=
		(sender, args) =>
			if (args.PropertyName == "CanFoo")
				((DelegateCommand) FooCommand).InvalidateCanExecuteChanged ();
	BarCommand = new DelegateCommand<String> (Bar, CanBar);
}
```
It's #4 that bugs me, having the Command instantiation & initialization code located far away from the actual Command logic.

Could there be a cleaner way? That's the goal of this utility:

```c#
// 1. Command Property
[CommandProperty(commandType: typeof(DelegateCommand))]
public ICommand FooCommand { get; private set; }

// 4. Initialization
[CommandInitializationMethod]
private void InitializeFooCommand(DelegateCommand command)
{
	PropertyChanged +=
		(sender, args) =>
			if (args.PropertyName == "CanFoo")
				command.InvalidateCanExecuteChanged ();
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

// 1. Command Property
[CommandProperty(commandType: typeof(DelegateCommand<String>), parameterType: typeof(String))]
public ICommand BarCommand { get; private set; }

// 2. CanExecute
[CommandCanExecuteMethod]
public bool CanBar (String barParameter)
{ return !String.IsNullOrEmpty(barParameter); }

// 3. Execute
[CommandExecuteMethod]
private void Bar (String barParameter)
{ if (CanBar (barParameter)) Output = "Bar!"; }

// 1. Command Property
[CommandProperty]
public DelegateCommand HelloWorldCommand { get; private set; }

// 4. Initialization
[CommandInitializationMethod]
private DelegateCommand InstantiateHelloWorldCommand()
{
	return new DelegateCommand (() => Output = "Hello world!", () => true);
}

// 3. Execute
[CommandExecuteMethod]
private void HelloWorld ()
{ Output = "Hello World!"; }

// ... dozens of lines later ...

public MyViewModel ()
{
	// 4. Instantiation of ALL ICommands using CommandWirer
	CommandWirer.WireAll (this);
}
```

So now, all the code related to a specific Command can now be located in a single contigiuous block, instead of spread throughout the ViewModel file.

Status
------

This is still very much a work-in-progress, I have done very little real-world testing with it. So I would greatly appreciate any feedback on this!

Implemented Features:

* Parameterless and Parameterized Commands
* Custom instantiation and initialization methods
* CanExecute method is optional, will default to always return true
* CanExecute method can be a Method or a Property

Caveats:

* If using `[CommandProperty]` attribute to instantiate, the specified Command Type needs to have a constructor like `(Action, Func<bool>)`, or `(Action<T>, Func<T,bool>|Predicate<T>)`. Otherwise, use the `[CommandInstantiationMethod]` attribute to declare the method that will instantiate the Command.

Shout-outs
----------

Thanks to [XAML BLOG](http://xamlblog.tumblr.com/post/46187145555/fixing-mvvm-part-1-commands) for the inspiration!

License
-------

The MIT License (MIT)

Copyright (c) 2014 PhilChuang.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
