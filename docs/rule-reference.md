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
| `MA0183` | The format string should use placeholders | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0183.md) |
| `MA0184` | Do not use interpolated string without parameters | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0184.md) |
| `MA0185` | Simplify string.Create when all parameters are culture invariant | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0185.md) |
| `MA0186` | Equals method should use [NotNullWhen(true)] on the parameter | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0186.md) |
| `MA0187` | Use constructor injection instead of [Inject] attribute | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0187.md) |
| `MA0188` | Use System.TimeProvider instead of a custom time abstraction | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0188.md) |
| `MA0189` | Use InlineArray instead of fixed-size buffers | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0189.md) |
| `MA0190` | Use partial property instead of partial method for GeneratedRegex | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0190.md) |
| `MA0191` | Do not use the null-forgiving operator | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0191.md) |
| `MA0192` | Use HasFlag instead of bitwise checks | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0192.md) |
| `MA0193` | Use an overload with a MidpointRounding argument | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0193.md) |
| `MA0194` | Merge is expressions on the same value | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0194.md) |
| `MA0195` | Do not use static fields before they are initialized | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0195.md) |
| `MA0196` | Do not use inheritdoc on non-inheriting members | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0196.md) |
| `MA0197` | Add dedicated documentation on types | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0197.md) |
| `MA0198` | Specify cref for ambiguous inheritdoc on types | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0198.md) |
| `MA0199` | Do not use inheritdoc on types without inheritance source | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0199.md) |
| `MA0200` | Do not use empty property patterns with non-nullable value types | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0200.md) |
| `MA0201` | Do not use zero-valued enum flags in flag checks | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0201.md) |
| `MA0202` | Conditional compilation branches have identical code | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0202.md) |
| `MA0203` | Do not use return tag for void method | warning | [docs](https://www.meziantou.net/analyzer/rules/203) |
| `MA0204` | Remove unnecessary partial modifier | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0204.md) |
| `MA0205` | Use exclusive or operator | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0205.md) |
| `MA0206` | Remove unnecessary braces in type declaration | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0206.md) |
| `MA0207` | [FixedAddressValueType] fields must be static | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0207.md) |
| `MA0208` | [FixedAddressValueType] fields must be value types | warning | [docs](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0208.md) |

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
| `CA2266` | File-based program entry point should start with '#!' | warning | [docs](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2266) |
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
| `S100` | Methods and properties should be named in PascalCase | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-100/) |
| `S1006` | Method overrides should not change parameter defaults | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1006/) |
| `S101` | Types should be named in PascalCase | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-101/) |
| `S106` | Standard outputs should not be used directly to log anything | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-106/) |
| `S1066` | Mergeable "if" statements should be combined | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1066/) |
| `S1067` | Expressions should not be too complex | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1067/) |
| `S107` | Methods should not have too many parameters | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-107/) |
| `S1075` | URIs should not be hardcoded | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1075/) |
| `S108` | Nested blocks of code should not be left empty | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-108/) |
| `S109` | Magic numbers should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-109/) |
| `S110` | Inheritance tree of classes should not be too deep | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-110/) |
| `S1110` | Redundant pairs of parentheses should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1110/) |
| `S1116` | Empty statements should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1116/) |
| `S1117` | Local variables should not shadow class fields or properties | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1117/) |
| `S1121` | Assignments should not be made from within sub-expressions | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1121/) |
| `S1125` | Boolean literals should not be redundant | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1125/) |
| `S1128` | Unnecessary "using" should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1128/) |
| `S1133` | Deprecated code should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1133/) |
| `S1134` | Track uses of "FIXME" tags | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1134/) |
| `S1135` | Track uses of "TODO" tags | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1135/) |
| `S1144` | Unused private types or members should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1144/) |
| `S1147` | Exit methods should not be called | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1147/) |
| `S1151` | “switch case” clauses should not have too many lines of code | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1151/) |
| `S1168` | Empty arrays and collections should be returned instead of null | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1168/) |
| `S1172` | Unused method parameters should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1172/) |
| `S1185` | Overriding members should do more than simply call the same member in the base class | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1185/) |
| `S1186` | Methods should not be empty | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1186/) |
| `S1192` | String literals should not be duplicated | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1192/) |
| `S1199` | Nested code blocks should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1199/) |
| `S1200` | Classes should not be coupled to too many other classes | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1200/) |
| `S1206` | 'Equals(Object)' and 'GetHashCode()' should be overridden in pairs | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1206/) |
| `S121` | Control structures should use curly braces | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-121/) |
| `S1215` | “GC.Collect” should not be called | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1215/) |
| `S1226` | Method parameters, caught exceptions and foreach variables' initial values should not be ignored | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1226/) |
| `S1227` | break statements should not be used except for switch cases | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1227/) |
| `S1244` | Floating point numbers should not be tested for equality | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1244/) |
| `S125` | Sections of code should not be commented out | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-125/) |
| `S126` | “if ... else if” constructs should end with “else” clauses | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-126/) |
| `S1264` | A "while" loop should be used instead of a "for" loop | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1264/) |
| `S127` | “for” loop stop conditions should be invariant | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-127/) |
| `S1301` | "switch" statements should have at least 3 "case" clauses | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1301/) |
| `S1309` | Track uses of in-source issue suppressions | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1309/) |
| `S131` | “switch/Select” statements should contain a “default/Case Else” clause | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-131/) |
| `S1312` | Logger fields should be 'private static readonly' | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1312/) |
| `S1313` | Using hardcoded IP addresses is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1313/) |
| `S134` | Control flow statements "if", "switch", "for", "foreach", "while", "do" and "try" should not be nested too deeply | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-134/) |
| `S138` | Functions should not have too many lines of code | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-138/) |
| `S1450` | Private fields only used as local variables in methods should become local variables | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1450/) |
| `S1479` | “switch” statements with many “case” clauses should have only one statement | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1479/) |
| `S1481` | Unused local variables should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1481/) |
| `S1541` | Methods and properties should not be too complex | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1541/) |
| `S1607` | Tests should not be ignored | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1607/) |
| `S1643` | Strings should not be concatenated using '+' in a loop | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1643/) |
| `S1656` | Variables should not be self-assigned | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1656/) |
| `S1659` | Multiple variables should not be declared on the same line | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1659/) |
| `S1694` | An abstract class should have both abstract and concrete methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1694/) |
| `S1696` | NullReferenceException should not be caught | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1696/) |
| `S1698` | "==" should not be used when "Equals" is overridden | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1698/) |
| `S1751` | Loops with at most one iteration should be refactored | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1751/) |
| `S1764` | Identical expressions should not be used on both sides of operators | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1764/) |
| `S1821` | “switch” statements should not be nested | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1821/) |
| `S1848` | Objects should not be created to be dropped immediately without being used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1848/) |
| `S1854` | Unused assignments should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1854/) |
| `S1862` | Related 'if/else if' statements should not have the same condition | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1862/) |
| `S1871` | Two branches in a conditional structure should not have exactly the same implementation | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1871/) |
| `S1905` | Redundant casts should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1905/) |
| `S1939` | Inheritance list should not be redundant | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1939/) |
| `S1940` | Boolean checks should not be inverted | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1940/) |
| `S1944` | Invalid casts should be avoided | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1944/) |
| `S1994` | “for” loop increment clauses should modify the loops’ counters | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-1994/) |
| `S2068` | Credentials should not be hard-coded | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2068/) |
| `S2092` | Creating cookies without the "secure" flag is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2092/) |
| `S2094` | Classes should not be empty | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2094/) |
| `S2114` | Collections should not be passed as arguments to their own methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2114/) |
| `S2115` | A secure password should be used when connecting to a database | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2115/) |
| `S2123` | Values should not be uselessly incremented | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2123/) |
| `S2139` | Exceptions should be either logged or rethrown but not both | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2139/) |
| `S2148` | Underscores should be used to make large numbers readable | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2148/) |
| `S2156` | “sealed” classes should not have “protected” members | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2156/) |
| `S2166` | Classes named like "Exception" should extend "Exception" or a subclass | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2166/) |
| `S2178` | Short-circuit logic should be used in boolean contexts | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2178/) |
| `S2183` | Integral numbers should not be shifted by zero or more than their number of bits-1 | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2183/) |
| `S2184` | Results of integer division should not be assigned to floating point variables | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2184/) |
| `S2187` | Test classes should contain at least one test case | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2187/) |
| `S2197` | Modulus results should not be checked for direct equality | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2197/) |
| `S2198` | Unnecessary mathematical comparisons should not be made | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2198/) |
| `S2219` | Runtime type checking should be simplified | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2219/) |
| `S2221` | “Exception” should not be caught | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2221/) |
| `S2225` | ToString() method should not return null | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2225/) |
| `S2234` | Arguments should be passed in the same order as the method parameters | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2234/) |
| `S2245` | Using pseudorandom number generators (PRNGs) is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2245/) |
| `S2251` | A "for" loop update clause should move the counter in the right direction | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2251/) |
| `S2252` | For-loop conditions should be true at least once | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2252/) |
| `S2257` | Using non-standard cryptographic algorithms is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2257/) |
| `S2291` | Overflow checking should not be disabled for "Enumerable.Sum" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2291/) |
| `S2292` | Trivial properties should be auto-implemented | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2292/) |
| `S2302` | nameof should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2302/) |
| `S2306` | “async” and “await” should not be used as identifiers | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2306/) |
| `S2325` | Methods and properties that don't access instance data should be static | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2325/) |
| `S2326` | Unused type parameters should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2326/) |
| `S2327` | “try” statements with identical “catch” and/or “finally” blocks should be merged | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2327/) |
| `S2328` | GetHashCode should not reference mutable fields | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2328/) |
| `S2330` | Array covariance should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2330/) |
| `S2333` | Redundant modifiers should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2333/) |
| `S2339` | Public constant members should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2339/) |
| `S2342` | Enumeration types should comply with a naming convention | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2342/) |
| `S2344` | Enumeration type names should not have "Flags" or "Enum" suffixes | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2344/) |
| `S2345` | Flags enumerations should explicitly initialize all their members | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2345/) |
| `S2346` | Flags enumerations zero-value members should be named "None" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2346/) |
| `S2360` | Optional parameters should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2360/) |
| `S2365` | Properties should not make collection or array copies | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2365/) |
| `S2368` | Public methods should not have multidimensional array parameters | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2368/) |
| `S2387` | Child class fields should not shadow parent class fields | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2387/) |
| `S2436` | Types and methods should not have too many generic parameters | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2436/) |
| `S2437` | Unnecessary bit operations should not be performed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2437/) |
| `S2445` | Blocks should be synchronized on read-only fields | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2445/) |
| `S2479` | Whitespace and control characters in string literals should be explicit | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2479/) |
| `S2486` | Generic exceptions should not be ignored | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2486/) |
| `S2612` | File permissions should not be set to world-accessible values | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2612/) |
| `S2681` | Multiline blocks should be enclosed in curly braces | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2681/) |
| `S2692` | IndexOf checks should not be for positive numbers | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2692/) |
| `S2696` | Instance members should not write to “static” fields | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2696/) |
| `S2699` | Tests should include assertions | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2699/) |
| `S2701` | Literal boolean values should not be used in assertions | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2701/) |
| `S2737` | catch clauses should do more than rethrow | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2737/) |
| `S2743` | Static fields should not be used in generic types | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2743/) |
| `S2755` | XML parsers should not be vulnerable to XXE attacks | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2755/) |
| `S2757` | Non-existent operators like '=+' should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2757/) |
| `S2760` | Sequential tests should not check the same condition | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2760/) |
| `S2761` | Doubled prefix operators '!!' and '~~' should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2761/) |
| `S2857` | SQL keywords should be delimited by whitespace | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2857/) |
| `S2925` | Thread.Sleep should not be used in tests | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2925/) |
| `S2930` | IDisposables should be disposed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2930/) |
| `S2933` | Fields that are only assigned in the constructor should be 'readonly' | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2933/) |
| `S2934` | Property assignments should not be made for 'readonly' fields not constrained to reference types | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2934/) |
| `S2955` | Generic parameters not constrained to reference types should not be compared to "null" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2955/) |
| `S2970` | Assertions should be complete | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2970/) |
| `S2971` | LINQ expressions should be simplified | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2971/) |
| `S2996` | ThreadStatic fields should not be initialized | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2996/) |
| `S2997` | IDisposables created in a 'using' statement should not be returned | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-2997/) |
| `S3005` | ThreadStatic should not be used on non-static fields | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3005/) |
| `S3010` | Static fields should not be updated in constructors | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3010/) |
| `S3011` | Reflection should not be used to increase accessibility of classes, methods, or fields | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3011/) |
| `S3052` | Members should not be initialized to default values | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3052/) |
| `S3059` | Types should not have members with visibility set higher than the type's visibility | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3059/) |
| `S3060` | "is" should not be used with "this" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3060/) |
| `S3063` | “StringBuilder” data should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3063/) |
| `S3168` | async methods should not return void | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3168/) |
| `S3172` | Delegates should not be subtracted | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3172/) |
| `S3215` | “interface” instances should not be cast to concrete types | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3215/) |
| `S3216` | "ConfigureAwait(false)" should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3216/) |
| `S3217` | Explicit conversions of foreach loops should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3217/) |
| `S3218` | Inner class members should not shadow outer class 'static' or type members | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3218/) |
| `S3220` | Method calls should not resolve ambiguously to overloads with "params" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3220/) |
| `S3234` | GC.SuppressFinalize should not be invoked for types without destructors | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3234/) |
| `S3235` | Redundant parentheses should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3235/) |
| `S3236` | Caller information arguments should not be provided explicitly | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3236/) |
| `S3237` | "value" contextual keyword should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3237/) |
| `S3240` | The simplest possible condition syntax should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3240/) |
| `S3241` | Methods should not return values that are never used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3241/) |
| `S3242` | Method parameters should be declared with base types | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3242/) |
| `S3246` | Generic type parameters should be co/contravariant when possible | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3246/) |
| `S3247` | Duplicate casts should not be made | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3247/) |
| `S3249` | Classes directly extending "object" should not call "base" in "GetHashCode" or "Equals" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3249/) |
| `S3251` | Implementations should be provided for "partial" methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3251/) |
| `S3253` | Constructor and destructor declarations should not be redundant | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3253/) |
| `S3256` | string.IsNullOrEmpty should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3256/) |
| `S3257` | Declarations and initializations should be as concise as possible | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3257/) |
| `S3260` | Non-derived 'private' classes and records should be 'sealed' | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3260/) |
| `S3261` | Namespaces should not be empty | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3261/) |
| `S3262` | "params" should be used on overrides | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3262/) |
| `S3263` | Static fields should appear in the order they must be initialized | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3263/) |
| `S3264` | Events should be invoked | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3264/) |
| `S3265` | Non-flags enums should not be used in bitwise operations | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3265/) |
| `S3267` | Loops should be simplified with "LINQ" expressions | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3267/) |
| `S3330` | Creating cookies without the "HttpOnly" flag is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3330/) |
| `S3343` | Caller information parameters should come at the end of the parameter list | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3343/) |
| `S3346` | Expressions used in "Debug.Assert" should not produce side effects | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3346/) |
| `S3353` | Unchanged variables should be marked as "const" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3353/) |
| `S3358` | Ternary operators should not be nested | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3358/) |
| `S3363` | Date and time should not be used as a type for primary keys | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3363/) |
| `S3366` | this should not be exposed from constructors | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3366/) |
| `S3376` | Attribute, EventArgs, and Exception type names should end with the type being extended | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3376/) |
| `S3397` | base.Equals should not be used to check for reference equality in Equals if base is not object | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3397/) |
| `S3398` | “private” methods called only by inner classes should be moved to those classes | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3398/) |
| `S3400` | Methods should not return constants | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3400/) |
| `S3415` | Assertion arguments should be passed in the correct order | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3415/) |
| `S3427` | Method overloads with default parameter values should not overlap | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3427/) |
| `S3431` | [ExpectedException] attribute should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3431/) |
| `S3433` | Test method signatures should be correct | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3433/) |
| `S3440` | Variables should not be checked against the values they're about to be assigned | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3440/) |
| `S3441` | Redundant property names should be omitted in anonymous classes | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3441/) |
| `S3444` | Interfaces should not simply inherit from base interfaces with colliding members | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3444/) |
| `S3447` | [Optional] should not be used on ref or out parameters | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3447/) |
| `S3449` | Right operands of shift operators should be integers | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3449/) |
| `S3453` | Classes should not have only 'private' constructors | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3453/) |
| `S3456` | string.ToCharArray() and ReadOnlySpan&lt;T&gt;.ToArray() should not be called redundantly | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3456/) |
| `S3458` | Empty 'case' clauses that fall through to the 'default' should be omitted | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3458/) |
| `S3459` | Unassigned members should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3459/) |
| `S3464` | Type inheritance should not be recursive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3464/) |
| `S3466` | Optional parameters should be passed to "base" calls | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3466/) |
| `S3532` | Empty default clauses should be removed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3532/) |
| `S3597` | ServiceContract and OperationContract attributes should be used together | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3597/) |
| `S3598` | One-way "OperationContract" methods should have "void" return type | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3598/) |
| `S3600` | params should not be introduced on overrides | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3600/) |
| `S3603` | Methods with "Pure" attribute should return a value | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3603/) |
| `S3604` | Member initializer values should not be redundant | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3604/) |
| `S3610` | Nullable type comparison should not be redundant | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3610/) |
| `S3626` | Jump statements should not be redundant | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3626/) |
| `S3717` | Track use of "NotImplementedException" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3717/) |
| `S3776` | Cognitive Complexity of methods should not be too high | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3776/) |
| `S3869` | SafeHandle.DangerousGetHandle should not be called | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3869/) |
| `S3872` | Parameter names should not duplicate the names of their methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3872/) |
| `S3874` | “out” and “ref” parameters should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3874/) |
| `S3878` | Arrays should not be created for params parameters | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3878/) |
| `S3884` | CoSetProxyBlanket and CoInitializeSecurity should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3884/) |
| `S3885` | Assembly.Load should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3885/) |
| `S3887` | Mutable, non-private fields should not be "readonly" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3887/) |
| `S3889` | Thread.Resume and Thread.Suspend should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3889/) |
| `S3897` | Classes that provide "Equals(<T>)" should implement "IEquatable<T>" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3897/) |
| `S3898` | Value types should implement "IEquatable<T>" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3898/) |
| `S3902` | Assembly.GetExecutingAssembly should not be called | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3902/) |
| `S3906` | Event Handlers should have the correct signature | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3906/) |
| `S3908` | Generic event handlers should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3908/) |
| `S3923` | All branches in a conditional structure should not have exactly the same implementation | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3923/) |
| `S3925` | ISerializable should be implemented correctly | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3925/) |
| `S3926` | Deserialization methods should be provided for OptionalField members | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3926/) |
| `S3927` | Serialization event handlers should be implemented correctly | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3927/) |
| `S3937` | Number patterns should be regular | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3937/) |
| `S3956` | Generic.List instances should not be part of public APIs | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3956/) |
| `S3967` | Multidimensional arrays should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3967/) |
| `S3971` | GC.SuppressFinalize should not be called | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3971/) |
| `S3981` | Collection sizes and array length comparisons should make sense | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3981/) |
| `S3984` | Exceptions should not be created without being thrown | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3984/) |
| `S3994` | URI Parameters should not be strings | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3994/) |
| `S3995` | URI return values should not be strings | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3995/) |
| `S3996` | URI properties should not be strings | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3996/) |
| `S3997` | String URI overloads should call "System.Uri" overloads | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-3997/) |
| `S4000` | Pointers to unmanaged memory should not be visible | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4000/) |
| `S4004` | Collection properties should be readonly | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4004/) |
| `S4005` | System.Uri arguments should be used instead of strings | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4005/) |
| `S4015` | Inherited member visibility should not be decreased | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4015/) |
| `S4016` | Enumeration members should not be named "Reserved" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4016/) |
| `S4017` | Method signatures should not contain nested generic types | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4017/) |
| `S4018` | All type parameters should be used in the parameter list to enable type inference | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4018/) |
| `S4022` | Enumerations should have "Int32" storage | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4022/) |
| `S4023` | Interfaces should not be empty | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4023/) |
| `S4025` | Child class fields should not differ from parent class fields only by capitalization | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4025/) |
| `S4027` | Exceptions should provide standard constructors | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4027/) |
| `S4035` | Classes implementing "IEquatable<T>" should be sealed | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4035/) |
| `S4036` | Searching OS commands in PATH is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4036/) |
| `S4040` | Strings should be normalized to uppercase | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4040/) |
| `S4041` | Type names should not match namespaces | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4041/) |
| `S4047` | Generics should be used when appropriate | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4047/) |
| `S4049` | Properties should be preferred | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4049/) |
| `S4057` | Locales should be set for data types | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4057/) |
| `S4061` | “params” should be used instead of “varargs” | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4061/) |
| `S4136` | Method overloads should be grouped together | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4136/) |
| `S4143` | Collection elements should not be replaced unconditionally | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4143/) |
| `S4144` | Methods should not have identical implementations | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4144/) |
| `S4159` | Classes should implement their "ExportAttribute" interfaces | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4159/) |
| `S4200` | Native methods should be wrapped | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4200/) |
| `S4201` | Null checks should not be combined with 'is' operator checks | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4201/) |
| `S4211` | Members should not have conflicting transparency annotations | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4211/) |
| `S4212` | Serialization constructors should be secured | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4212/) |
| `S4220` | Events should have proper arguments | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4220/) |
| `S4225` | Extension methods should not extend "object" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4225/) |
| `S4226` | Extensions should be in separate namespaces | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4226/) |
| `S4260` | ConstructorArgument parameters should exist in constructors | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4260/) |
| `S4261` | Methods should be named according to their synchronicities | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4261/) |
| `S4275` | Getters and setters should access the expected fields | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4275/) |
| `S4277` | Shared parts should not be created with new | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4277/) |
| `S4426` | Cryptographic keys should be robust | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4426/) |
| `S4428` | PartCreationPolicyAttribute should be used with ExportAttribute | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4428/) |
| `S4433` | LDAP connections should be authenticated | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4433/) |
| `S4456` | Parameter validation in yielding methods should be wrapped | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4456/) |
| `S4457` | Parameter validation in "async"/"await" methods should be wrapped | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4457/) |
| `S4502` | Disabling CSRF protections is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4502/) |
| `S4507` | Delivering code in production with debug features activated is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4507/) |
| `S4524` | “default” clauses should be first or last | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4524/) |
| `S4583` | Calls to delegate's method "BeginInvoke" should be paired with calls to "EndInvoke" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4583/) |
| `S4635` | Start index should be used instead of calling Substring | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4635/) |
| `S4663` | Comments should not be empty | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4663/) |
| `S4792` | Configuring loggers is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-4792/) |
| `S5042` | Expanding archive files should not be done without controlling resource consumption | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5042/) |
| `S5122` | Having a permissive Cross-Origin Resource Sharing policy is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5122/) |
| `S5332` | Using clear-text protocols is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5332/) |
| `S5344` | Passwords should not be stored in plaintext or with a fast hashing algorithm | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5344/) |
| `S5443` | Using publicly writable directories is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5443/) |
| `S5445` | Insecure temporary file creation methods should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5445/) |
| `S5542` | Encryption algorithms should be used with secure mode and padding scheme | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5542/) |
| `S5659` | JWT should be signed and verified with strong cipher algorithms | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5659/) |
| `S5693` | Allowing requests with excessive content length is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5693/) |
| `S5753` | Disabling ASP.NET Request Validation feature is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5753/) |
| `S5766` | Creating Serializable objects without data validation checks is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5766/) |
| `S5856` | Regular expressions should be syntactically valid | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-5856/) |
| `S6354` | Use a testable date/time provider | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6354/) |
| `S6377` | XML signatures should be validated securely | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6377/) |
| `S6418` | Secrets should not be hard-coded | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6418/) |
| `S6444` | Not specifying a timeout for regular expressions is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6444/) |
| `S6507` | Blocks should not be synchronized on local variables | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6507/) |
| `S6513` | ExcludeFromCodeCoverage attributes should include a justification | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6513/) |
| `S6561` | Avoid using "DateTime.Now" for benchmarking or timing operations | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6561/) |
| `S6562` | Always set the DateTimeKind when creating new DateTime instances | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6562/) |
| `S6563` | Use UTC when recording DateTime instants | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6563/) |
| `S6566` | Use "DateTimeOffset" instead of "DateTime" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6566/) |
| `S6575` | Use "TimeZoneInfo.FindSystemTimeZoneById" without converting the timezones with "TimezoneConverter" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6575/) |
| `S6585` | Don't hardcode the format when turning dates and times to strings | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6585/) |
| `S6602` | Find method should be used instead of the FirstOrDefault extension on List<T> | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6602/) |
| `S6603` | The collection-specific TrueForAll method should be used instead of the All extension on List<T> | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6603/) |
| `S6605` | Collection-specific "Exists" method should be used instead of the "Any" extension on List<T> | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6605/) |
| `S6609` | Min/Max properties of Set types should be used instead of the Enumerable extension methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6609/) |
| `S6612` | The lambda parameter should be used instead of capturing arguments in ConcurrentDictionary methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6612/) |
| `S6613` | “First” and “Last” properties of “LinkedList” should be used instead of the “First()” and “Last()” extension methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6613/) |
| `S6617` | "Contains" should be used instead of "Any" for simple equality checks | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6617/) |
| `S6618` | "string.Create" should be used instead of "FormattableString" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6618/) |
| `S6640` | Using unsafe code blocks is security-sensitive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6640/) |
| `S6664` | The code block contains too many logging calls | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6664/) |
| `S6667` | Logging in a catch clause should pass the caught exception as a parameter | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6667/) |
| `S6668` | Logging arguments should be passed to the correct parameter | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6668/) |
| `S6669` | Logger field or property name should comply with a naming convention | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6669/) |
| `S6670` | "Trace.Write" and "Trace.WriteLine" should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6670/) |
| `S6673` | Log message template placeholders should be in the right order | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6673/) |
| `S6674` | Log message template should be syntactically correct | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6674/) |
| `S6675` | Trace.WriteLineIf should not be used with TraceSwitch levels | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6675/) |
| `S6677` | Message template placeholders should be unique | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6677/) |
| `S6797` | Blazor query parameter type should be supported | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6797/) |
| `S6798` | [JSInvokable] attribute should only be used on public methods | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6798/) |
| `S6800` | Component parameter type should match the route parameter type constraint | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6800/) |
| `S6802` | Using lambda expressions in loops should be avoided in Blazor markup section | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6802/) |
| `S6803` | Parameters with SupplyParameterFromQuery attribute should be used only in routable components | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6803/) |
| `S6930` | Backslash should be avoided in route templates | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6930/) |
| `S6931` | ASP.NET controller actions should not have a route template starting with "/" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6931/) |
| `S6932` | Use model binding instead of reading raw request data | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6932/) |
| `S6934` | A Route attribute should be added to the controller when a route template is specified at the action level | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6934/) |
| `S6960` | Controllers should not have mixed responsibilities | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6960/) |
| `S6961` | API Controllers should derive from ControllerBase instead of Controller | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6961/) |
| `S6962` | Pool HTTP connections with HttpClientFactory — flags new HttpClient() created inside a public action method of an ASP.NET Core controller class (deriving from ControllerBase or Controller), where the instance is not stored for reuse (e.g. not a field initializer, not assigned to a static member, not in a constructor). | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6962/) |
| `S6964` | Value type property used as input in a controller action should be nullable, required, or annotated with JsonRequiredAttribute to avoid under-posting | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6964/) |
| `S6965` | REST API actions should be annotated with an HTTP verb attribute | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6965/) |
| `S6966` | Awaitable method should be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6966/) |
| `S6967` | ModelState.IsValid should be called in controller actions | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6967/) |
| `S6968` | Actions that return a value should be annotated with ProducesResponseTypeAttribute containing the return type | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-6968/) |
| `S7039` | Content Security Policies should be restrictive | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-7039/) |
| `S818` | Literal suffixes should be upper case | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-818/) |
| `S8367` | Identifiers should not conflict with the C# 14 'field' contextual keyword | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-8367/) |
| `S8368` | Identifiers should not conflict with the C# 14 'extension' contextual keyword | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-8368/) |
| `S8380` | Return types named "partial" should be escaped with "@" | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-8380/) |
| `S8381` | "scoped" should be escaped when used as an identifier or type name in parenthesized lambda parameter lists | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-8381/) |
| `S881` | Increment (++) and decrement (--) operators should not be used in a method call or mixed with other operators in an expression | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-881/) |
| `S907` | goto statement should not be used | warning | [docs](https://rules.sonarsource.com/csharp/RSPEC-907/) |

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

