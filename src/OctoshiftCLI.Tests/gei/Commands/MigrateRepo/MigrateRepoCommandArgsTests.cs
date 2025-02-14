using FluentAssertions;
using Moq;
using OctoshiftCLI.GithubEnterpriseImporter.Commands.MigrateRepo;
using OctoshiftCLI.Services;
using Xunit;

namespace OctoshiftCLI.Tests.GithubEnterpriseImporter.Commands.MigrateRepo
{
    public class MigrateRepoCommandArgsTests
    {
        private readonly Mock<OctoLogger> _mockOctoLogger = TestHelpers.CreateMock<OctoLogger>();

        private const string GHES_API_URL = "https://myghes/api/v3";
        private const string AZURE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=mykey;EndpointSuffix=core.windows.net";
        private const string SOURCE_ORG = "foo-source-org";
        private const string SOURCE_REPO = "foo-repo-source";
        private const string TARGET_ORG = "foo-target-org";
        private const string TARGET_REPO = "foo-target-repo";
        private const string GITHUB_TARGET_PAT = "github-target-pat";
        private const string AWS_BUCKET_NAME = "aws-bucket-name";

        [Fact]
        public void AdoServer_Source_Without_SourceOrg_Provided_Throws_Error()
        {
            var args = new MigrateRepoCommandArgs
            {
                AdoServerUrl = "https://ado.contoso.com",
                AdoTeamProject = "FooProj",
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO
            };

            FluentActions
                .Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should()
                .ThrowExactly<OctoshiftCliException>();
        }

        [Fact]
        public void No_Source_Provided_Throws_Error()
        {
            var args = new MigrateRepoCommandArgs
            {
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                TargetApiUrl = ""
            };

            FluentActions
                .Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should()
                .ThrowExactly<OctoshiftCliException>();
        }

        [Fact]
        public void Ado_Source_Without_Team_Project_Throws_Error()
        {
            var args = new MigrateRepoCommandArgs
            {
                AdoSourceOrg = SOURCE_ORG,
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                TargetApiUrl = ""
            };

            FluentActions
                .Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should()
                .ThrowExactly<OctoshiftCliException>();
        }

        [Fact]
        public void Defaults_TargetRepo_To_SourceRepo()
        {
            var args = new MigrateRepoCommandArgs
            {
                GithubSourceOrg = SOURCE_ORG,
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                Wait = true
            };

            args.Validate(_mockOctoLogger.Object);

            args.TargetRepo.Should().Be(SOURCE_REPO);
        }

        [Fact]
        public void It_Falls_Back_To_Github_Target_Pat_If_Github_Source_Pat_Is_Not_Provided()
        {
            var args = new MigrateRepoCommandArgs
            {
                GithubSourceOrg = SOURCE_ORG,
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                GithubTargetPat = GITHUB_TARGET_PAT,
                QueueOnly = true,
            };

            args.Validate(_mockOctoLogger.Object);

            args.GithubSourcePat.Should().Be(GITHUB_TARGET_PAT);
        }

        [Fact]
        public void Aws_Bucket_Name_Without_Ghes_Api_Url_Throws()
        {
            var args = new MigrateRepoCommandArgs
            {
                SourceRepo = SOURCE_REPO,
                GithubSourceOrg = SOURCE_ORG,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                AwsBucketName = AWS_BUCKET_NAME
            };

            FluentActions.Invoking(() => args.Validate(_mockOctoLogger.Object))
                 .Should()
                 .ThrowExactly<OctoshiftCliException>()
                 .WithMessage("*--aws-bucket-name*");
        }

        [Fact]
        public void No_Ssl_Verify_Without_Ghes_Api_Url_Throws()
        {
            var args = new MigrateRepoCommandArgs
            {
                SourceRepo = SOURCE_REPO,
                GithubSourceOrg = SOURCE_ORG,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                NoSslVerify = true
            };

            FluentActions.Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should()
                .ThrowExactly<OctoshiftCliException>()
                .WithMessage("*--no-ssl-verify*");
        }

        [Fact]
        public void Keep_Archive_Without_Ghes_Api_Url_Throws()
        {
            var args = new MigrateRepoCommandArgs
            {
                SourceRepo = SOURCE_REPO,
                GithubSourceOrg = SOURCE_ORG,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                KeepArchive = true
            };

            FluentActions.Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should()
                .ThrowExactly<OctoshiftCliException>()
                .WithMessage("*--keep-archive*");
        }

        [Fact]
        public void Validates_Wait_And_QueueOnly_Not_Passed_Together()
        {
            var args = new MigrateRepoCommandArgs
            {
                GithubSourceOrg = SOURCE_ORG,
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                GhesApiUrl = GHES_API_URL,
                AzureStorageConnectionString = AZURE_CONNECTION_STRING,
                Wait = true,
                KeepArchive = true,
                QueueOnly = true,
            };

            FluentActions.Invoking(() => args.Validate(_mockOctoLogger.Object))
                               .Should()
                               .ThrowExactly<OctoshiftCliException>()
                               .WithMessage("*wait*");
        }

        [Fact]
        public void Wait_Flag_Shows_Warning()
        {
            var args = new MigrateRepoCommandArgs
            {
                GithubSourceOrg = SOURCE_ORG,
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                GhesApiUrl = GHES_API_URL,
                AzureStorageConnectionString = AZURE_CONNECTION_STRING,
                Wait = true,
                KeepArchive = true,
            };

            args.Validate(_mockOctoLogger.Object);

            _mockOctoLogger.Verify(x => x.LogWarning(It.Is<string>(x => x.ToLower().Contains("wait"))));
        }

        [Fact]
        public void No_Wait_And_No_Queue_Only_Flags_Shows_Warning()
        {
            var args = new MigrateRepoCommandArgs
            {
                GithubSourceOrg = SOURCE_ORG,
                SourceRepo = SOURCE_REPO,
                GithubTargetOrg = TARGET_ORG,
                TargetRepo = TARGET_REPO,
                GhesApiUrl = GHES_API_URL,
                AzureStorageConnectionString = AZURE_CONNECTION_STRING,
                Wait = false,
                QueueOnly = false,
                KeepArchive = true,
            };

            args.Validate(_mockOctoLogger.Object);

            _mockOctoLogger.Verify(x => x.LogWarning(It.Is<string>(x => x.ToLower().Contains("wait"))));
        }
    }
}
