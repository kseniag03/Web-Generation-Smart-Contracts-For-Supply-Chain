using Scriban;
using Scriban.Runtime;
using Microsoft.Extensions.Configuration;
using Infrastructure.Repositories.Helpers;
using System.Text.Json;
using Application.Common;
using Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Application.Specifications.Yaml;

namespace Infrastructure.Repositories
{
    public class ScribanRepository: ITemplateRepository
    {
        private readonly string _solutionDirectory;
        private readonly string _templatesPath;

        public ScribanRepository(IConfiguration configuration, IHostEnvironment hostEnv)
        {
            var tempatesPath = configuration["ContractTemplatesPath"];
            var configsPath = configuration["HardhatConfigsPath"];

            if (string.IsNullOrEmpty(tempatesPath) || string.IsNullOrEmpty(configsPath))
            {
                throw new ArgumentException("Templates or configs path is not found.");
            }

            _solutionDirectory = SolutionPathHelper.GetSolutionRoot(configuration, hostEnv);
            _templatesPath = Path.Combine(_solutionDirectory, tempatesPath);
        }

        public async Task<string> LoadAreaModelTemplate(string area)
        {
            var path = Path.Combine(_templatesPath, $"contract-spec-{area}.yaml");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"YAML template for '{area}' not found", path);
            }

            return await File.ReadAllTextAsync(path);
        }

        public async Task<string> GenerateContractCode(string area, string yaml, string instancePath)
        {
            return await GenerateCodeUsingSbnTemplate(area, yaml, instancePath, AppConstants.ContractSbn);
        }

        public async Task<string> GenerateContractTestScript(string area, string yaml, string instancePath)
        {
            return await GenerateCodeUsingSbnTemplate(area, yaml, instancePath, AppConstants.TestSbn);
        }

        public async Task<string> GenerateContractTestGasScript(string area, string yaml, string instancePath)
        {
            return await GenerateCodeUsingSbnTemplate(area, yaml, instancePath, AppConstants.TestGasSbn);
        }

        public string ExtractContractName(string yaml)
        {
            var spec = SpecLoader.LoadModelFromYaml(yaml);

            return spec.ContractName;
        }

        public async Task UpdateFileContent(string area, string yaml, string instancePath, string? content, string sbnType = AppConstants.DefaultSbnType)
        {
            var spec = SpecLoader.LoadModelFromYaml(yaml);

            await EditSourceCodeFile(area, spec, instancePath, content, sbnType);
        }

        private async Task<string> GenerateCodeUsingSbnTemplate(
            string area,
            string yaml,
            string instancePath,
            string sbnType = AppConstants.DefaultSbnType
        )
        {
            var sbnPath = Path.Combine(_templatesPath, $"{sbnType}.sbn");
            var sbnText = await File.ReadAllTextAsync(sbnPath);
            var spec = SpecLoader.LoadModelFromYaml(yaml);
            var template = Template.Parse(sbnText);
            var ctx = new TemplateContext();
            var script = new ScriptObject();

            script.Import(spec);
            ctx.PushGlobal(script);
            ctx.BuiltinObject.Import("json_stringify", new Func<object, string>(obj =>
                JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = true
                })
            ));

            var result = template.Render(ctx);

            await EditSourceCodeFile(area, spec, instancePath, result, sbnType);

            return result;
        }

        private async Task EditSourceCodeFile(
            string area,
            ContractModel spec,
            string instancePath,
            string? result,
            string sbnType = AppConstants.DefaultSbnType
        )
        {
            var outputPath = Path.Combine(instancePath, sbnType);

            Directory.CreateDirectory(outputPath);

            var extension =
                string.Equals(sbnType, AppConstants.ContractSbn, StringComparison.Ordinal) ||
                string.Equals(sbnType, AppConstants.TestGasSbn, StringComparison.Ordinal)
                    ? "sol"
                    : "js";

            var contractFilePath = Path.Combine(outputPath, $"{area}-{spec.ContractName}.{extension}");

            await File.WriteAllTextAsync(contractFilePath, result);
        }
    }
}
