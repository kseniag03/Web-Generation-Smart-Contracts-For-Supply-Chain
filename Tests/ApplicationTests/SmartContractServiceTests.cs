
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Moq;

namespace Tests.ApplicationTests
{
    public class SmartContractServiceTests
    {
        private readonly Mock<ISolutionPathProvider> _solutionPathProviderMock;
        private readonly Mock<IContractModelProvider> _contractModelProviderMock;
        private readonly Mock<ISmartContractRepository> _contractRepositoryMock;
        private readonly Mock<ITemplateRepository> _templateRepositoryMock;
        private readonly SmartContractService _smartContractService;

        public SmartContractServiceTests()
        {
            _solutionPathProviderMock = new Mock<ISolutionPathProvider>();
            _contractModelProviderMock = new Mock<IContractModelProvider>();
            _contractRepositoryMock = new Mock<ISmartContractRepository>();
            _templateRepositoryMock = new Mock<ITemplateRepository>();

            _smartContractService = new SmartContractService(
                _solutionPathProviderMock.Object,
                _contractModelProviderMock.Object,
                _contractRepositoryMock.Object,
                _templateRepositoryMock.Object
            );
        }

        [Fact]
        public void GenerateContractCode_ShouldReturnGeneratedCode()
        {
            var contractName = "TestContract";
            var expectedCode = $"generated code for {contractName}";

            _ = _templateRepositoryMock.Setup(repo => repo.GenerateContractCode(AppConstants.DefaultContractAreaPath, AppConstants.DefaultYamlContent, string.Empty))
                .Returns(() => expectedCode);

            var dto = new ContractParamsDto { };
            var result = _smartContractService.GenerateContractCode(dto, string.Empty);

            Assert.Equal(expectedCode, result?.Result?.Code);
        }

        [Fact]
        public void GetContractCode_WithExistingContract_ShouldReturnCode()
        {
            var contractName = "ExistingContract";
            var expectedCode = $"contract {contractName} code: WANTED";

            _contractRepositoryMock.Setup(repo => repo.GetContractCode(contractName, string.Empty))
                .Returns(expectedCode);

            var result = string.Empty; // _smartContractService.GetContractCode(contractName, string.Empty);

            Assert.Equal(expectedCode, result);
        }
    }
}
