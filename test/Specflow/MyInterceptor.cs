// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Infrastructure;

namespace Test.Specflow
{
    public sealed class MyInterceptor : IAsyncInterceptor
    {
        readonly ISpecFlowOutputHelper _specFlowOutputHelper;

        public MyInterceptor(ISpecFlowOutputHelper specFlowOutputHelper)
        {
            _specFlowOutputHelper = specFlowOutputHelper;
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            _ = invocation ?? throw new ArgumentNullException(nameof(invocation));
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            _ = invocation ?? throw new ArgumentNullException(nameof(invocation));
            invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation);
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            _ = invocation ?? throw new ArgumentNullException(nameof(invocation));
            InternalIntercept(invocation);
            ValidateReturnValue(invocation, invocation.ReturnValue);
        }

        async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            _ = invocation ?? throw new ArgumentNullException(nameof(invocation));
            InternalIntercept(invocation);
            Task task = (Task)invocation.ReturnValue;
            await task.ConfigureAwait(false);
        }

        async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            _ = invocation ?? throw new ArgumentNullException(nameof(invocation));
            InternalIntercept(invocation);
            Task<TResult> task = (Task<TResult>)invocation.ReturnValue;
            TResult returnValue = await task.ConfigureAwait(false);
            ValidateReturnValue(invocation, returnValue);
            return returnValue;
        }

        void ValidateReturnValue(IInvocation invocation, object returnValue)
        {
            // var validationResults = _validator.ValidateObject(returnValue);
            // if (validationResults.Length > 0)
            // {
            //     var faultDetail = new ResponseValidationFault
            //     {
            //         ValidationResults = validationResults.ToDataContractValidationResults().ToArray()
            //     };
            //     var operationName = $"{invocation.Method?.DeclaringType?.FullName}.{invocation.Method?.Name}";
            //     var error = $"Response validation failed for '{operationName}':{Environment.NewLine}{validationResults.PrettyPrint()}";
            //     throw new FaultException<ResponseValidationFault>(faultDetail, new FaultReason(error));
            // }
        }

        void InternalIntercept(IInvocation invocation)
        {
            _ = invocation ?? throw new ArgumentNullException(nameof(invocation));
            string test = invocation.Method.DeclaringType?.FullName + "." + invocation.Method.Name;
            _specFlowOutputHelper.WriteLine(test);
            if (invocation.Arguments is { Length: > 0 })
            {
                object first = invocation.Arguments[0];
                _specFlowOutputHelper.WriteLine(JsonConvert.SerializeObject(first));
            }
            // var validationResults = _validator.ValidateOperation(invocation.Method, invocation.Arguments);
            // if (validationResults.Length > 0)
            // {
            //     var faultDetail = new RequestValidationFault
            //     {
            //         ValidationResults = validationResults.ToDataContractValidationResults().ToArray()
            //     };
            //
            //     var operationName = $"{invocation.Method?.DeclaringType?.FullName}.{invocation.Method?.Name}";
            //     var error = $"Request validation failed for '{operationName}':{Environment.NewLine}{validationResults.PrettyPrint()}";
            //     throw new FaultException<RequestValidationFault>(faultDetail, new FaultReason(error));
            // }
            invocation.Proceed();
        }
    }
}
