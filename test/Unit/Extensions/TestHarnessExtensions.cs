﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Kaylumah.Ssg.iFX.Test;
using Test.Unit;
using Test.Unit.BDD;

// ReSharper disable once CheckNamespace
namespace Test.Utilities
{
    public static class TestHarnessExtensions
    {
        public static async Task TestService<T>(this TestHarness testHarness, Func<T, Task> scenario, ValidationContext validationContext) where T : class
        {
            try
            {
                await testHarness.TestService(scenario).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                validationContext.TestServiceException = ex;
            }
        }
    }
}
