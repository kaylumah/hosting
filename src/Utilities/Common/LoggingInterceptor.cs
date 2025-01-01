// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Kaylumah.Ssg.Utilities.Common
{
#pragma warning disable CA1848, CA2254
    public class LoggingInterceptor : IAsyncInterceptor
    {
        readonly ILogger<LoggingInterceptor> _Logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _Logger = logger;
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            invocation.ReturnValue = InterceptAsynchronousInternal(invocation, stopwatch);
        }

        async Task InterceptAsynchronousInternal(IInvocation invocation, Stopwatch stopwatch)
        {
            try
            {
                invocation.Proceed(); // Execute the original method
                Task? task = (Task)invocation.ReturnValue;
                await task.ConfigureAwait(false); // Await the task's completion
            }
            finally
            {
                stopwatch.Stop();
                _Logger.LogInformation($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            invocation.ReturnValue = InterceptAsynchronousInternal<TResult>(invocation, stopwatch);
        }

        async Task<TResult> InterceptAsynchronousInternal<TResult>(IInvocation invocation, Stopwatch stopwatch)
        {
            try
            {
                invocation.Proceed(); // Execute the original method
                Task<TResult>? task = (Task<TResult>)invocation.ReturnValue;
                return await task.ConfigureAwait(false); // Await the task's completion and return its result
            }
            finally
            {
                stopwatch.Stop();
                _Logger.LogInformation($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                invocation.Proceed(); // Ensure the original method is called
            }
            finally
            {
                stopwatch.Stop();
                _Logger.LogInformation($"Method {invocation.Method.Name} executed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }
    }
}