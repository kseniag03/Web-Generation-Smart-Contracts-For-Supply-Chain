using System.ComponentModel.DataAnnotations;
using Application.DTOs;
using Application.Services;
using Application.Common;
using Application.Specifications.Yaml;
using Application.Interfaces;
using Moq;

namespace Tests.WebTests
{
    public class ValidationAndServiceTests
    {
        private static IList<ValidationResult> Validate(object dto)
        {
            var ctx = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(dto, ctx, results, validateAllProperties: true);

            return results;
        }

        [Fact]
        public void LoginDto_EmptyLogin_ShouldHaveRequiredError()
        {
            var dto = new LoginDto { Login = "", Password = "Valid123!" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(LoginDto.Login)) &&
                e.ErrorMessage == "Login required");
        }

        [Fact]
        public void LoginDto_InvalidLoginChars_ShouldHaveRegexError()
        {
            var dto = new LoginDto { Login = "bad*name", Password = "Valid123!" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(LoginDto.Login)) &&
                e.ErrorMessage.Contains("only Latin letters"));
        }

        [Fact]
        public void LoginDto_ShortPassword_ShouldHaveMinLengthError()
        {
            var dto = new LoginDto { Login = "username", Password = "123" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(LoginDto.Password)) &&
                e.ErrorMessage.Contains("at least 6 characters"));
        }

        [Fact]
        public void RegisterDto_InvalidEmail_ShouldHaveEmailError()
        {
            var dto = new RegisterDto { Login = "user1", Password = "Valid123!", Email = "not-an-email" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(RegisterDto.Email)) &&
                e.ErrorMessage == "Incorrect email format");
        }

        [Fact]
        public void MetaMaskDto_InvalidWallet_ShouldHaveRegexError()
        {
            var dto = new MetaMaskDto { WalletAddress = "0x123" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(MetaMaskDto.WalletAddress)) &&
                e.ErrorMessage.StartsWith("WalletAddress must start"));
        }

        [Fact]
        public void ChangePasswordDto_EmptyNewPassword_ShouldHaveRequiredError()
        {
            var dto = new ChangePasswordDto { OldPassword = "Valid123!", NewPassword = "" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(ChangePasswordDto.NewPassword)) &&
                e.ErrorMessage == "New password required");
        }

        [Fact]
        public void ChangePasswordDto_OldPasswordWithSpace_ShouldHaveRegexError()
        {
            var dto = new ChangePasswordDto { OldPassword = "bad pass", NewPassword = "Valid123!" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(ChangePasswordDto.OldPassword)) &&
                e.ErrorMessage.Contains("only Latin letters"));
        }

        [Fact]
        public void ContractParamsDto_InvalidArea_ShouldHaveEnumError()
        {
            var dto = new ContractParamsDto { Area = "UnknownArea", LayoutYaml = "dummy" };
            var errors = Validate(dto);

            Assert.Contains(errors, e =>
                e is not null &&
                !string.IsNullOrWhiteSpace(e.ErrorMessage) &&
                e.MemberNames.Contains(nameof(ContractParamsDto.Area)) &&
                e.ErrorMessage.Contains("Area must be one of"));
        }

        [Fact]
        public void SmartContractService_GetInstancePath_InvalidContractName_Throws()
        {
            var dto = new ContractParamsDto
            {
                Area = AppConstants.EmptyContractAreaPath,
                LayoutYaml = "---"
            };

            var spMock = new Mock<ISolutionPathProvider>();
            var cmpMock = new Mock<IContractModelProvider>();
            var badModel = new ContractModel { ContractName = "Bad Name!" };

            spMock
                .Setup(s => s.GetSolutionRoot())
                .Returns(@"fake-root");

            cmpMock
                .Setup(c => c.GetContractModelFromYaml(It.Is<string>(yaml => yaml == dto.LayoutYaml)))
                .Returns(badModel);

            var svc = new SmartContractService(
                spMock.Object,
                cmpMock.Object,
                null!,
                null!);

            Assert.Throws<ValidationException>(() => svc.GetInstancePath(dto));
        }

        [Fact]
        public void SmartContractService_GetInstancePath_EmptyYaml_UsesDefault()
        {
            var dto = new ContractParamsDto
            {
                Area = AppConstants.EmptyContractAreaPath,
                LayoutYaml = null!
            };

            var spMock = new Mock<ISolutionPathProvider>();
            var cmpMock = new Mock<IContractModelProvider>();
            var model = new ContractModel { ContractName = "ValidName" };

            Assert.Null(dto.LayoutYaml);

            spMock
                .Setup(s => s.GetSolutionRoot())
                .Returns(@"fake-root");

            cmpMock
                .Setup(c => c.GetContractModelFromYaml(It.Is<string>(yaml => yaml == dto.LayoutYaml)))
                .Returns(model);

            var svc = new SmartContractService(
                spMock.Object,
                cmpMock.Object,
                null!,
                null!);

            Assert.Null(dto.LayoutYaml);

            _ = svc.GetInstancePath(dto);

            Assert.Equal(AppConstants.DefaultYamlContent, dto.LayoutYaml);
        }
    }
}
