# Forge.TransactionHandler

![image](Designer.png)

## Quickstart 

Once the package is installed you can get started by first adding a transaction group to your application's service container:

```csharp
builder.Services.AddTransactionGroup(
    group => group
         .UseName("Your group name")
         .Add<YourDbContext>()
    );
```


The next step is to add the transaction handler to the middleware pipeline:

```csharp
app.UseTransactionHandler();
```

And thats add changes will now automatically be persisted to your `DbContext` if the status code of the response was <=299 and >=200


## Transaction Groups

A transaction group can contain as many `DbContexts` as you want:

```
builder.Services.AddTransactionGroup(
    group => group
         .UseName("Your group name")
         .Add<YourDbContext>()
         .Add<YourSecondDbContext>()
         .Add<YourThirdDbContext>()
    );
```
The purpose of the transaction group is to save changes on all contexts in that group in one database transaction.

As in if one fails they all fail and only if all of them succeed are changes persisted.

Transaction groups can be used seperately from the transaction handler. 

If a transaction group is required inside a service it can be resolved like so:

```

class MyService()
{
    readonly ITransactionGroup _myGroup;

    public MyService([ResolveTransactionGroup("Your group name")] ITransactionGroup myGroup)
    {
        _myGroup = myGroup
    }
}
```

## Transaction Handler

The transaction handler can be configured to persist changes based on any condition (the default being if the request was successful).

This can be done like so:

```

app.UseTransactionHandler(config =>
{
    return config.HandleTransactionGroup(
        "Your group",
        group => group.SaveIf(context => context.User.IsInRole("Admin"))
                .And(context => context.Response.Headers.ContainsKey("Persist"))
                .Or(context => context.Response.StatusCode >= 400)
        );
});
```

This is using explicit configuration of the transaction handler if this is done all transaction groups must be added in this way

The transaction handler can also be disabled for any given endpoint or controller like so:

```

[HttpGet]
[DisableTransactionHandler]
public SomeResponse Get()
{
    ...
}

```