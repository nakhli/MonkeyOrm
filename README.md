![My image](http://www.monkeyorm.com/images/monkeyorm_monkey_orm_small.png)

MonkeyOrm is a small, powerful SQL-based ORM for .NET.

# Installation
Only MySql is supported at the moment. A [nuget](http://nuget.org/packages/MonkeyOrm.MySql) is available, just run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console):
```powershell
Install-Package MonkeyOrm.MySql
```

# Save anything
POCOs:

```csharp
connection.Save("Users", new User { Name = "Anne", Age = 31 });
```
Anonymous objects:

```csharp
connection.Save("Users", new { Name = "John", Age = 26 });
```
Hashes: [<code>IDictionary<></code>](http://msdn.microsoft.com/en-us/library/s4ys34ea), [<code>IDictionary</code>](http://msdn.microsoft.com/en-us/library/system.collections.idictionary), [<code>ExpandoObject</code>](http://msdn.microsoft.com/en-us/library/System.Dynamic.ExpandoObject.aspx) or [<code>NameValueCollection</code>](http://msdn.microsoft.com/en-us/library/System.Collections.Specialized.NameValueCollection.aspx):
```csharp
connection.Save("Users", new Dictionary<string, object>
                                {
                                    { "Name", "Fred" },
                                    { "Age", 22 }
                                });
```
Get back the auto generated serial id if any:

```csharp
int pabloId;
connection.Save("Users", new User { Name = "Pablo", Age = 49 }, out pabloId);
```

##### What the heck is `connection`?
MonkeyOrm Api consists mainly of extension methods, just like the <code>Save</code> method in the snippets above.
Several types are extended: `connection` can be an [<code>IDbConnection</code>](http://msdn.microsoft.com/en-us/library/system.data.idbconnection.aspx), an [<code>IDbTransaction</code>](http://msdn.microsoft.com/en-us/library/system.data.idbtransaction.aspx), 
any function in your code returning a new connection [`Func<IDbConnection>`](http://msdn.microsoft.com/en-us/library/bb534960.aspx), or the MonkeyOrm defined interface [<code>IConnectionFactory</code>](https://github.com/Sinbadsoft/MonkeyOrm/blob/master/MonkeyOrm/IConnectionFactory.cs).



# Read just one item

```csharp
var joe = connection.ReadOne("Select * From Users Where Name = @name", new { name = "Joe" });
```
Reads only the first element, if any, from the result set. You can also read computed data:

```csharp
var stats = connection.ReadOne("Select Max(Age) As Max, Min(Age) As Min From Users");
Console.WriteLine("Max {0} - Min {1}", stats.Max, stats.Min);
```

# Read'em All
```csharp
var users = connection.ReadAll("Select * From Users Where Age > @age", new { age = 30 });
```

Bulk fetches the whole result set in memory as a list.

# Update

```csharp
connection.Update("Users", new { CanBuyAlchohol = true }, "Age >= @age", new { age = 21 });
```

# Save or Update
```csharp
connection.SaveOrUpdate("Users", new User { Name = "Anne", Age = 32 });
```
Aka Upsert. Attempts to save first. If the insertion violates a key or unicity constraint, an update is performed instead.

# Delete
```csharp
connection.Delete("Users", "Name=@name", new { name = "Sauron" });
```

# Stream Read
Instead of bulk fetching query results in memory, they are wrapped in an enumerable for lazy evaluation. Items are loaded from the databse when the returned `IEnumerable` is actually enumerated, one at a time.

Here is an example where results are streamed from the database to a file on disk:
```csharp
var users = connection.ReadStream("Select * From Users");

using(var file = new StreamWriter("result.txt"))
foreach (var user in users)
{
    file.WriteLine("{0} - {1}", user.Name, user.Age);
}
```

Two Bonus Points: (1) the result enumerable can be enumerated multiple times if data needs to be re-streamed from the database (the query will be executed again), (2) Linq queries can be used on the result as for any enumerable, no restrictions.

`ReadStream` has also an overload that —instead of returning the result as an enumerable— takes a function that it calls for each result item until it returns `false`. The snippet above would be equivalent to:
```csharp
using(var file = new StreamWriter("result.txt"))
connection.ReadStream("Select * From Users", user => 
{
    file.WriteLine("{0} - {1}", user.Name, user.Age);
    return true;
});
```

# Transactions
```csharp
int spockId = connection.InTransaction(autocommit: true).Do(t =>
{
    int id;
    t.Save("Users", new { Name = "Spock", Age = 55 }, out id);
    t.Save("Profiles", new { UserId = id, Bio = "Federation Ambassador" });
    return id;
});
```
The transaction block can be a function or an action. The return value, if any, is returned back to client code. The transaction can be manually committed at any point by invoking `t.Commit()`. Setting the `autocommit` parameter to `true` will insert a call to `Commit()` after the transaction block.

The transaction [isolation level](http://msdn.microsoft.com/en-us/library/system.data.isolationlevel.aspx) can be specified using the `isolation` parameter:
```csharp
connection.InTransaction(true, IsolationLevel.Serializable).Do(t =>
{
    var james = t.ReadOne("Select * From Users Where Name=@name", new { name = "James" });
    t.Update("Users", new { Age = james.Age + 15 }, "Id=@Id", new { james.Id });
});
```
# Batch insertion
Batch insertion enables insertion of enumerable data sets; whether this data set is held in memory or streamed from any other source (file, database, network, computed on the fly etc.).

```csharp
connection.SaveBatch("Users", new[]
    {
        new User { Name = "Monica", Age = 34 },
        new User { Name = "Fred", Age = 58 },
        // ...
    });
```

By default, one object at a time is read from the provided enumerable and inserted in the database. In order to tune memory usage vs network round-trips more elements can be loaded and inserted at once. This is controlled by the `chunkSize` parameter.

In the following snippet, 100 objects are loaded and inserted at once —in the same query— from the provided enumerable to the database.
```csharp
connection.SaveBatch("Users", LoadDataFromRemoteSource(), 100);
```

Batch insertion can also be wrapped in a transaction
```csharp
connection.InTransaction().SaveBatch("Users", users);
```

# Object Slicing
In some contexts, the object (or hash) to be saved in the database needs to be filtered in order to exclude some of its properties. This can be for security reasons: the object has been automatically mapped from user input —by a [model binder](http://msdn.microsoft.com/en-us/library/system.web.mvc.imodelbinder.aspx) or a similar mechanism— and thus needs to be checked against the set of authorized properties. Not properly filtering user input is a security vulnerability; the github site was [hacked](http://www.theregister.co.uk/2012/03/05/github_hack/) due to a similar issue (if you want to read [more](http://www.diaryofaninja.com/blog/2012/03/11/what-aspnet-mvc-developers-can-learn-from-githubrsquos-security-woes) about this).

MonkeyOrm can slice the input object when calling `Save` or `Update` by applying either a black list or a white list filter on object properties.

```csharp
connection.Save("Users", user, blacklist: new[] { "IsAdmin" });
```
This will prevent a hacker form forging user input that would force `IsAdmin` column to `true`.

```csharp
connection.Update("Users", user, "Id=@id", new { id }, whitelist: new[] { "Name", "Age" });
```
Only allows `Name` and `Age`to be updated, nothing else.

# Interceptors and Blobbing
Interceptors are functions you can set in order to control how data is processed by MonkeyOrm. One interesting interceptor is the `UnknownValueType` interceptor. It is called when the data to be inserted in a database column does not map directly to a database native type. Consider the following example:
```csharp
connection.Save("Users", new { Name="Joe", Age=67, Profile=new ProfileData { /* ... */ });
```
The property `Profile` holds an instance of POCO type `ProfileData`. This type can't be directly inserted into the column `Profile` of the `Users` table as it is.

In this situation, MonkeyOrm calls the `UnknownValueType` callback in order the give client code a chance to "intercept" the non-trivial type and transform it to something the database can handle. A typical example would be to blob (serialize) the `ProfileData` instance. Here is an example of a xml blobber interceptor:
```csharp
MonkeyOrm.Settings.Interceptors.UnknownValueType = o =>
{
    var writer = new StringWriter();
    new XmlSerializer(o.GetType()).Serialize(writer, o);
    return writer.ToString();
};
```

# Custom non Query Commands
`Execute` runs any valid SQL code:
```csharp
connection.Execute("Truncate Table Users");
```
It can also run commands with parameters:
```csharp
connection.Execute(
    "Insert Into Users (Name, Age) Values (@Name, @Age)",
    new { Name = "Philip", Age = 55 });
```

# Related Projects
* [Massive](https://github.com/robconery/massive)
* [PetaPoco](http://www.toptensoftware.com/petapoco/)
* [Dapper](http://code.google.com/p/dapper-dot-net/)
* [Simple.Data](https://github.com/markrendle/Simple.Data)

# License
Copyright 2012-2013 [Sinbasdoft](http://www.sinbadsoft.com).

Licensed under the Apache License, [Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).
