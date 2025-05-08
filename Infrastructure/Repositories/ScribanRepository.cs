using Scriban;
using Scriban.Runtime;
using Microsoft.Extensions.Configuration;
using Infrastructure.Repositories.Helpers;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public class ScribanRepository
    {
        private readonly string _solutionDirectory;
        private readonly string _templatesPath;

        public ScribanRepository(IConfiguration configuration)
        {
            var tempatesPath = configuration["ContractTemplatesPath"];
            var configsPath = configuration["HardhatConfigsPath"];

            if (string.IsNullOrEmpty(tempatesPath) || string.IsNullOrEmpty(configsPath))
            {
                throw new ArgumentException("Templates or configs path is not found.");
            }

            _solutionDirectory = SolutionPathHelper.GetSolutionRoot(configuration);
            _templatesPath = Path.Combine(_solutionDirectory, tempatesPath);
        }

        public void Init(string areaPath = "Empty")
        {
            var yamlPath = Path.Combine(_templatesPath, $"contract-spec-{areaPath}.yaml");
            var sbnPath = Path.Combine(_templatesPath, "contract.sbn");

            var spec = SpecLoader.LoadModelFromYaml(yamlPath);
            var template = Template.Parse(File.ReadAllText(sbnPath));
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
            var outputPath = Path.Combine(_templatesPath, $"out/{areaPath}-{spec.ContractName}.sol");

            // File.WriteAllText(outputPath, result);
        }
    }
}
