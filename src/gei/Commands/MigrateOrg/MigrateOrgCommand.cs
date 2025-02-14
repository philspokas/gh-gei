﻿using System;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using OctoshiftCLI.Commands;
using OctoshiftCLI.Contracts;
using OctoshiftCLI.Services;

namespace OctoshiftCLI.GithubEnterpriseImporter.Commands.MigrateOrg
{
    public class MigrateOrgCommand : CommandBase<MigrateOrgCommandArgs, MigrateOrgCommandHandler>
    {
        public MigrateOrgCommand() : base(
            name: "migrate-org",
            description: "Invokes the GitHub APIs to migrate a GitHub org with its teams and the repositories.")
        {
            AddOption(GithubSourceOrg);
            AddOption(GithubTargetOrg);
            AddOption(GithubTargetEnterprise);

            AddOption(GithubSourcePat);
            AddOption(GithubTargetPat);
            AddOption(Wait);
            AddOption(QueueOnly);
            AddOption(Verbose);
        }

        public Option<string> GithubSourceOrg { get; } = new("--github-source-org")
        {
            IsRequired = true,
            Description = "Uses GH_SOURCE_PAT env variable or --github-source-pat option. Will fall back to GH_PAT or --github-target-pat if not set."
        };
        public Option<string> GithubTargetOrg { get; } = new("--github-target-org")
        {
            IsRequired = true,
            Description = "Uses GH_PAT env variable or --github-target-pat option."
        };
        public Option<string> GithubTargetEnterprise { get; } = new("--github-target-enterprise")
        {
            IsRequired = true,
            Description = "Name of the target enterprise."
        };
        public Option<string> GithubSourcePat { get; } = new("--github-source-pat");
        public Option<string> GithubTargetPat { get; } = new("--github-target-pat");
        public Option<bool> Wait { get; } = new("--wait")
        {
            IsHidden = true,
            Description = "Synchronously waits for the org migration to finish."
        };
        public Option<bool> QueueOnly { get; } = new("--queue-only")
        {
            Description = "Only queues the migration, does not wait for it to finish. Use the wait-for-migration command to subsequently wait for it to finish and view the status."
        };
        public Option<bool> Verbose { get; } = new("--verbose");

        public override MigrateOrgCommandHandler BuildHandler(MigrateOrgCommandArgs args, IServiceProvider sp)
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
            var environmentVariableProvider = sp.GetRequiredService<EnvironmentVariableProvider>();

            var targetGithubApiFactory = sp.GetRequiredService<ITargetGithubApiFactory>();
            var targetGithubApi = targetGithubApiFactory.Create(targetPersonalAccessToken: args.GithubTargetPat);

            return new MigrateOrgCommandHandler(log, targetGithubApi, environmentVariableProvider);
        }
    }
}
