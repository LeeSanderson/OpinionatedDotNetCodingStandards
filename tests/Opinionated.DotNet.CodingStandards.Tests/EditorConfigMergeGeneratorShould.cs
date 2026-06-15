// Copyright (c) Codurance. All rights reserved.

using Shouldly;

namespace Opinionated.DotNet.CodingStandards.Tests;

public class EditorConfigMergeGeneratorShould
{
    private const string MinimalHeader = "is_global = true\nglobal_level = 0";

    [Fact]
    public void NewRuleAddsAtWarning()
    {
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "Do not declare static members on generic types",
                "https://example.com", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(MinimalHeader, descriptors);

        result.RewrittenText.ShouldContain("dotnet_diagnostic.CA1000.severity = warning");
    }

    [Fact]
    public void NewDisabledByDefaultRuleAlsoAddsAtWarning()
    {
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "", RuleDefaultSeverity.Warning, EnabledByDefault: false),
        };

        var result = EditorConfigMergeGenerator.Generate(MinimalHeader, descriptors);

        result.RewrittenText.ShouldContain("dotnet_diagnostic.CA1000.severity = warning");
    }

    [Fact]
    public void NewRuleAppearsInAddedIds()
    {
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(MinimalHeader, descriptors);

        result.AddedIds.ShouldContain("CA1000");
    }

    [Fact]
    public void PreserveCuratedSeverity()
    {
        var existing = MinimalHeader + "\n\n# CA1000: title\n# Help link: https://example.com\n# Enabled: True, Severity: silent\ndotnet_diagnostic.CA1000.severity = none";
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "https://example.com", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(existing, descriptors);

        result.RewrittenText.ShouldContain("dotnet_diagnostic.CA1000.severity = none");
        result.AddedIds.ShouldBeEmpty();
    }

    [Fact]
    public void RefreshTitleComment()
    {
        var existing = MinimalHeader + "\n\n# CA1000: Old title\n# Enabled: True, Severity: silent\ndotnet_diagnostic.CA1000.severity = warning";
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "New title", "", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(existing, descriptors);

        result.RewrittenText.ShouldContain("# CA1000: New title");
        result.RewrittenText.ShouldNotContain("Old title");
    }

    [Fact]
    public void RefreshHelpLinkComment()
    {
        var existing = MinimalHeader + "\n\n# CA1000: title\n# Help link: https://old.example.com\n# Enabled: True, Severity: silent\ndotnet_diagnostic.CA1000.severity = warning";
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "https://new.example.com", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(existing, descriptors);

        result.RewrittenText.ShouldContain("# Help link: https://new.example.com");
        result.RewrittenText.ShouldNotContain("old.example.com");
    }

    [Fact]
    public void RefreshDefaultSeverityComment()
    {
        var existing = MinimalHeader + "\n\n# CA1000: title\n# Enabled: True, Severity: silent\ndotnet_diagnostic.CA1000.severity = warning";
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "", RuleDefaultSeverity.Warning, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(existing, descriptors);

        result.RewrittenText.ShouldContain("Severity: warning");
    }

    [Fact]
    public void HelpLinkLineOmittedWhenEmpty()
    {
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(MinimalHeader, descriptors);

        result.RewrittenText.ShouldNotContain("# Help link:");
    }

    [Theory]
    [InlineData(RuleDefaultSeverity.Hidden, "silent")]
    [InlineData(RuleDefaultSeverity.Info, "suggestion")]
    [InlineData(RuleDefaultSeverity.Warning, "warning")]
    [InlineData(RuleDefaultSeverity.Error, "error")]
    public void SeverityWordMapping(RuleDefaultSeverity defaultSeverity, string expectedWord)
    {
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "", defaultSeverity, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(MinimalHeader, descriptors);

        result.RewrittenText.ShouldContain($"Severity: {expectedWord}");
    }

    [Fact]
    public void StaleRuleCarriedThroughUnchanged()
    {
        var staleBlock = "# CA1000: Old title\n# Help link: https://example.com\n# Enabled: True, Severity: silent\ndotnet_diagnostic.CA1000.severity = none";
        var existing = MinimalHeader + "\n\n" + staleBlock;
        var descriptors = Array.Empty<RuleDescriptor>();

        var result = EditorConfigMergeGenerator.Generate(existing, descriptors);

        result.RewrittenText.ShouldContain(staleBlock);
        result.StaleIds.ShouldContain("CA1000");
    }

    [Fact]
    public void HeaderPreserved()
    {
        var customHeader = "# Custom header comment\nis_global = true\nglobal_level = 5";
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "title", "", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(customHeader, descriptors);

        result.RewrittenText.ShouldStartWith(customHeader);
    }

    [Fact]
    public void RulesSortedById()
    {
        var existing = MinimalHeader
            + "\n\n# CA1002: title\n# Enabled: True, Severity: silent\ndotnet_diagnostic.CA1002.severity = warning"
            + "\n\n# CA1000: title\n# Enabled: True, Severity: silent\ndotnet_diagnostic.CA1000.severity = warning";
        var descriptors = new[]
        {
            new RuleDescriptor("CA1002", "title", "", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
            new RuleDescriptor("CA1000", "title", "", RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };

        var result = EditorConfigMergeGenerator.Generate(existing, descriptors);

        var ca1000Pos = result.RewrittenText.IndexOf("CA1000", StringComparison.Ordinal);
        var ca1002Pos = result.RewrittenText.IndexOf("CA1002", StringComparison.Ordinal);
        ca1000Pos.ShouldBeLessThan(ca1002Pos);
    }

    [Fact]
    public void Idempotent()
    {
        var descriptors = new[]
        {
            new RuleDescriptor("CA1000", "Do not declare static members on generic types",
                "https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1000",
                RuleDefaultSeverity.Hidden, EnabledByDefault: true),
        };
        var firstPass = EditorConfigMergeGenerator.Generate(MinimalHeader, descriptors);

        var secondPass = EditorConfigMergeGenerator.Generate(firstPass.RewrittenText, descriptors);

        secondPass.RewrittenText.ShouldBe(firstPass.RewrittenText);
        secondPass.AddedIds.ShouldBeEmpty();
        secondPass.StaleIds.ShouldBeEmpty();
    }
}
