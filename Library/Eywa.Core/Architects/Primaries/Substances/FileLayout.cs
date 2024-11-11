namespace Eywa.Core.Architects.Primaries.Substances;
public readonly ref struct FileLayout
{
    public static bool IsFile(in string? path) => path.ExistFile();
    public static bool IsCsvFile(in string? path) => path.ExistFile(FileExtension.CSV);
    public static bool IsXmlFile(in string? path) => path.ExistFile(FileExtension.XML);
    public static void ClearEmptyFolders(in string path) => path.ToDeleteEmptyFolder();
    public static string GetProjectName<T>() => GetProjectName(typeof(T));
    public static string GetProjectName(in Type type) => type.Assembly.GetName().Name!;
    public static string GetProjectName<T>(in T entity) where T : notnull => GetProjectName(entity.GetType());
    public static string GetRootFolderPath(in string directoryName) => directoryName.GetFolderPath();
    public static string GetLogFolderPath(in string directoryName) => directoryName.GetFolderPath(Folder.Log);
    public static string GetConfigFolderPath(in string directoryName) => directoryName.GetFolderPath(Folder.Config);
    public static string GetModuleFilePath(in string fileName) => Path.Combine(ModuleFolderLocation, fileName);
    public static T? GetRootJsonFile<T>(string fileName = "appsettings.json") => ReadJsonFile<T>(RootFolderLocation, fileName);
    public static async ValueTask<T> GetForemostProfile<T>(string fileName, CancellationToken ct = default)
    {
        var fullPath = GetFullFilePath(ConfigFolderLocation, fileName, FileExtension.YAML);
        var result = await ReadYamlFileAsync<T>(fullPath, notExistCreate: false, ct).ConfigureAwait(false);
        if (result is not null) await WriteYamlFileAsync(fullPath, result, ct).ConfigureAwait(false);
        return result!;
    }
    public static string GetFullFilePath(string directoryName, in string fileName, in string extensionName)
    {
        return Path.Combine(directoryName, $"{Path.GetFileNameWithoutExtension(fileName)}{extensionName}");
    }
    public static string GetSnowflakeId() => IdAlgorithm.Next().ToString(CultureInfo.InvariantCulture);
    public static ValueTask<T?> ReadYamlFileAsync<T>(in string fullPath, bool notExistCreate, in CancellationToken ct)
    {
        return fullPath.ReadYamlFileAsync<T>(notExistCreate, ct);
    }
    public static ValueTask<T?> ReadYamlFileAsync<T>(in string filePath, in string fileName, bool notExistCreate, in CancellationToken ct)
    {
        return filePath.ReadYamlFileAsync<T>(fileName, notExistCreate, ct);
    }
    public static ValueTask<T?> ReadYamlFileAsync<T>(in T content, in string fullPath, bool notExistCreate, in CancellationToken ct)
    {
        return content.ReadYamlFileAsync(fullPath, notExistCreate, ct);
    }
    public static T? ReadJsonFile<T>(in string filePath, in string fileName) => filePath.ReadJsonFile<T>(fileName);
    public static T? ReadXmlFile<T>(in string filePath, in string fileName) => filePath.ReadXmlFile<T>(fileName);
    public static ValueTask WriteYamlFileAsync<T>(in string fullPath, in T content, in CancellationToken ct)
    {
        return fullPath.WriteYamlFileAsync(content, ct);
    }
    public static ValueTask WriteYamlFileAsync<T>(in string filePath, in string fileName, in T content, in CancellationToken ct)
    {
        return filePath.WriteYamlFileAsync(fileName, content, ct);
    }
    public static async ValueTask WriteXmlFileAsync<T>(string directoryName, T content, string fileName = nameof(File))
    {
        var filePath = Path.Combine(directoryName.GetFolderPath(Folder.EywaLink), $"{fileName}{FileExtension.XML}");
        if (!File.Exists(filePath))
        {
            StreamWriter writer = new(filePath);
            await using (writer.ConfigureAwait(false))
            {
                new XmlSerializer(typeof(T), string.Empty).Serialize(XmlWriter.Create(writer, new()
                {
                    Indent = true,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = true,
                }), content, new([new XmlQualifiedName(string.Empty, string.Empty)]));
            }
        }
    }
    public static Task LogAsync(in object value) => RecordAsync(nameof(System), Regex.Unescape(value.ToJson()));
    public static ValueTask OutputAsync(in HistoryLetter item) => HistoryTransfer.Writer.WriteAsync(item);
    public static string LogFolderLocation => Folder.Log.GetFolderPath();
    public static string RootFolderLocation => Folder.Root.GetFolderPath();
    public static string ModuleFolderLocation => Folder.Module.GetFolderPath();
    public static string ConfigFolderLocation => Folder.Config.GetFolderPath();
    public static string EywaLinkFolderLocation => Folder.EywaLink.GetFolderPath();
    public static string RetentionFolderLocation => Folder.Retention.GetFolderPath();
}