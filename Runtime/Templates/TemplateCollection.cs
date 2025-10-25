namespace TechCosmos.Serialization.Runtime
{
    public class TemplateCollection<T> : IJsonDataCollection<T>
    {
        public T[] _targets;
        public T[] targets
        {
            get => _targets;
            set => _targets = value;
        }
    }
}
