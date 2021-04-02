// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

using Xunit;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Test.Unit
{
    public class TransformationEngineTests
    {
        [Fact]
        public void Test1()
        {
            ITransformationEngine transformEngine = new TransformationEngine();
        }
    }
}