namespace Eywa.Domain.HumanResources.PeopleManagements.Commons.Quarterlies;
internal interface IPeopleManagementTool
{
    string Link(in PeopleManagementCulture culture);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class PeopleManagementTool : IPeopleManagementTool
{
    public string Link(in PeopleManagementCulture culture) => BaseCreator.Culture(culture.ToString());
    public required IBaseCreator BaseCreator { get; init; }
}