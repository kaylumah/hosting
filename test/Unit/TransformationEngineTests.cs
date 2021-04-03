// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.

using Xunit;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Test.Unit.Mocks;
using Kaylumah.Ssg.Utilities;
using Kaylumah.Ssg.Engine.Transformation.Service.Plugins;

namespace Test.Unit
{
    public class TransformationEngineTests
    {
        [Fact]
        public void Test1()
        {
            var fileSystemMock = new FileSystemMock();
            ITransformationEngine transformEngine = new TransformationEngine(fileSystemMock.Object, new IPlugin[] {});
        }
    }
}