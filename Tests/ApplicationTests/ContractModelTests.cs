using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Application.Specifications.Yaml;
using Application.Common;

public class ContractModelTests
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    [Fact]
    public void Deserialization_ShouldUseYamlValue_WhenContractNameIsSpecified()
    {
        const string ContractName = "MyCustomContract";

        var yaml = $"contractName: {ContractName}";
        var model = _deserializer.Deserialize<ContractModel>(yaml);

        Assert.Equal(ContractName, model.ContractName);
    }

    [Fact]
    public void Deserialization_ShouldUseDefault_WhenContractNameIsMissing()
    {
        var yaml = AppConstants.DefaultYamlContent;
        var model = _deserializer.Deserialize<ContractModel>(yaml);

        Assert.Equal(AppConstants.DefaultContractName, model.ContractName);
    }
}
