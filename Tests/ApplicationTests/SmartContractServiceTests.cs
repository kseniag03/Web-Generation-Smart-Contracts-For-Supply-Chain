
using Application.DTOs;
using Application.Services;
using Core.Interfaces;
using Moq;

namespace Tests.ApplicationTests
{
    public class SmartContractServiceTests
    {
        private readonly Mock<ISmartContractRepository> _contractRepositoryMock;
        private readonly SmartContractService _smartContractService;

        public SmartContractServiceTests()
        {
            _contractRepositoryMock = new Mock<ISmartContractRepository>();
            _smartContractService = new SmartContractService(_contractRepositoryMock.Object);
        }

        [Fact]
        public void GenerateContractCode_ShouldReturnGeneratedCode()
        {
            // Arrange
            var contractName = "TestContract";
            var expectedCode = $"generated code for {contractName}";
            _contractRepositoryMock.Setup(repo => repo.GenerateContractCode(contractName, "area", "uint8", false, false, string.Empty))
                .Returns(expectedCode);

            var dto = new ContractParamsDto { 
                ContractName = contractName,
                ApplicationArea = "area",
                UintType = "uint8",
                EnableEvents = false,
                IncludeVoidLabel = false,
                TargetOs = "windows",
            };

            // Act
            var result = _smartContractService.GenerateContractCode(dto, string.Empty);

            // Assert
            Assert.Equal(expectedCode, result);
        }

        [Fact]
        public void GetContractCode_WithExistingContract_ShouldReturnCode()
        {
            // Arrange
            var contractName = "ExistingContract";
            var expectedCode = $"contract {contractName} code: WANTED";
            _contractRepositoryMock.Setup(repo => repo.GetContractCode(contractName, string.Empty))
                .Returns(expectedCode);

            // Act
            var result = _smartContractService.GetContractCode(contractName, string.Empty);

            // Assert
            Assert.Equal(expectedCode, result);
        }
    }
}
