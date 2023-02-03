
namespace Pccl.ProjectTemplate.Domain.Entities.SettingAggregate
{
    public class Setting : AuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Description { get; private set; }

        private Setting()
        {
            // required by EF
        }

        public Setting(string name,string value,string description)
        {
            Name= name;
            Value= value;
            Description= description;
        }

        public void SetName(string name)
        {
            Name = name;
        }
        public void SetValue(string value)
        { 
            Value= value;
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
    }
}