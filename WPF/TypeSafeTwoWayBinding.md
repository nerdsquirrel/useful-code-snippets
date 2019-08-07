### why is needed?
The as usual implementation using `INotifyPropertyChanged ` has one problem, that is you have to pass the name of the property as a string when you fire the event from each property setter. 

```csharp
public class Person : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string name;
    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private int age;
    public int Age
    {
        get { return age; }
        set
        {
            age = value;
            OnPropertyChanged(nameof(Age));
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string property = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
```
### Safer way

```csharp
public class ModelBase<TModel> : INotifyPropertyChanged
{   
    public event PropertyChangedEventHandler PropertyChanged;
   
    protected virtual void NotifyPropertyChanged<TResult>(Expression<Func<TModel, TResult>> property)
    {        
        string propertyName = ((MemberExpression)property.Body).Member.Name;       
        InternalNotifyPropertyChanged(propertyName);
    }

    protected void InternalNotifyPropertyChanged([CallerMemberName] string property = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}

public class Person : ModelBase<Person>
{
    private string name;
    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            NotifyPropertyChanged(m => m.Name);
        }
    }

    private int age;
    public int Age
    {
        get { return age; }
        set
        {
            age = value;
            NotifyPropertyChanged(m => m.Age);
        }
    }
}
```

### Usage

```csharp
public class Example
{
    public Person Person{ get; set; }
    public Example()
    {
        Person = new Person();        
        Person.PropertyChanged += PersonOnPropertyChanged();
    }

    private void PersonOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Name)) 
        {
            // do something
        }

        
    }
}
```