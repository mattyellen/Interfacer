# Interfacer

Interfacer allows you to apply custom interfaces to 3rd party classes (such as the .NET Framework).  It works by automatically generating wrapper classes using Castle Project's [DynamicProxy](http://www.castleproject.org/projects/dynamicproxy/).

#### Why do I want this?

Using interfaces allows your code to be decoupled from the concrete implementation of its dependencies.  Objects and services can then be resolved through dependency injection and mocked in unit tests.

# Usage

Start with a class that doesn't currently implement an interface -- for example: [System.Threading.Semaphore](https://msdn.microsoft.com/en-us/library/system.threading.semaphore(v=vs.110).aspx). Let's create an interface for it.
```
[ApplyToInstance(typeof(Semaphore))]
public interface ISemaphore
{
    void WaitOne();
    int Release();
}
```
That's it.  Of course we could add some of the additional public methods on Semaphore, but this is all we need for now.  The `[ApplyToInstance]` attribute tells Interfacer to apply this interface to concrete instances of the Semaphore class.  But what about static methods?
```
[ApplyToStatic(typeof(Semaphore))]
public interface ISemaphoreStatic
{
    ISemaphore OpenExisting(string name);
    bool TryOpenExisting(string name, out ISemaphore result);
}
```
Couple things to note here.  The `[ApplyToStatic]` attribute tells Interfacer to apply this interface to the static methods of the Semaphore class.  Also `OpenExisting()` returns a Semaphore instance, but in our interface we defined it as returning an ISemaphore.  Interfacer will automatically wrap and unwrap objects as needed.  

So how do we actually use this now?  Easy.
```
ISemaphoreStatic semaphoreStatic = InterfacerFactory.Create<ISemaphoreStatic>();
ISemaphore semaphore = semaphoreStatic.OpenExisting("foo");
```
The `InterfacerFactory.Create<>()` method can be used to create instances of any static wrapper classes, or instance wrappers as long as the wrapped object has a parameterless constructor.  However in this case the constuctors of Semaphore all have parameters; so how do we make a new Semaphore instance?  Simple, just think of a constructor as another kind of static method.
```
[ApplyToStatic(typeof(Semaphore))]
public interface ISemaphoreStatic
{
    [Constructor]
    ISemaphore Create(int initialCount, int maximumCount);
    [Constructor]
    ISemaphore Create(int initialCount, int maximumCount, string name);
    
    ISemaphore OpenExisting(string name);
    bool TryOpenExisting(string name, out ISemaphore result);
}
```
The `[Constructor]` attribute tells Interfacer to apply the method to a constructor rather than a normal static method.  Constructor methods can be named anything, but must return an instance of the appropriate class (or an interface that wraps an instance.)  So now we can make wrapped Semaphore instances like this:
```
ISemaphoreStatic semaphoreStatic = InterfacerFactory.Create<ISemaphoreStatic>();
ISemaphore semaphore = semaphoreStatic.Create(10, 10);
semaphore.WaitOne();
```
And there you have it.  Other advanced features like generics, out parameters, and covariance / contravariance should all just work as expected.
