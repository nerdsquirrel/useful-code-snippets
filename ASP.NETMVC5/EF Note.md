## Entity state in EF
```csharp
public enum EntityState
    {
        //
        // Summary:
        //     The entity is not being tracked by the context. An entity is in this state immediately
        //     after it has been created with the new operator or with one of the System.Data.Entity.DbSet
        //     Create methods.
        Detached = 1,
        //
        // Summary:
        //     The entity is being tracked by the context and exists in the database, and its
        //     property values have not changed from the values in the database.
        Unchanged = 2,
        //
        // Summary:
        //     The entity is being tracked by the context but does not yet exist in the database.
        Added = 4,
        //
        // Summary:
        //     The entity is being tracked by the context and exists in the database, but has
        //     been marked for deletion from the database the next time SaveChanges is called.
        Deleted = 8,
        //
        // Summary:
        //     The entity is being tracked by the context and exists in the database, and some
        //     or all of its property values have been modified.
        Modified = 16
    }
```

* The Context not only holds the reference to all the objects retrieved from the database but also it holds the entity states and maintains modifications made to the properties of the entity. This feature is known as Change Tracking.

* The change in entity state from the Unchanged to the Modified state is the only state that's automatically handled by the context. All other changes must be made explicitly using proper methods of DbContext and DbSet. 

* Entity Framework 5.0 API was distributed in two places, in NuGet package and in .NET framework. The .NET framework 4.0/4.5 included EF core API, whereas EntityFramework.dll via NuGet package included EF 5.0 specific features.This has been changed with EF 6.0 which is included in EntityFramework.dll only and is not dependent on .NET framework. 

SaveChanges does different things for entities in different states:

    Unchanged entities are not touched by SaveChanges. Updates are not sent to the database for entities in the Unchanged state.
    Added entities are inserted into the database and then become Unchanged when SaveChanges returns.
    Modified entities are updated in the database and then become Unchanged when SaveChanges returns.
    Deleted entities are deleted from the database and are then detached from the context.
    
 ## Important Link:
 https://msdn.microsoft.com/en-us/data/jj592676.aspx
    
    
    
    