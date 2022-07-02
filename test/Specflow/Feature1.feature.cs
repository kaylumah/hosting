﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Test.Specflow
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class FeatureOneFeature : object, Xunit.IClassFixture<FeatureOneFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "Feature1.feature"
#line hidden
        
        public FeatureOneFeature(FeatureOneFeature.FixtureData fixtureData, Test_Specflow_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "", "Feature One", null, ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Empty file gets default metadata")]
        [Xunit.TraitAttribute("FeatureTitle", "Feature One")]
        [Xunit.TraitAttribute("Description", "Empty file gets default metadata")]
        public void EmptyFileGetsDefaultMetadata()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Empty file gets default metadata", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 5
  this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
      testRunner.Given("\'2022-01-01-example.md\' is an empty post:", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 7
      testRunner.Given("\'example.md\' is an empty post:", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                            "DirectoriesToSkip",
                            "FileExtensionsToTarget"});
                table2.AddRow(new string[] {
                            "",
                            ".md, .txt"});
#line 8
      testRunner.When("the files are retrieved:", ((string)(null)), table2, "When ");
#line hidden
                TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                            "Uri",
                            "Created",
                            "Modified"});
                table3.AddRow(new string[] {
                            "2022/01/01/example.md",
                            "2022-1-1",
                            "2022-1-1"});
                table3.AddRow(new string[] {
                            "example.md",
                            "<null>",
                            "<null>"});
#line 11
      testRunner.Then("the following V2:", ((string)(null)), table3, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Test")]
        [Xunit.TraitAttribute("FeatureTitle", "Feature One")]
        [Xunit.TraitAttribute("Description", "Test")]
        public void Test()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Test", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 23
  this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "scope",
                            "path",
                            "key",
                            "value"});
                table4.AddRow(new string[] {
                            "posts",
                            "",
                            "feed",
                            "true"});
#line 24
      testRunner.Given("the following defaults:", ((string)(null)), table4, "Given ");
#line hidden
#line 29
      testRunner.Given("post \'sample_001.md\' has the following contents:", "---\nauthor: max\n---", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "DirectoriesToSkip",
                            "FileExtensionsToTarget"});
                table5.AddRow(new string[] {
                            "",
                            ".md, .txt"});
#line 35
      testRunner.When("the files are retrieved:", ((string)(null)), table5, "When ");
#line hidden
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "Path",
                            "Key",
                            "Value"});
                table6.AddRow(new string[] {
                            "sample_001.md",
                            "uri",
                            "sample_001.md"});
                table6.AddRow(new string[] {
                            "sample_001.md",
                            "collection",
                            "posts"});
                table6.AddRow(new string[] {
                            "sample_001.md",
                            "author",
                            "max"});
                table6.AddRow(new string[] {
                            "sample_001.md",
                            "feed",
                            "true"});
#line 38
      testRunner.Then("the following:", ((string)(null)), table6, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Different Givens")]
        [Xunit.TraitAttribute("FeatureTitle", "Feature One")]
        [Xunit.TraitAttribute("Description", "Different Givens")]
        public void DifferentGivens()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Different Givens", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 45
  this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 46
    testRunner.Given("post \'demo.md\' has the following contents:", "Hello World", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 50
    testRunner.And("a test post named \'not-demo.md\':", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 51
    testRunner.And("a test post v2 named \'with-frontmatter.md\':", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "DirectoriesToSkip",
                            "FileExtensionsToTarget"});
                table7.AddRow(new string[] {
                            "",
                            ".md, .txt"});
#line 52
    testRunner.When("the files are retrieved:", ((string)(null)), table7, "When ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "Path",
                            "Key",
                            "Value"});
                table8.AddRow(new string[] {
                            "demo.md",
                            "uri",
                            "demo.md"});
                table8.AddRow(new string[] {
                            "demo.md",
                            "collection",
                            "posts"});
                table8.AddRow(new string[] {
                            "not-demo.md",
                            "uri",
                            "not-demo.md"});
                table8.AddRow(new string[] {
                            "not-demo.md",
                            "collection",
                            "posts"});
                table8.AddRow(new string[] {
                            "with-frontmatter.md",
                            "uri",
                            "with-frontmatter.md"});
                table8.AddRow(new string[] {
                            "with-frontmatter.md",
                            "collection",
                            "posts"});
                table8.AddRow(new string[] {
                            "with-frontmatter.md",
                            "output",
                            "true"});
#line 55
    testRunner.Then("the following:", ((string)(null)), table8, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                FeatureOneFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                FeatureOneFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
