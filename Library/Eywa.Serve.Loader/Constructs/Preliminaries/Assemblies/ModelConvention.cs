namespace Eywa.Serve.Loader.Constructs.Preliminaries.Assemblies;
internal sealed class ModelConvention : IControllerModelConvention
{
    public void Apply(ControllerModel model)
    {
        if (model.Selectors.Count == default) model.Selectors.Add(new());
        if (model.Selectors.Any(x => x.AttributeRouteModel is not null)) return;
        for (int i = default; i < model.Selectors.Count; i++)
        {
            model.Selectors[i].AttributeRouteModel = new()
            {
                Template = AttributeRouteModel.CombineTemplates(RESTfulLayout.Prefix, GetRoutingPath(model)),
            };
        }
        for (int i = default; i < model.Actions.Count; i++)
        {
            if (model.Actions[i].Selectors.Any(x => x.AttributeRouteModel is not null)) continue;
            if (model.Actions[i].Selectors.Count is (int)default) model.Actions[i].Selectors.Add(new SelectorModel());
            for (int item = default; item < model.Actions[i].Selectors.Count; item++)
            {
                model.Actions[i].Selectors[item].AttributeRouteModel = new()
                {
                    Template = model.Actions[i].ActionName,
                };
            }
        }
    }
    static string GetRoutingPath(in ControllerModel model)
    {
        var endIndex = model.DisplayName.IndexOf(')', StringComparison.Ordinal);
        var startIndex = model.DisplayName.IndexOf('(', StringComparison.Ordinal) + 1;
        var projectTag = $"{model.DisplayName[startIndex..endIndex]}.";
        var path = model.ControllerType.FullName!.Replace(projectTag, string.Empty, StringComparison.Ordinal).Split(".");
        return Route(path);
        static string Route(string[] paths)
        {
            StringBuilder result = new();
            for (var i = default(int); i < paths.Length; i++)
            {
                var tag = paths[i].LetConvertPath();
                if (i == paths.Length - 1) result.Append(tag);
                else result.Append($"{tag}/");
            }
            return result.ToString();
        }
    }
}