namespace Eywa.Domain.HumanResources.PeopleManagements.Commons.Scaffolders;
internal static class PeopleManagementDefault
{
    public static string GetRegistrationContent(this string password, in string url) => $$"""
    <h3>默認密碼：<span style="border: 1px solid gray; font-weight: bold; padding: 4px;">{{password}}</span></h3>
    <p>請點擊以下鏈接進行登錄：<a href="{{url}}">登錄</a></p>
    """;
}