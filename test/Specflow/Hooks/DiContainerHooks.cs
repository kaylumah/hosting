// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using BoDi;

namespace Test.Specflow;

[Binding]
public class DiContainerHooks
{
    private readonly IObjectContainer _objectContainer;

    public DiContainerHooks(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeScenario]
    public void InitializeWebDriver()
    {
        var mockFileSystem = new MockFileSystem();
        _objectContainer.RegisterInstanceAs(mockFileSystem);
    }
}
