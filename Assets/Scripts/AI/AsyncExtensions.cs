using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Provides extension methods to make Unity's AsyncOperation awaitable in async/await code.
/// </summary>
public static class AsyncExtensions
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += _ => { tcs.TrySetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }
} 