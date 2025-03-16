
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
            _contractRepositoryMock.Setup(repo => repo.GenerateContractCode(contractName))
                .Returns(expectedCode);

            // Act
            var result = _smartContractService.GenerateContractCode(contractName);

            // Assert
            Assert.Equal(expectedCode, result);
        }

        [Fact]
        public void GetContractCode_WithExistingContract_ShouldReturnCode()
        {
            // Arrange
            var contractName = "ExistingContract";
            var expectedCode = $"contract {contractName} code: WANTED";
            _contractRepositoryMock.Setup(repo => repo.GetContractCode(contractName))
                .Returns(expectedCode);

            // Act
            var result = _smartContractService.GetContractCode(contractName);

            // Assert
            Assert.Equal(expectedCode, result);
        }
    }
}
