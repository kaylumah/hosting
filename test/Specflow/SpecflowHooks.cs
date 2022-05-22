// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using BoDi;
using TechTalk.SpecFlow;

namespace Test.Specflow;

[Binding]
public class SpecflowHooks
{
    private readonly IObjectContainer _objectContainer;

    public SpecflowHooks(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }


    [BeforeScenario]
    public void RegisterDependencies()
    {
        // https://docs.specflow.org/projects/specflow/en/latest/Bindings/Context-Injection.html
        //_objectContainer.register
    }
}
