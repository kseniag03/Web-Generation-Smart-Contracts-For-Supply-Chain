using Application.Common;
using YamlDotNet.Serialization;

namespace Application.Specifications.Yaml
{
    public class EventModel
    {
        public EventModel()
        {
            Name = AppConstants.DefaultEventName;
        }

        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "params")]
        public List<FieldModel> Params { get; set; } = new();
    }
}
