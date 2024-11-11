namespace Eywa.Serve.Constructs.Adminiculars.Composers;
public abstract class NodeEnlarge<T1, T2>(RolePolicy role = RolePolicy.Viewer) : Endpoint<T2> where T2 : notnull
{
    protected void RequiredConfiguration(int cacheSeconds = 10)
    {
        RoutingSettings();
        DontCatchExceptions();
        Options(x => x.CacheOutput(x => x.Expire(TimeSpan.FromSeconds(cacheSeconds))));
        void RoutingSettings()
        {
            switch (Type.Name)
            {
                case var item when item.IsFuzzy("List"):
                    Get(Route);
                    break;

                case var item when item.IsFuzzy("Get"):
                    Get($"{Route}/{{id}}");
                    break;

                case var item when item.IsFuzzy("Enum"):
                    Get($"{Route}/{Type.Name
                        .Replace("Enum", string.Empty, StringComparison.Ordinal)
                        .Replace("Vehicle", string.Empty, StringComparison.Ordinal).LetConvertPath()}");
                    break;

                case var item when item.IsFuzzy("Export"):
                    Get($"{Route}/exports");
                    break;

                case var item when item.IsFuzzy("View"):
                    Get($"{Route}/remove-displays");
                    break;

                case var item when item.IsFuzzy("Add"):
                    Post(Route);
                    break;

                case var item when item.IsFuzzy("Put"):
                    Put(Route);
                    break;

                case var item when item.IsFuzzy("Import"):
                    Put($"{Route}/imports");
                    break;

                case var item when item.IsFuzzy("Clear"):
                    Delete(Route);
                    break;

                case var item when item.IsFuzzy("Del"):
                    Delete($"{Route}/{{id}}");
                    break;
            }
        }
    }
    protected string GetUserName()
    {
        var claim = HttpContext.User.FindFirst(ClaimTypes.Name);
        return claim is not null ? claim.Value : string.Empty;
    }
    protected string GetUserId() => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    protected IEnumerable<T> Pagination<T>(in PageContents<T> contents, in IEnumerable<(string key, string? value)> queries)
    {
        HttpContext.Pagination(contents, $"{BaseURL}{Route}", queries);
        return contents;
    }
    protected async ValueTask VerifyAsync(CancellationToken ct)
    {
        var rootUser = await IsRootUserAsync(ct).ConfigureAwait(false);
        if (!rootUser) throw new Exception(DurableSetup.Link(AccountAccessFlag.IncorrectAccessPermissions));
    }
    async ValueTask<bool> IsRootUserAsync(CancellationToken ct)
    {
        var profile = await CiphertextPolicy.GetRootFileAsync(ct).ConfigureAwait(false);
        return GetUserId().IsMatch(profile.Id);
    }
    protected async ValueTask VerifyAsync(FieldModule type, CancellationToken ct)
    {
        var rootUserAsync = IsRootUserAsync(ct);
        var moduleName = DurableSetup.Link(type);
        var templet = DurableSetup.Link(AccountAccessFlag.IncorrectModulePermissions);
        var fieldMembersAsync = CacheMediator.ListFieldMemberAsync(ct);
        var fieldOwnersAsync = CacheMediator.ListFieldOwnerAsync(ct);
        var fieldOwners = await fieldOwnersAsync.ConfigureAwait(false);
        var rootUser = await rootUserAsync.ConfigureAwait(false);
        if (rootUser) throw new Exception(DurableSetup.Link(AccountAccessFlag.IncorrectAccessPermissions));
        if (!fieldOwners.Any(x => x.FieldType == type))
        {
            List<FieldTeamQueryImport.Output> fieldMembers = [];
            if (CacheMediator.ExistFieldModule(FieldModule.HumanResources))
            {
                fieldMembers.AddRange(await fieldMembersAsync.ConfigureAwait(false));
            }
            var fieldMember = fieldMembers.Find(x => x.Id.IsMatch(GetUserId()) && x.FieldType == type);
            switch (fieldMember)
            {
                case var x when x is { Id: not null }:
                    Authorization(fieldMember.RoleType);
                    break;

                default:
                    throw new Exception(string.Format(templet, moduleName));
            }
        }
        void Authorization(in RolePolicy policy)
        {
            switch (role)
            {
                case RolePolicy.Owner:
                    if (policy > RolePolicy.Owner) throw new Exception(DurableSetup.Link(AccountAccessFlag.IncorrectAccessPermissions));
                    break;

                case RolePolicy.Editor:
                    if (policy > RolePolicy.Editor) throw new Exception(DurableSetup.Link(AccountAccessFlag.IncorrectAccessPermissions));
                    break;

                case RolePolicy.Viewer:
                    if (policy > RolePolicy.Viewer) throw new Exception(DurableSetup.Link(AccountAccessFlag.IncorrectAccessPermissions));
                    break;
            }
        }
    }
    string Route
    {
        get
        {
            var paths = Type.Namespace!
                .Replace(nameof(Eywa), string.Empty, StringComparison.Ordinal)
                .Replace(nameof(Serve), string.Empty, StringComparison.Ordinal)
                .Replace("Endpoints", string.Empty, StringComparison.Ordinal)
                .Replace("..", ".", StringComparison.Ordinal).TrimStart('.')
                .TrimEnd('.').Split(".");
            StringBuilder builder = new($"{RESTfulLayout.Prefix}/");
            for (var i = default(int); i < paths.Length; i++)
            {
                var tag = paths[i].LetConvertPath();
                if (i == paths.Length - 1) builder.Append(tag);
                else builder.Append($"{tag}/");
            }
            return builder.ToString();
        }
    }
    Type Type { get; } = typeof(T1);
    public required IValidator<T2> Validator { get; init; }
    public required IDurableSetup DurableSetup { get; init; }
    public required ICacheMediator CacheMediator { get; init; }
    public required ICiphertextPolicy CiphertextPolicy { get; init; }
}