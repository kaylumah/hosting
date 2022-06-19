---
title: Improve Code Quality with BannedSymbolAnalyzers
description: Learn how a simple Roslyn Analyzer can improve code consistency
image: /assets/images/posts/20220131/improve-code-quality-with-bannedsymbolanalyzers/cover_image.png
tags:
  - csharp
  - code-quality
publishedtime: '21:45'
commentid: '67'
---
There are many aspects of code quality; testability and consistency come to mind. I don t find it hard to imagine that the longer a project takes with multiple engineers that work on it, the more inconsistencies are in your codebase.

Thanks to the combination of .editorConfig and Roslyn Analyzers, managing this for your team is easier. Recently, however, I was required to create a custom check for my code. A quick google search pointed me to [an article from Meziantou](https://www.meziantou.net/the-roslyn-analyzers-i-use.htm) which mentioned the "Banned Symbol" Roslyn Analyzer.

## Scenario

Before diving into this Analyzer, let's take a few steps back and do a quick scenario sketch. Imagine you work for a company that allows funds transfer between two parties. Since your company needs to make money, you charge a small fee.

```cs
public class FeeCalculator : IFeeCalculator
{
    private const decimal FeePercentage = 0.12M;
    private const decimal MinimumCharge = 0.50M;
    private const decimal PriorityFeePercentage = 0.25M;
    private const decimal PriorityMinimumCharge = 7.50M;

    public decimal Calculate(decimal baseAmount, bool isPriority = false)
    {
        if (isPriority)
        {
            return InternalCalculate(baseAmount, PriorityFeePercentage, PriorityMinimumCharge);
        }
        return InternalCalculate(baseAmount, FeePercentage, MinimumCharge);
    }

    private static decimal InternalCalculate(decimal amount, decimal percentage, decimal minimumFee)
    {
        var calculatedFee = amount * (percentage / 100);
        if (calculatedFee < minimumFee)
        {
            return minimumFee;
        }
        return calculatedFee;
    }
}
```

The company decided to offer "Monday Madness" at a heavily discounted fee as a special offer.
The implementation would look similar to the snippet below.

```cs
public class DatedFeeCalculator : IFeeCalculator
{
    private const decimal DiscountedFeePercentage = 0.07M;
    private const decimal FeePercentage = 0.12M;
    private const decimal MinimumCharge = 0.50M;
    private const decimal PriorityFeePercentage = 0.25M;
    private const decimal PriorityMinimumCharge = 7.50M;

    public decimal Calculate(decimal baseAmount, bool isPriority = false)
    {
        if (isPriority)
        {
            return InternalCalculate(baseAmount, PriorityFeePercentage, PriorityMinimumCharge);
        }

        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
        {
            return InternalCalculate(baseAmount, DiscountedFeePercentage, MinimumCharge);
        }
        return InternalCalculate(baseAmount, FeePercentage, MinimumCharge);
    }

    private static decimal InternalCalculate(decimal amount, decimal percentage, decimal minimumFee)
    {
        var calculatedFee = amount * (percentage / 100);
        if (calculatedFee < minimumFee)
        {
            return minimumFee;
        }
        return calculatedFee;
    }
}
```

Take notice of line `16` which now uses `DateTime.Now`; the problem that now arises is: how do we test this code?

## Why testing DateTime is hard

The following test only results in a green build on a Monday, which is excellent if we release every week just before "Monday Madness" but not so great on every other day.

```cs
[Fact]
public void Test2_Discounted()
{
    IFeeCalculator calculator = new DatedFeeCalculator();
    var fee = calculator.Calculate(10_000M, false);
    fee.Should().Be(7.00M); // note only on Mondays it's 7.00; every other day its 12.00
}
```

We could mitigate by skipping the offending test every day that is not Monday like this; problem solved, right?

```cs
[SkippableFact]
public void Test2_Discounted_Alternative()
{
    Skip.If(DateTimeOffset.Now.DayOfWeek != DayOfWeek.Monday);
    IFeeCalculator calculator = new DatedFeeCalculator();
    var fee = calculator.Calculate(10_000M, false);
    fee.Should().Be(7.00M);
}
```

Well, no, not actually. What we want is to decouple our code from statics like DateTime.Now by putting them behind an interface. By providing an interface implementation, we can [stub a static reference](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#stub-static-references).
In an ideal world, this interface would already exist, similar to ILogger in Microsoft.Extensions. For some background reading on why it does not yet exist, see this [GitHub Issue](https://github.com/dotnet/runtime/issues/36617).

## Updated Scenario

In its most simple from the SystemClock can look like the snippet below.

```cs
public interface ISystemClock
{
    DateTimeOffset Now { get; }
}

public class SystemClock : ISystemClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
```

Our updated scenario looks like this:

```cs
public class SystemClockFeeCalculator : IFeeCalculator
{
    private const decimal DiscountedFeePercentage = 0.07M;
    private const decimal FeePercentage = 0.12M;
    private const decimal MinimumCharge = 0.50M;
    private const decimal PriorityFeePercentage = 0.25M;
    private const decimal PriorityMinimumCharge = 7.50M;

    private readonly ISystemClock systemClock;

    public SystemClockFeeCalculator(ISystemClock systemClock)
    {
        this.systemClock = systemClock;
    }


    public decimal Calculate(decimal baseAmount, bool isPriority = false)
    {
        if (isPriority)
        {
            return InternalCalculate(baseAmount, PriorityFeePercentage, PriorityMinimumCharge);
        }

        if (systemClock.Now.DayOfWeek == DayOfWeek.Monday)
        {
            return InternalCalculate(baseAmount, DiscountedFeePercentage, MinimumCharge);
        }
        return InternalCalculate(baseAmount, FeePercentage, MinimumCharge);
    }

    private static decimal InternalCalculate(decimal amount, decimal percentage, decimal minimumFee)
    {
        var calculatedFee = amount * (percentage / 100);
        if (calculatedFee < minimumFee)
        {
            return minimumFee;
        }
        return calculatedFee;
    }
}
```

With the use of a TestSystemClock or a Moq, we can test our behaviour every day of the week. See, we are improving quality already. In a previous article, ["Adventures with Mock"](https://kaylumah.nl/2021/04/11/an-approach-to-writing-mocks.html) you can read more about my preferred way of creating mocks.

```cs
public sealed class SystemClockMock : Mock<ISystemClock>
{
    public SystemClockMock SetupSystemTime(DateTimeOffset systemTime)
    {
        Setup(x => x.Now).Returns(systemTime);
        return this;
    }
}
```

Thanks to `SystemClockMock` I can now change the current date for the test.

```cs
[Fact]
public void Test3_FakeClock_Monday()
{
    var clock = new SystemClockMock()
        .SetupSystemTime(new DateTimeOffset(new DateTime(2022, 1, 31)));
    IFeeCalculator calculator = new SystemClockFeeCalculator(clock.Object);
    var fee = calculator.Calculate(10_000M, false);
    fee.Should().Be(7.00M);
}

[Fact]
public void Test3_FakeClock_Tuesday()
{
    var clock = new SystemClockMock()
        .SetupSystemTime(new DateTimeOffset(new DateTime(2022, 2, 1)));
    IFeeCalculator calculator = new SystemClockFeeCalculator(clock.Object);
    var fee = calculator.Calculate(10_000M, false);
    fee.Should().Be(12.00M);
}
```

## Force Wrapper over Static Reference

Now that we have our SystemClock, how do we make sure every dev in our team uses it over just calling `DateTimeOffset.Now`?

Finally, our Roslyn Analyzer comes into play. We can use [Microsoft.CodeAnalysis.BannedApiAnalyzers](https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md), which triggers the build warning `RS0030`. I prefer to enable these warnings on every project under src, so I use a Directory.Build.props file to install the analyzer via NuGet.

```xml
<Project>
  <Import Project="../Directory.Build.props" />
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.BannedApiAnalyzers" Version="3.3.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <AdditionalFiles Include="$(MSBuildThisFileDirectory)/BannedSymbols.txt" />
  </ItemGroup>
</Project>
```

All that remains is to create a file called `BannedSymbols.txt` with the following content

> **note**: I also blocked the use of DateTime in favour of DateTimeOffset.

```txt
T:System.DateTime;Always use System.DateTimeOffset over System.DateTime
P:System.DateTimeOffset.Now;Use ISystemClock.Now instead
```

From this point on every use of `DateTimeOffset.Now` results in the following error: `error RS0030: The symbol 'DateTimeOffset.Now' is banned in this project: Use ISystemClock.Now instead`. Which in my opinion is pretty cool :)

## Closing Thoughts
Even if the system used in today's example is fictional, I think the BannedSymbolAnalyzers is a compelling package to include in your toolbelt. At the very least, I will use it to force DateTimeOffset over DateTime. Situation allowing I will also push my wrappers over static references to improve testability.

As always, if you have any questions, feel free to reach out. Do you have suggestions or alternatives? I would love to hear about them.

The corresponding source code for this article is on [GitHub](https://github.com/kaylumah/ImproveCodeQualityWithBannedSymbolAnalyzers).

See you next time, stay healthy and happy coding to all 🧸!

## Additional Resources

- [https://github.com/dotnet/roslyn-analyzers](Roslyn Analyzers)
- [https://docs.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview](Visual Studio Code Quality)