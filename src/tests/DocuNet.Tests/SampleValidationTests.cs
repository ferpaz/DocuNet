namespace DocuNet.Tests;

// Private inputs are never needed by the normal cross-platform test suite.
public sealed class SampleValidationTests
{
    [Fact(Explicit = true)]
    public void AuditAndComparePrivateBaseline()
    {
        string? baseline = Environment.GetEnvironmentVariable("DOCUNET_BASELINE");
        string? generated = Environment.GetEnvironmentVariable("DOCUNET_GENERATED");
        string? resultDirectory = Environment.GetEnvironmentVariable("DOCUNET_VALIDATION_RESULTS");
        Assert.SkipWhen(baseline is null || generated is null || resultDirectory is null,
            "Provide DOCUNET_BASELINE, DOCUNET_GENERATED and DOCUNET_VALIDATION_RESULTS.");
        Assert.True(Directory.Exists(baseline));
        Assert.True(Directory.Exists(generated));
        Directory.CreateDirectory(resultDirectory!);
        var audit = HtmlSiteInspector.Inspect(generated!);
        var parser = new HtmlParser();
        string? acceptedPath = Environment.GetEnvironmentVariable("DOCUNET_ACCEPTED_DIFFERENCES");
        using var accepted = JsonDocument.Parse(acceptedPath is null ? "{\"signatureDifferences\":[],\"typeSummaryDifferences\":[]}" : File.ReadAllText(acceptedPath));
        var rules = accepted.RootElement.GetProperty("signatureDifferences").EnumerateArray().ToArray();
        var approvedSummaries = accepted.RootElement.GetProperty("typeSummaryDifferences").EnumerateArray().Select(e => e.GetString()!).ToArray();
        var acceptedSignatures = new List<string>();
        var missingPages = new List<string>();
        var missingSignatures = new List<string>();
        var addedSignatures = new List<string>();
        var changedSummaries = new List<string>();
        var changedMemberBodies = new List<string>();
        int baselineBlocks = 0;
        foreach (string page in Directory.EnumerateFiles(baseline!, "*.htm", SearchOption.AllDirectories).Order(StringComparer.Ordinal))
        {
            string relative = Path.GetRelativePath(baseline!, page);
            string target = Path.Combine(generated!, relative);
            if (!File.Exists(target)) { missingPages.Add(relative); continue; }
            using var before = parser.ParseDocument(File.ReadAllText(page));
            using var after = parser.ParseDocument(File.ReadAllText(target));
            var oldMembers = before.QuerySelectorAll("div.method > h4").Select(e => Normalize(e.TextContent)).ToArray();
            var newMembers = after.QuerySelectorAll("div.method > h4").Select(e => Normalize(e.TextContent)).ToArray();
            baselineBlocks += oldMembers.Length;
            foreach (string signature in oldMembers.Except(newMembers, StringComparer.Ordinal))
            {
                var rule = rules.FirstOrDefault(r => r.GetProperty("path").GetString() == relative.Replace(Path.DirectorySeparatorChar, '/')
                    && r.GetProperty("before").GetString() == signature && newMembers.Contains(r.GetProperty("after").GetString(), StringComparer.Ordinal));
                if (rule.ValueKind != JsonValueKind.Undefined) acceptedSignatures.Add(relative + ": " + signature + " -> " + rule.GetProperty("after").GetString());
                else missingSignatures.Add(relative + ": " + signature);
            }
            foreach (string signature in newMembers.Except(oldMembers, StringComparer.Ordinal))
                addedSignatures.Add(relative + ": " + signature);
            var newBlocks = after.QuerySelectorAll("div.method").GroupBy(b => Normalize(b.QuerySelector("h4")?.TextContent ?? ""), StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
            foreach (var oldBlock in before.QuerySelectorAll("div.method"))
            {
                string signature = Normalize(oldBlock.QuerySelector("h4")?.TextContent ?? "");
                newBlocks.TryGetValue(signature, out var newBlock);
                if (newBlock is not null && Normalize(oldBlock.QuerySelector(".content")?.TextContent ?? "") != Normalize(newBlock.QuerySelector(".content")?.TextContent ?? ""))
                    changedMemberBodies.Add(relative + ": " + signature);
            }
            string oldSummary = Normalize(before.QuerySelector("#summary")?.TextContent ?? "");
            string newSummary = Normalize(after.QuerySelector("#summary")?.TextContent ?? "");
            if (oldSummary != newSummary) changedSummaries.Add(relative);
        }
        var resourceDifferences = new List<string>();
        foreach (string resource in Directory.EnumerateFiles(baseline!, "*", SearchOption.AllDirectories)
            .Where(p => !p.EndsWith(".htm", StringComparison.OrdinalIgnoreCase)))
        {
            string relative = Path.GetRelativePath(baseline!, resource);
            string target = Path.Combine(generated!, relative);
            if (!File.Exists(target) || !SHA256.HashData(File.ReadAllBytes(resource)).SequenceEqual(SHA256.HashData(File.ReadAllBytes(target))))
                resourceDifferences.Add(relative);
        }
        var report = new
        {
            Audit = audit, BaselineMemberBlocks = baselineBlocks, MissingPages = missingPages,
            MissingSignatures = missingSignatures, AddedSignatures = addedSignatures,
            ChangedTypeSummaries = changedSummaries, ChangedMemberBodies = changedMemberBodies, AcceptedSignatureDifferences = acceptedSignatures, ResourceDifferences = resourceDifferences
        };
        File.WriteAllText(Path.Combine(resultDirectory!, "sample-comparison.json"),
            JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        Assert.Empty(audit.Failures);
        Assert.Empty(missingPages);
        Assert.Empty(resourceDifferences);
        Assert.Empty(missingSignatures);
        Assert.Equal(rules.Length, acceptedSignatures.Count);
        Assert.All(changedSummaries, page => Assert.Contains(page.Replace(Path.DirectorySeparatorChar, '/'), approvedSummaries));
    }

    private static string Normalize(string text) => Regex.Replace(text, @"\s+", " ").Trim();
}
