namespace Eywa.Core.Architects.Primaries.Substances;
internal sealed class YamlProvider(FileConfigurationSource source) : FileConfigurationProvider(source)
{
    public override void Load(Stream stream)
    {
        YamlStream yamlStream = [];
        Stack<string> stackContext = new();
        yamlStream.Load(new StreamReader(stream));
        SortedDictionary<string, string?> datas = new(StringComparer.Ordinal);
        if (yamlStream.Documents.Count > (int)default) VisitNode(string.Empty, yamlStream.Documents[default].RootNode);
        void VisitNode(string context, YamlNode node)
        {
            string currentPath;
            switch (node)
            {
                case YamlScalarNode scalar:
                    EnterContext(context);
                    datas[currentPath] = scalar.Value ?? string.Empty;
                    ExitContext();
                    break;

                case YamlMappingNode mapping:
                    EnterContext(context);
                    foreach (KeyValuePair<YamlNode, YamlNode> children in mapping.Children)
                    {
                        context = ((YamlScalarNode)children.Key).Value ?? string.Empty;
                        VisitNode(context, children.Value);
                    }
                    ExitContext();
                    break;

                case YamlSequenceNode sequence:
                    EnterContext(context);
                    for (int item = default; item < sequence.Children.Count; item++)
                    {
                        VisitNode(item.ToString(CultureInfo.CurrentCulture), sequence.Children[item]);
                    }
                    ExitContext();
                    break;
            }
            void EnterContext(string context)
            {
                if (!string.IsNullOrEmpty(context)) stackContext.Push(context);
                currentPath = ConfigurationPath.Combine(stackContext.Reverse());
            }
            void ExitContext()
            {
                if (stackContext.Count is not (int)default) stackContext.Pop();
                currentPath = ConfigurationPath.Combine(stackContext.Reverse());
            }
        }
        Data = datas;
    }
}