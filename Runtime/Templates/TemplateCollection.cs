using ZJM_JsonTool.Runtime.Interfaces;
namespace ZJM_JsonTool.Runtime.Templates
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
