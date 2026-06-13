// Copyright (c) Codurance. All rights reserved.

namespace Opinionated.DotNet.CodingStandards.Tooling;

public record EditorConfigMergeResult(string RewrittenText, IReadOnlyList<string> AddedIds, IReadOnlyList<string> StaleIds);
