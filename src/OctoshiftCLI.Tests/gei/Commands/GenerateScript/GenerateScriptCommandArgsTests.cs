using System.IO;
using FluentAssertions;
using Moq;
using OctoshiftCLI.GithubEnterpriseImporter.Commands.GenerateScript;
using OctoshiftCLI.Services;
using Xunit;

namespace OctoshiftCLI.Tests.GithubEnterpriseImporter.Commands.GenerateScript
{
    public class GenerateScriptCommandArgsTests
    {
        private readonly Mock<OctoLogger> _mockOctoLogger = TestHelpers.CreateMock<OctoLogger>();

        private const string SOURCE_ORG = "FOO-SOURCE-ORG";
        private const string TARGET_ORG = "FOO-TARGET-ORG";
        private const string AWS_BUCKET_NAME = "AWS_BUCKET_NAME";

        [Fact]
        public void AdoServer_Source_Without_SourceOrg_Provided_Throws_Error()
        {
            var args = new GenerateScriptCommandArgs
            {
                AdoServerUrl = "https://ado.contoso.com",
                GithubTargetOrg = TARGET_ORG
            };

            FluentActions
                .Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should().Throw<OctoshiftCliException>();
        }

        [Fact]
        public void No_Github_Source_Org_Or_Ado_Source_Org_Throws()
        {
            var args = new GenerateScriptCommandArgs { GithubTargetOrg = TARGET_ORG };

            FluentActions
                .Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should().Throw<OctoshiftCliException>();
        }

        [Fact]
        public void It_Throws_When_Aws_Bucket_Name_Is_Provided_But_Ghes_Api_Url_Is_Not()
        {
            // Arrange
            var args = new GenerateScriptCommandArgs
            {
                GithubSourceOrg = SOURCE_ORG,
                GithubTargetOrg = TARGET_ORG,
                Output = new FileInfo("unit-test-output"),
                AwsBucketName = AWS_BUCKET_NAME,
                Sequential = true
            };

            // Act, Assert
            FluentActions.Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should()
                .Throw<OctoshiftCliException>();
        }

        [Fact]
        public void It_Throws_When_No_Ssl_Verify_Is_Set_But_Ghes_Api_Url_Is_Not()
        {
            // Arrange
            var args = new GenerateScriptCommandArgs
            {
                GithubSourceOrg = SOURCE_ORG,
                GithubTargetOrg = TARGET_ORG,
                Output = new FileInfo("unit-test-output"),
                NoSslVerify = true,
                Sequential = true
            };

            // Act, Assert
            FluentActions.Invoking(() => args.Validate(_mockOctoLogger.Object))
                .Should()
                .Throw<OctoshiftCliException>();
        }
    }
}
