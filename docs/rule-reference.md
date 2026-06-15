# Rule Reference

Rules enforced by `Opinionated.DotNet.CodingStandards`.
Only rules with severity `warning`, `error`, or `suggestion` are listed.
Rules set to `none` or `silent` are omitted.

## Meziantou.Analyzer

| Rule ID | Description | Severity | Help |
|---------|-------------|----------|------|
| `MA0015` | Specify the parameter name in ArgumentException | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0015.md) |
| `MA0017` | Abstract types should not have public or internal constructors | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0017.md) |
| `MA0019` | Use EventArgs.Empty | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0019.md) |
| `MA0022` | Return Task.FromResult instead of returning null | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0022.md) |
| `MA0023` | Add RegexOptions.ExplicitCapture | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0023.md) |
| `MA0027` | Prefer rethrowing an exception implicitly | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0027.md) |
| `MA0029` | Combine LINQ methods | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0029.md) |
| `MA0030` | Remove useless OrderBy call | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0030.md) |
| `MA0035` | Do not use dangerous threading methods | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0035.md) |
| `MA0037` | Remove empty statement | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0037.md) |
| `MA0040` | Forward the CancellationToken parameter to methods that take one | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0040.md) |
| `MA0042` | Do not use blocking calls in an async method | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0042.md) |
| `MA0044` | Remove useless ToString call | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0044.md) |
| `MA0052` | Replace constant Enum.ToString with nameof | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0052.md) |
| `MA0054` | Embed the caught exception as innerException | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0054.md) |
| `MA0055` | Do not use finalizer | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0055.md) |
| `MA0056` | Do not call overridable members in constructor | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0056.md) |
| `MA0060` | The value returned by Stream.Read/Stream.ReadAsync is not used | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0060.md) |
| `MA0063` | Use Where before OrderBy | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0063.md) |
| `MA0067` | Use Guid.Empty | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0067.md) |
| `MA0068` | Invalid parameter name for nullable attribute | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0068.md) |
| `MA0070` | Obsolete attributes should include explanations | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0070.md) |
| `MA0072` | Do not throw from a finally block | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0072.md) |
| `MA0073` | Avoid comparison with bool constant | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0073.md) |
| `MA0079` | Forward the CancellationToken using .WithCancellation() | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0079.md) |
| `MA0082` | NaN should not be used in comparisons | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0082.md) |
| `MA0085` | Anonymous delegates should not be used to unsubscribe from Events | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0085.md) |
| `MA0086` | Do not throw from a finalizer | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0086.md) |
| `MA0087` | Parameters with [DefaultParameterValue] attributes should also be marked [Optional] | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0087.md) |
| `MA0088` | Use [DefaultParameterValue] instead of [DefaultValue] | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0088.md) |
| `MA0090` | Remove empty else/finally block | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0090.md) |
| `MA0093` | EventArgs should not be null | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0093.md) |
| `MA0099` | Use Explicit enum value instead of 0 | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0099.md) |
| `MA0100` | Await task before disposing of resources | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0100.md) |
| `MA0103` | Use SequenceEqual instead of equality operator | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0103.md) |
| `MA0108` | Remove redundant argument value | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0108.md) |
| `MA0113` | Use DateTime.UnixEpoch | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0113.md) |
| `MA0114` | Use DateTimeOffset.UnixEpoch | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0114.md) |
| `MA0128` | Use 'is' operator instead of SequenceEqual | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0128.md) |
| `MA0129` | Await task in using statement | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0129.md) |
| `MA0130` | GetType() should not be used on System.Type instances | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0130.md) |
| `MA0134` | Observe result of async calls | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0134.md) |
| `MA0140` | Both if and else branch have identical code | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0140.md) |
| `MA0143` | Primary constructor parameters should be readonly | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0143.md) |
| `MA0144` | Use System.OperatingSystem to check the current OS | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0144.md) |
| `MA0145` | Signature for [UnsafeAccessorAttribute] method is not valid | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0145.md) |
| `MA0146` | Name must be set explicitly on local functions | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0146.md) |
| `MA0151` | DebuggerDisplay must contain valid members | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0151.md) |
| `MA0152` | Use Unwrap instead of using await twice | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0152.md) |
| `MA0158` | Use System.Threading.Lock | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0158.md) |
| `MA0159` | Use 'Order' instead of 'OrderBy' | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0159.md) |
| `MA0160` | Use ContainsKey instead of TryGetValue | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0160.md) |
| `MA0166` | Forward the TimeProvider to methods that take one | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0166.md) |
| `MA0173` | Use LazyInitializer.EnsureInitialize | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0173.md) |
| `MA0176` | Optimize guid creation | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0176.md) |
| `MA0178` | Use TimeSpan.Zero instead of TimeSpan.FromXXX(0) | suggestion | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0178.md) |
| `MA0179` | Use Attribute.IsDefined instead of GetCustomAttribute(s) | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0179.md) |
| `MA0180` | ILogger type parameter should match containing type | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0180.md) |
| `MA0181` | Do not use cast | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0181.md) |
| `MA0182` | Avoid unused internal types | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0182.md) |

## Microsoft.CodeAnalysis.BannedApiAnalyzers

| Rule ID | Description | Severity | Help |
|---------|-------------|----------|------|
| `RS0030` | Do not use banned APIs | warning | [docs](https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md) |
| `RS0031` | The list of banned symbols contains a duplicate | warning | [docs](https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md) |
| `RS0035` | External access to internal symbols outside the restricted namespace(s) is prohibited | error | [docs](https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.BannedApiAnalyzers/BannedApiAnalyzers.Help.md) |

## Microsoft.CodeAnalysis.CSharp.CodeStyle

| Rule ID | Description | Severity | Help |
|---------|-------------|----------|------|
| `EnableGenerateDocumentationFile` | Set MSBuild property 'GenerateDocumentationFile' to 'true' | warning | [docs](https://github.com/dotnet/roslyn/issues/41640) |
| `IDE0004` | Remove Unnecessary Cast | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0004) |
| `IDE0005` | Using directive is unnecessary. | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0005) |
| `IDE0007` | Use implicit type | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0007) |
| `IDE0010` | Add missing cases | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0010) |
| `IDE0011` | Add braces | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0011) |
| `IDE0017` | Simplify object initialization | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0017) |
| `IDE0018` | Inline variable declaration | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0018) |
| `IDE0019` | Use pattern matching to avoid 'as' followed by a 'null' check | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0019) |
| `IDE0020` | Use pattern matching | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0020) |
| `IDE0028` | Use collection initializers or expressions | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0028) |
| `IDE0029` | Use coalesce expression | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0029) |
| `IDE0030` | Use coalesce expression | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0030) |
| `IDE0031` | Use null propagation | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0031) |
| `IDE0033` | Use explicitly provided tuple name | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0033) |
| `IDE0034` | Simplify 'default' expression | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0034) |
| `IDE0036` | Order modifiers | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0036) |
| `IDE0039` | Use local function instead of lambda | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0039) |
| `IDE0040` | Add accessibility modifiers | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0040) |
| `IDE0043` | Format string contains invalid placeholder | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0043) |
| `IDE0044` | Add readonly modifier | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0044) |
| `IDE0045` | Convert to conditional expression | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0045) |
| `IDE0046` | Convert to conditional expression | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0046) |
| `IDE0051` | Remove unused private members | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0051) |
| `IDE0052` | Remove unread private members | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0052) |
| `IDE0054` | Use compound assignment | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0054) |
| `IDE0055` | Fix formatting | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0055) |
| `IDE0056` | Use index operator | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0056) |
| `IDE0057` | Use range operator | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0057) |
| `IDE0059` | Unnecessary assignment of a value | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0059) |
| `IDE0060` | Remove unused parameter | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0060) |
| `IDE0062` | Make local function 'static' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0062) |
| `IDE0064` | Make struct fields writable | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0064) |
| `IDE0065` | Misplaced using directive | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0065) |
| `IDE0066` | Convert switch statement to expression | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0066) |
| `IDE0070` | Use 'System.HashCode' | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0070) |
| `IDE0071` | Simplify interpolation | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0071) |
| `IDE0074` | Use compound assignment | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0074) |
| `IDE0076` | Invalid global 'SuppressMessageAttribute' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0076) |
| `IDE0078` | Use pattern matching | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0078) |
| `IDE0079` | Remove unnecessary suppression | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0079) |
| `IDE0080` | Remove unnecessary suppression operator | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0080) |
| `IDE0082` | 'typeof' can be converted to 'nameof' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0082) |
| `IDE0083` | Use pattern matching | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0083) |
| `IDE0100` | Remove redundant equality | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0100) |
| `IDE0110` | Remove unnecessary discard | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0110) |
| `IDE0130` | Namespace does not match folder structure | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0130) |
| `IDE0161` | Convert to file-scoped namespace | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0161) |
| `IDE0170` | Property pattern can be simplified | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0170) |
| `IDE0180` | Use tuple to swap values | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0180) |
| `IDE0200` | Remove unnecessary lambda expression | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0200) |
| `IDE0230` | Use UTF-8 string literal | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0230) |
| `IDE0240` | Remove redundant nullable directive | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0240) |
| `IDE0241` | Remove unnecessary nullable directive | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0241) |
| `IDE0250` | Make struct 'readonly' | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0250) |
| `IDE0260` | Use pattern matching | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0260) |
| `IDE0270` | Use coalesce expression | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0270) |
| `IDE0280` | Use 'nameof' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0280) |
| `IDE0300` | Simplify collection initialization | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0300) |
| `IDE0301` | Simplify collection initialization | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0301) |
| `IDE0302` | Simplify collection initialization | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0302) |
| `IDE0303` | Simplify collection initialization | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0303) |
| `IDE0304` | Simplify collection initialization | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0304) |
| `IDE0305` | Simplify collection initialization | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0305) |
| `IDE0330` | Use 'System.Threading.Lock' | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0330) |
| `IDE1005` | Delegate invocation can be simplified. | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide1005) |
| `IDE1006` | Naming Styles | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide1006) |
| `IDE2000` | Avoid multiple blank lines | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2000) |
| `IDE2001` | Embedded statements must be on their own line | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2001) |
| `IDE2002` | Consecutive braces must not have blank line between them | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2002) |
| `IDE2003` | Blank line required between block and subsequent statement | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2003) |
| `IDE2004` | Blank line not allowed after constructor initializer colon | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2004) |
| `IDE2005` | Blank line not allowed after conditional expression token | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2005) |
| `IDE2006` | Blank line not allowed after arrow expression clause token | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide2006) |

## Microsoft.CodeAnalysis.NetAnalyzers

| Rule ID | Description | Severity | Help |
|---------|-------------|----------|------|
| `CA1000` | Do not declare static members on generic types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1000) |
| `CA1001` | Types that own disposable fields should be disposable | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1001) |
| `CA1003` | Use generic event handler instances | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1003) |
| `CA1008` | Enums should have zero value | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1008) |
| `CA1010` | Generic interface should also be implemented | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1010) |
| `CA1012` | Abstract types should not have public constructors | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1012) |
| `CA1016` | Mark assemblies with assembly version | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1016) |
| `CA1018` | Mark attributes with AttributeUsageAttribute | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1018) |
| `CA1019` | Define accessors for attribute arguments | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1019) |
| `CA1027` | Mark enums with FlagsAttribute | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1027) |
| `CA1028` | Enum Storage should be Int32 | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1028) |
| `CA1030` | Use events where appropriate | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1030) |
| `CA1033` | Interface methods should be callable by child types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1033) |
| `CA1036` | Override methods on comparable types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1036) |
| `CA1041` | Provide ObsoleteAttribute message | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1041) |
| `CA1043` | Use Integral Or String Argument For Indexers | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1043) |
| `CA1044` | Properties should not be write only | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1044) |
| `CA1046` | Do not overload equality operator on reference types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1046) |
| `CA1047` | Do not declare protected member in sealed type | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1047) |
| `CA1050` | Declare types in namespaces | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1050) |
| `CA1051` | Do not declare visible instance fields | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1051) |
| `CA1052` | Static holder types should be Static or NotInheritable | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1052) |
| `CA1058` | Types should not extend certain base types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1058) |
| `CA1061` | Do not hide base class methods | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1061) |
| `CA1063` | Implement IDisposable Correctly | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1063) |
| `CA1065` | Do not raise exceptions in unexpected locations | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1065) |
| `CA1066` | Implement IEquatable when overriding Object.Equals | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1066) |
| `CA1067` | Override Object.Equals(object) when implementing IEquatable<T> | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1067) |
| `CA1068` | CancellationToken parameters must come last | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1068) |
| `CA1069` | Enums values should not be duplicated | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1069) |
| `CA1070` | Do not declare event fields as virtual | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1070) |
| `CA1304` | Specify CultureInfo | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1304) |
| `CA1305` | Specify IFormatProvider | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1305) |
| `CA1307` | Specify StringComparison for clarity | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1307) |
| `CA1309` | Use ordinal string comparison | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1309) |
| `CA1310` | Specify StringComparison for correctness | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1310) |
| `CA1311` | Specify a culture or use an invariant version | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1311) |
| `CA1401` | P/Invokes should not be visible | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1401) |
| `CA1416` | Validate platform compatibility | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1416) |
| `CA1417` | Do not use 'OutAttribute' on string parameters for P/Invokes | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1417) |
| `CA1418` | Use valid platform string | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1418) |
| `CA1419` | Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1419) |
| `CA1420` | Property, type, or attribute requires runtime marshalling | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1420) |
| `CA1421` | This method uses runtime marshalling even when the 'DisableRuntimeMarshallingAttribute' is applied | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1421) |
| `CA1422` | Validate platform compatibility | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1422) |
| `CA1510` | Use ArgumentNullException throw helper | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1510) |
| `CA1511` | Use ArgumentException throw helper | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1511) |
| `CA1512` | Use ArgumentOutOfRangeException throw helper | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1512) |
| `CA1513` | Use ObjectDisposedException throw helper | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1513) |
| `CA1514` | Avoid redundant length argument | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1514) |
| `CA1516` | Use cross-platform intrinsics | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1516) |
| `CA1700` | Do not name enum values 'Reserved' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1700) |
| `CA1708` | Identifiers should differ by more than case | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1708) |
| `CA1712` | Do not prefix enum values with type name | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1712) |
| `CA1713` | Events should not have 'Before' or 'After' prefix | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1713) |
| `CA1715` | Identifiers should have correct prefix | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1715) |
| `CA1721` | Property names should not match get methods | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1721) |
| `CA1725` | Parameter names should match base declaration | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1725) |
| `CA1727` | Use PascalCase for named placeholders | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1727) |
| `CA1802` | Use literals where appropriate | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1802) |
| `CA1805` | Do not initialize unnecessarily | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1805) |
| `CA1806` | Do not ignore method results | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1806) |
| `CA1810` | Initialize reference type static fields inline | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1810) |
| `CA1813` | Avoid unsealed attributes | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1813) |
| `CA1814` | Prefer jagged arrays over multidimensional | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1814) |
| `CA1815` | Override equals and operator equals on value types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1815) |
| `CA1816` | Dispose methods should call SuppressFinalize | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1816) |
| `CA1820` | Test for empty strings using string length | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1820) |
| `CA1821` | Remove empty Finalizers | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1821) |
| `CA1823` | Avoid unused private fields | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1823) |
| `CA1824` | Mark assemblies with NeutralResourcesLanguageAttribute | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1824) |
| `CA1825` | Avoid zero-length array allocations | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1825) |
| `CA1826` | Do not use Enumerable methods on indexable collections | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1826) |
| `CA1827` | Do not use Count() or LongCount() when Any() can be used | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1827) |
| `CA1828` | Do not use CountAsync() or LongCountAsync() when AnyAsync() can be used | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1828) |
| `CA1829` | Use Length/Count property instead of Count() when available | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1829) |
| `CA1830` | Prefer strongly-typed Append and Insert method overloads on StringBuilder | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1830) |
| `CA1831` | Use AsSpan or AsMemory instead of Range-based indexers when appropriate | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1831) |
| `CA1832` | Use AsSpan or AsMemory instead of Range-based indexers when appropriate | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1832) |
| `CA1833` | Use AsSpan or AsMemory instead of Range-based indexers when appropriate | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1833) |
| `CA1834` | Consider using 'StringBuilder.Append(char)' when applicable | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1834) |
| `CA1835` | Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1835) |
| `CA1836` | Prefer IsEmpty over Count | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1836) |
| `CA1837` | Use 'Environment.ProcessId' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1837) |
| `CA1838` | Avoid 'StringBuilder' parameters for P/Invokes | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1838) |
| `CA1839` | Use 'Environment.ProcessPath' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1839) |
| `CA1840` | Use 'Environment.CurrentManagedThreadId' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1840) |
| `CA1841` | Prefer Dictionary.Contains methods | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1841) |
| `CA1842` | Do not use 'WhenAll' with a single task | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1842) |
| `CA1843` | Do not use 'WaitAll' with a single task | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1843) |
| `CA1844` | Provide memory-based overrides of async methods when subclassing 'Stream' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1844) |
| `CA1845` | Use span-based 'string.Concat' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1845) |
| `CA1846` | Prefer 'AsSpan' over 'Substring' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1846) |
| `CA1847` | Use char literal for a single character lookup | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1847) |
| `CA1849` | Call async methods when in an async method | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1849) |
| `CA1850` | Prefer static 'HashData' method over 'ComputeHash' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1850) |
| `CA1851` | Possible multiple enumerations of 'IEnumerable' collection | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1851) |
| `CA1852` | Seal internal types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1852) |
| `CA1853` | Unnecessary call to 'Dictionary.ContainsKey(key)' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1853) |
| `CA1854` | Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1854) |
| `CA1855` | Prefer 'Clear' over 'Fill' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1855) |
| `CA1856` | Incorrect usage of ConstantExpected attribute | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1856) |
| `CA1857` | A constant is expected for the parameter | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1857) |
| `CA1858` | Use 'StartsWith' instead of 'IndexOf' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1858) |
| `CA1861` | Avoid constant arrays as arguments | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1861) |
| `CA1862` | Use the 'StringComparison' method overloads to perform case-insensitive string comparisons | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1862) |
| `CA1864` | Prefer the 'IDictionary.TryAdd(TKey, TValue)' method | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1864) |
| `CA1865` | Use char overload | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1865) |
| `CA1866` | Use char overload | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1866) |
| `CA1867` | Use char overload | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1867) |
| `CA1868` | Unnecessary call to 'Contains(item)' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1868) |
| `CA1869` | Cache and reuse 'JsonSerializerOptions' instances | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1869) |
| `CA1870` | Use a cached 'SearchValues' instance | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1870) |
| `CA1871` | Do not pass a nullable struct to 'ArgumentNullException.ThrowIfNull' | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1871) |
| `CA1872` | Prefer 'Convert.ToHexString' and 'Convert.ToHexStringLower' over call chains based on 'BitConverter.ToString' | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1872) |
| `CA1873` | Avoid potentially expensive logging | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1873) |
| `CA1874` | Use 'Regex.IsMatch' | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1874) |
| `CA1875` | Use 'Regex.Count' | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1875) |
| `CA2002` | Do not lock on objects with weak identity | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2002) |
| `CA2009` | Do not call ToImmutableCollection on an ImmutableCollection value | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2009) |
| `CA2011` | Avoid infinite recursion | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2011) |
| `CA2012` | Use ValueTasks correctly | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2012) |
| `CA2013` | Do not use ReferenceEquals with value types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2013) |
| `CA2014` | Do not use stackalloc in loops | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2014) |
| `CA2015` | Do not define finalizers for types derived from MemoryManager<T> | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2015) |
| `CA2016` | Forward the 'CancellationToken' parameter to methods | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2016) |
| `CA2017` | Parameter count mismatch | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2017) |
| `CA2018` | 'Buffer.BlockCopy' expects the number of bytes to be copied for the 'count' argument | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2018) |
| `CA2019` | Improper 'ThreadStatic' field initialization | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2019) |
| `CA2020` | Prevent behavioral change caused by built-in operators of IntPtr and UIntPtr | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2020) |
| `CA2021` | Do not call Enumerable.Cast<T> or Enumerable.OfType<T> with incompatible types | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2021) |
| `CA2022` | Avoid inexact read with 'Stream.Read' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2022) |
| `CA2023` | Invalid braces in message template | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2023) |
| `CA2024` | Do not use 'StreamReader.EndOfStream' in async methods | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2024) |
| `CA2100` | Review SQL queries for security vulnerabilities | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2100) |
| `CA2101` | Specify marshaling for P/Invoke string arguments | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2101) |
| `CA2119` | Seal methods that satisfy private interfaces | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2119) |
| `CA2153` | Do Not Catch Corrupted State Exceptions | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2153) |
| `CA2200` | Rethrow to preserve stack details | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2200) |
| `CA2201` | Do not raise reserved exception types | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2201) |
| `CA2207` | Initialize value type static fields inline | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2207) |
| `CA2208` | Instantiate argument exceptions correctly | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2208) |
| `CA2211` | Non-constant fields should not be visible | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2211) |
| `CA2213` | Disposable fields should be disposed | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2213) |
| `CA2214` | Do not call overridable methods in constructors | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2214) |
| `CA2215` | Dispose methods should call base class dispose | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2215) |
| `CA2216` | Disposable types should declare finalizer | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2216) |
| `CA2217` | Do not mark enums with FlagsAttribute | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2217) |
| `CA2218` | Override GetHashCode on overriding Equals | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2218) |
| `CA2219` | Do not raise exceptions in finally clauses | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2219) |
| `CA2224` | Override Equals on overloading operator equals | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2224) |
| `CA2226` | Operators should have symmetrical overloads | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2226) |
| `CA2231` | Overload operator equals on overriding value type Equals | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2231) |
| `CA2235` | Mark all non-serializable fields | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2235) |
| `CA2237` | Mark ISerializable types with SerializableAttribute | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2237) |
| `CA2241` | Provide correct arguments to formatting methods | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2241) |
| `CA2242` | Test for NaN correctly | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2242) |
| `CA2243` | Attribute string literals should parse correctly | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2243) |
| `CA2244` | Do not duplicate indexed element initializations | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2244) |
| `CA2245` | Do not assign a property to itself | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2245) |
| `CA2246` | Assigning symbol and its member in the same statement | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2246) |
| `CA2247` | Argument passed to TaskCompletionSource constructor should be TaskCreationOptions enum instead of TaskContinuationOptions enum | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2247) |
| `CA2248` | Provide correct 'enum' argument to 'Enum.HasFlag' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2248) |
| `CA2249` | Consider using 'string.Contains' instead of 'string.IndexOf' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2249) |
| `CA2250` | Use 'ThrowIfCancellationRequested' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2250) |
| `CA2251` | Use 'string.Equals' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2251) |
| `CA2252` | This API requires opting into preview features | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2252) |
| `CA2253` | Named placeholders should not be numeric values | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2253) |
| `CA2254` | Template should be a static expression | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2254) |
| `CA2255` | The 'ModuleInitializer' attribute should not be used in libraries | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2255) |
| `CA2256` | All members declared in parent interfaces must have an implementation in a DynamicInterfaceCastableImplementation-attributed interface | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2256) |
| `CA2257` | Members defined on an interface with the 'DynamicInterfaceCastableImplementationAttribute' should be 'static' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2257) |
| `CA2258` | Providing a 'DynamicInterfaceCastableImplementation' interface in Visual Basic is unsupported | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2258) |
| `CA2259` | 'ThreadStatic' only affects static fields | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2259) |
| `CA2260` | Use correct type parameter | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2260) |
| `CA2261` | Do not use ConfigureAwaitOptions.SuppressThrowing with Task<TResult> | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2261) |
| `CA2262` | Set 'MaxResponseHeadersLength' properly | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2262) |
| `CA2263` | Prefer generic overload when type is known | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2263) |
| `CA2264` | Do not pass a non-nullable value to 'ArgumentNullException.ThrowIfNull' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2264) |
| `CA2265` | Do not compare Span<T> to 'null' or 'default' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2265) |
| `CA2300` | Do not use insecure deserializer BinaryFormatter | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2300) |
| `CA2301` | Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2301) |
| `CA2302` | Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2302) |
| `CA2305` | Do not use insecure deserializer LosFormatter | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2305) |
| `CA2310` | Do not use insecure deserializer NetDataContractSerializer | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2310) |
| `CA2311` | Do not deserialize without first setting NetDataContractSerializer.Binder | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2311) |
| `CA2312` | Ensure NetDataContractSerializer.Binder is set before deserializing | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2312) |
| `CA2315` | Do not use insecure deserializer ObjectStateFormatter | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2315) |
| `CA2321` | Do not deserialize with JavaScriptSerializer using a SimpleTypeResolver | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2321) |
| `CA2322` | Ensure JavaScriptSerializer is not initialized with SimpleTypeResolver before deserializing | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2322) |
| `CA2326` | Do not use TypeNameHandling values other than None | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2326) |
| `CA2327` | Do not use insecure JsonSerializerSettings | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2327) |
| `CA2328` | Ensure that JsonSerializerSettings are secure | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2328) |
| `CA2329` | Do not deserialize with JsonSerializer using an insecure configuration | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2329) |
| `CA2330` | Ensure that JsonSerializer has a secure configuration when deserializing | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2330) |
| `CA2350` | Do not use DataTable.ReadXml() with untrusted data | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2350) |
| `CA2351` | Do not use DataSet.ReadXml() with untrusted data | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2351) |
| `CA2352` | Unsafe DataSet or DataTable in serializable type can be vulnerable to remote code execution attacks | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2352) |
| `CA2353` | Unsafe DataSet or DataTable in serializable type | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2353) |
| `CA2354` | Unsafe DataSet or DataTable in deserialized object graph can be vulnerable to remote code execution attacks | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2354) |
| `CA2355` | Unsafe DataSet or DataTable type found in deserializable object graph | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2355) |
| `CA2356` | Unsafe DataSet or DataTable type in web deserializable object graph | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2356) |
| `CA2361` | Ensure auto-generated class containing DataSet.ReadXml() is not used with untrusted data | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2361) |
| `CA2362` | Unsafe DataSet or DataTable in auto-generated serializable type can be vulnerable to remote code execution attacks | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2362) |
| `CA3061` | Do Not Add Schema By URL | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3061) |
| `CA3075` | Insecure DTD processing in XML | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3075) |
| `CA3077` | Insecure Processing in API Design, XmlDocument and XmlTextReader | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3077) |
| `CA3147` | Mark Verb Handlers With Validate Antiforgery Token | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca3147) |
| `CA5350` | Do Not Use Weak Cryptographic Algorithms | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5350) |
| `CA5351` | Do Not Use Broken Cryptographic Algorithms | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5351) |
| `CA5358` | Review cipher mode usage with cryptography experts | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5358) |
| `CA5359` | Do Not Disable Certificate Validation | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5359) |
| `CA5360` | Do Not Call Dangerous Methods In Deserialization | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5360) |
| `CA5361` | Do Not Disable SChannel Use of Strong Crypto | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5361) |
| `CA5363` | Do Not Disable Request Validation | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5363) |
| `CA5364` | Do Not Use Deprecated Security Protocols | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5364) |
| `CA5365` | Do Not Disable HTTP Header Checking | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5365) |
| `CA5366` | Use XmlReader for 'DataSet.ReadXml()' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5366) |
| `CA5367` | Do Not Serialize Types With Pointer Fields | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5367) |
| `CA5368` | Set ViewStateUserKey For Classes Derived From Page | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5368) |
| `CA5369` | Use XmlReader for 'XmlSerializer.Deserialize()' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5369) |
| `CA5370` | Use XmlReader for XmlValidatingReader constructor | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5370) |
| `CA5371` | Use XmlReader for 'XmlSchema.Read()' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5371) |
| `CA5372` | Use XmlReader for XPathDocument constructor | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5372) |
| `CA5373` | Do not use obsolete key derivation function | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5373) |
| `CA5374` | Do Not Use XslTransform | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5374) |
| `CA5375` | Do Not Use Account Shared Access Signature | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5375) |
| `CA5376` | Use SharedAccessProtocol HttpsOnly | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5376) |
| `CA5377` | Use Container Level Access Policy | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5377) |
| `CA5378` | Do not disable ServicePointManagerSecurityProtocols | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5378) |
| `CA5379` | Ensure Key Derivation Function algorithm is sufficiently strong | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5379) |
| `CA5380` | Do Not Add Certificates To Root Store | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5380) |
| `CA5381` | Ensure Certificates Are Not Added To Root Certificate Store | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5381) |
| `CA5382` | Use Secure Cookies In ASP.NET Core | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5382) |
| `CA5383` | Ensure Use Secure Cookies In ASP.NET Core | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5383) |
| `CA5384` | Do Not Use Digital Signature Algorithm (DSA) | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5384) |
| `CA5385` | Use Rivest-Shamir-Adleman (RSA) Algorithm With Sufficient Key Size | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5385) |
| `CA5386` | Avoid hardcoding SecurityProtocolType value | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5386) |
| `CA5387` | Do Not Use Weak Key Derivation Function With Insufficient Iteration Count | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5387) |
| `CA5388` | Ensure Sufficient Iteration Count When Using Weak Key Derivation Function | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5388) |
| `CA5390` | Do not hard-code encryption key | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5390) |
| `CA5391` | Use antiforgery tokens in ASP.NET Core MVC controllers | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5391) |
| `CA5392` | Use DefaultDllImportSearchPaths attribute for P/Invokes | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5392) |
| `CA5393` | Do not use unsafe DllImportSearchPath value | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5393) |
| `CA5395` | Miss HttpVerb attribute for action methods | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5395) |
| `CA5396` | Set HttpOnly to true for HttpCookie | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5396) |
| `CA5397` | Do not use deprecated SslProtocols values | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5397) |
| `CA5398` | Avoid hardcoded SslProtocols values | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5398) |
| `CA5401` | Do not use CreateEncryptor with non-default IV | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5401) |
| `CA5402` | Use CreateEncryptor with the default IV | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5402) |
| `CA5403` | Do not hard-code certificate | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5403) |
| `CA5404` | Do not disable token validation checks | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5404) |
| `CA5405` | Do not always skip token validation in delegates | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca5405) |

## SonarCSharp

| Rule ID | Description | Severity | Help |
|---------|-------------|----------|------|
| `S1541` |  | warning |  |
| `S3776` |  | warning |  |

## StyleCop.Analyzers.Unstable

| Rule ID | Description | Severity | Help |
|---------|-------------|----------|------|
| `SA1649` | File name should match first type name | warning | [docs](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1649.md) |

## IDE / editor rules

These rules are configured in the IDE tier. Build enforcement varies: some fire
during `dotnet build`, others are IDE-only and not emitted by Roslyn build analyzers.

| Rule ID | Description | Severity | Help |
|---------|-------------|----------|------|
| `IDE0001` | Simplify name | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0001) |
| `IDE0002` | Simplify member access | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0002) |
| `IDE0003` | Remove this or Me qualification | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0003) |
| `IDE0038` | Use pattern matching to avoid is check followed by a cast | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0038) |
| `IDE0049` | Use language keywords instead of framework type names | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0049) |
| `IDE0084` | Use pattern matching (IsNot operator) | suggestion | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0084) |

