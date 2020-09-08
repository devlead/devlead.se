---
title: Azure SQL transient errors
tags:
    - .NET
    - Azure
    - C#
    - Polly
    - Sql Server
author: devlead
published: 2017-03-30
canonical: https://medium.com/hackernoon/azure-sql-transient-errors-7625ad6e0a06
description: Handling Azure SQL transient errors in .NET Core using Polly
---
![Transient extension methods](https://cdn.devlead.se/clipimg-vscode/2021/01/11/e8cee4c8-d521-c835-71f6-23b18c0b210d.png?sv=2019-12-12&st=2021-01-10T14%3A02%3A39Z&se=2031-01-11T14%3A02%3A39Z&sr=b&sp=r&sig=uJ6SYGXiqheSG%2BHTludvAA71m%2F9O36lFlmSZMa6%2BN3o%3D)

One thing people will notice when migrating to using cloud services are intermittent errors occur at a higher rate than you’re used to when running on premise.

These errors are often called transient errors and can be mitigated by just retrying the operation. While some operations might be fine to retry in all cases i.e. fetching data, others like creating orders, deducting money, etc. might not or at least need a little more fine grain control. Also, some errors makes no sense to retry and as a result it’s better to fail fast.

Writing the code to handle can easy turn to spaghetti code and it isn’t code you want to copy paste as it’s easy to get wrong and hard to maintain — so you really do want a framework to assist you with these scenarios.

# Introducing Polly

This is how the authors describe Polly:

> Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner

Which really sums up what it does, it lets you create policies using a fluent API, policies which then can be reused and can range from being generic to target very specific scenarios.

Polly is available as [NuGet](https://www.nuget.org/packages/Polly) package and works with both old .NET and shiny .NET Core, which will be what I’m using in this post.

![Polly in Package manager](https://cdn.devlead.se/clipimg-vscode/2021/01/11/59b16711-011f-b854-27a1-da29029d5150.png?sv=2019-12-12&st=2021-01-10T14%3A04%3A00Z&se=2031-01-11T14%3A04%3A00Z&sr=b&sp=r&sig=g60K%2FZg3ZagWIVlNy8YsBmRC9K5Fw5MvItx1dFEidcM%3D)

Also worth noting Polly is open source available on [GitHub](https://github.com/App-vNext/Polly).
Polly’s also member of the [].NET Foundation](https://dotnetfoundation.org/).

## Creating policies

Defining a policy with Polly is really straightforward in the Polly namespace you Policy class which is what you utilize to create your policies

```csharp
var retryPolicy = Policy
    .Handle<TimeoutException>()
    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(3))
```

Above we’ve via the `Handle<TException>method` defined a policy that if a `TimeoutException` occurs it’ll wait 3 seconds and retry the operation 5 times before failing on the 6th time which is defined using the `WaitAndRetry` method.

Because the delay of each retry attempt in this case is retrieved using a `Func<int, TimeSpan>` delegate it’s really easy to implement more advanced scenarios like a sliding delay

```csharp
var retryPolicy = Policy
    .Handle<TimeoutException>()
    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt));
```

to let our policy handle more exceptions we can use one to many `Or<TException>` method after the `Handle<TException>` call

```csharp
var retryPolicy = Policy
    .Handle<TimeoutException>()
    .Or<FileNotFoundException>()
    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt));
```

you can also make your policy even more granular by inspecting the exception with an exception predicate

```csharp
var retryPolicy = Policy
    .Handle<TimeoutException>()
    .Or<System.IO.FileNotFoundException>(
      exceptionPredicate => !exceptionPredicate
                              .FileName
                              .EndsWith(".tmp", StringComparison.OrdinalIgnoreCase)
      )
    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt));
```

In this case we’ll fail fast if it’s a `.tmp` file that’s not found. You can also set policies based on result values, besides retries there’s also support for circuit-breaker, timeout, fallback, composing policies together and more, so the policy “engine” is very flexible.

## Using policies

We now have our policy in place and want to put it into good use, here the policies `Execute` method comes in play

```csharp
var intput = /* some input value */

var result = retryPolicy.Execute(() => MyRetryableMethod(input));
```

It’s almost too good to be true, there’s a lot happening under the hood — but in my opinion “hidden” under a very easy to understand and maintainable API.

## Azure SQL policies

When is comes to SQL server there’s a few errors known to be safe to retry, so we explicitly look for those, so if we begin with it could look something like this:

```csharp
private static readonly Policy SqlRetryPolicy = Policy
    .Handle<TimeoutException>()
    .Or<SqlException>(AnyRetryableError)
    .WaitAndRetry(SqlRetryCount, ExponentialBackoff);
```

where `SqlRetryCount` is just a constant for how many retries to do and `ExponetialBackoff` is a method that exponentially increases the delay between each attempt

```csharp
private const int SqlRetryCount = 4;
private static TimeSpan ExponentialBackoff(int attempt)
{
  return TimeSpan.FromSeconds(Math.Pow(2, attempt));
}
```

`TimeoutException:s` will always be retried but SqlExceptions will be passed to `AnyRetryableError` method for assessment if it’s retryable or not

```csharp
private static bool AnyRetryableError(SqlException exception)
{
    return exception.Errors.OfType<SqlError>().Any(RetryableError);
}
```

`AnyRetryableError` will iterate all errors and with the `RetryableError` method check if it’s an error known to be retriable and here is where the magic happens

```csharp
private static bool RetryableError(SqlError error)
{
  switch (error.Number)
  {
    case SqlErrorOperationInProgress:
    case SqlErrorDatabaseUnavailable:
    case SqlErrorServiceExperiencingAProblem:
    case SqlErrorServiceRequestProcessFail:
    case SqlErrorServiceBusy:
    case SqlErrorServiceErrorEncountered:
    case SqlErrorResourceLimitReached:
    case SqlErrorDatabaseLimitReached:
    case SqlErrorNetworkReleatedErrorDuringConnect:
    case SqlErrorTransportLevelErrorWhenSendingRequestToServer:
    case SqlErrorTransportLevelErrorReceivingResult:
    case SqlErrorUnableToEstablishConnection:
    case SqlErrorConnectedButLoginFailed:
    case SqlErrorInstanceDoesNotSupportEncryption:
        return true;

    default:
        return false;
  }
}
```

we will switch on SqlException error number and use a few known constants to determine if it should fail fast or retry.

## Taking the policy for a spin

In this case, I’m going to do a simple API that extends `SqlCOnnection` with `WithRetry` methods, i.e. open SQL connection

```csharp
public static SqlConnection OpenWithRetry(this SqlConnection conn)
{
  SqlRetryPolicy.Execute(conn.Open);
  return conn;
}

public static async Task<SqlConnection> OpenWithRetryAsync(this SqlConnection conn)
{
  await SqlRetryAsyncPolicy.ExecuteAsync(conn.OpenAsync);
  return conn;
}
```

giving us an open sync and async method, which in its simplest form usage could look something like this

```csharp
using (var connection = new SqlConnection(SampleDBConnectionString))
{
  connection.OpenWithRetry();
}

using (var connection = new SqlConnection(SampleDBConnectionString))
{
  await connection.OpenWithRetryAsync();
}
```

combining with a simple SQL to .NET object mapper like [Dapper])(https://github.com/StackExchange/Dapper) results in pretty clean code

```csharp
using (var connection = new SqlConnection(SampleDBConnectionString))
{
  var customer = connection
                  .OpenWithRetry()
                  .Query<Customer>(SelectCustomerStatement, new { CustomerId = 1})
                  .FirstOrDefault();
}
```

the above code will open the sql connection using the retry policy, but the query will be executed without retry, which could be what you want for some operations, but for a select it’s often safe/what you want to retry the whole operation. For that we add an overload that lets us execute code within the policy boundary

```csharp
public static TResult OpenWithRetry<TResult>(
  this SqlConnection conn,
  Func<SqlConnection, TResult> process
  )
{
  return SqlRetryPolicy.Execute(() =>
  {
      conn.Open();
      return process(conn);
  }
  );
}
```

this will open the connection and then invoke and return the result of Func you pass to it all within the scope of policy and just small refactoring of the calling code

```csharp
using (var connection = new SqlConnection(SampleDBConnectionString))
{
    var customer = connection
                    .OpenWithRetry(
                        conn=>conn.Query<Customer>(
                            SelectCustomerStatement,
                            new { CustomerId = 1 })
                    ).FirstOrDefault();
}
```

and async variant usage could look something like this

```csharp
using (var connection = new SqlConnection(SampleDBConnectionString))
{
  await connection.OpenWithRetryAsync();
  var customer = (await connection.QueryAsync<Customer>(
                                          SelectCustomerStatement,
                                          new { CustomerId = 1 }
                                      )
                  ).FirstOrDefault();
}
```

## Conclusion

Polly is really powerful, yet really easy to get started with and also fairly easy to retrofit into an existing application — so I definitely think you should take it for a spin.

I’ve created a complete sample repository on GitHub with the code from this post, thanks for reading! ❤

[![Code samples](https://cdn.devlead.se/clipimg-vscode/2021/01/11/eee8a88a-0769-f1bd-e95e-ab14a8a539e6.png?sv=2019-12-12&st=2021-01-10T14%3A31%3A58Z&se=2031-01-11T14%3A31%3A58Z&sr=b&sp=r&sig=Ci6FKYOZ%2BRhvWzVFoMC4HjZ2TItYL9XZZ3c9OkXR0pA%3D)](https://github.com/azurevoodoo/AzureSQLTransientHandling)
