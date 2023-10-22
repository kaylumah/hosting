// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using BoDi;
using TechTalk.SpecFlow;

namespace Test.Specflow
{
    [Binding]
    public class DiContainerHooks
    {
        readonly IObjectContainer _objectContainer;

        public DiContainerHooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void InitializeWebDriver()
        {
            MockFileSystem mockFileSystem = new MockFileSystem();
            _objectContainer.RegisterInstanceAs(mockFileSystem);
        }
    }
}
