using System;
using System.CommandLine;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using OctoshiftCLI.Contracts;
using OctoshiftCLI.Services;

[assembly: InternalsVisibleTo("OctoshiftCLI.Tests")]

namespace OctoshiftCLI.Commands.WaitForMigration;

public class WaitForMigrationCommandBase : CommandBase<WaitForMigrationCommandArgs, WaitForMigrationCommandHandler>
{
    public WaitForMigrationCommandBase() : base(
        name: "wait-for-migration",
        description: "Waits for migration(s) to finish and reports all in progress and queued ones.")
    {
    }

    public virtual Option<string> MigrationId { get; } = new("--migration-id")
    {
        IsRequired = true,
        Description = "Waits for the specified migration to finish."
    };

    public virtual Option<string> GithubPat { get; } = new("--github-pat")
    {
        Description = "Personal access token of the GitHub target. Overrides GH_PAT environment variable."
    };

    public virtual Option<bool> Verbose { get; } = new("--verbose");

    protected void AddOptions()
    {
        AddOption(MigrationId);
        AddOption(GithubPat);
        AddOption(Verbose);
    }

    public override WaitForMigrationCommandHandler BuildHandler(WaitForMigrationCommandArgs args, IServiceProvider sp)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (sp is null)
        {
            throw new ArgumentNullException(nameof(sp));
        }

        var log = sp.GetRequiredService<OctoLogger>();
        var githubApi = sp.GetRequiredService<ITargetGithubApiFactory>().Create(targetPersonalAccessToken: args.GithubPat);

        return new WaitForMigrationCommandHandler(log, githubApi);
    }
}
