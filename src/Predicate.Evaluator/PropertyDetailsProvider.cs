namespace Predicate.Evaluator
{
    public interface IPropertyDetailsProvider
    {
        PropertyDetails GetPropertyDetails(string propertyName);
    }

    public class PropertyDetailsProvider : IPropertyDetailsProvider
    {
        public PropertyDetails GetPropertyDetails(string propertyName)
        {
            switch (propertyName)
            {
                case "current_job_title":
                    return new PropertyDetails(ConcreteType.String, "currentJobTitle");
                case "experience_years":
                    return new PropertyDetails(ConcreteType.Number, "experienceInYears");
                case "salary":
                    return new PropertyDetails(ConcreteType.Number, "salary");
                default:
                    return null;
            }
        }
    }

    public class PropertyDetails
    {
        public PropertyDetails(ConcreteType type, string sourceName)
        {
            Type = type;
            SourceName = sourceName;
        }
        public ConcreteType Type { get; }
        public string SourceName { get; }
    }

    public enum ConcreteType
    {
        Number,
        String
    }
}