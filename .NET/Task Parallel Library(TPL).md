## why TPL?
TPL not only save time for rolling and optimizing the solution, but TPL also has the advantage of being able to hook into internals of the ThreadPool that are not exposed publicly and that can boost performance in a variety of key scenarios.As mentioned above Tasks take advantage of the new work-stealing queues in the ThreadPool, which can help avoid issues of contention and cache coherency, both of which can lead to performance degradation.Using Tasks results in larger object allocations, and maintaining a Task’s lifecycle (status, cancellation requests, exception handling, etc.) requires extra work and synchronization.

```csharp
public static void BasicLoopWithCancellation()
  {
      int[] nums = Enumerable.Range(0, 100000000).ToArray();
      CancellationTokenSource cts = new CancellationTokenSource();
      ParallelOptions parallelOptions = new ParallelOptions();
      parallelOptions.CancellationToken = cts.Token;
      parallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
      Console.WriteLine("Press any key to start. Press 's' to cancel.");
      Console.ReadKey();
      Task.Factory.StartNew(() =>
       {
           if (Console.ReadKey().KeyChar == 's')
               cts.Cancel();
           Console.WriteLine("press any key to exit");
       });
      try
      {
          Parallel.ForEach(nums, parallelOptions, (num) =>
          {
              Console.WriteLine("{0} on {1}", num);
              parallelOptions.CancellationToken.ThrowIfCancellationRequested();
          });
      }
      catch (OperationCanceledException e)
      {
          Console.WriteLine(e.Message);
      }
      finally
      {
          cts.Dispose();
      }
  }
```