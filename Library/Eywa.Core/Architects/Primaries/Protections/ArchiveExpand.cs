namespace Eywa.Core.Architects.Primaries.Protections;
internal static class ArchiveExpand
{
    const string upperPath = "..";
    public enum Folder
    {
        Root,
        Log,
        Module,
        Config,
        EywaLink,
        Retention,
    }
    public static void ToDeleteEmptyFolder(this string directoryPath)
    {
        if (directoryPath.IsEmptyFolder()) directoryPath.DeleteFolder();
    }
    public static bool IsEmptyFolder(this string directoryPath) =>
        Directory.GetDirectories(directoryPath).Length == default && Directory.GetFiles(directoryPath).Length == default;
    public static bool ExistFile(this string? path) => File.Exists(path);
    public static bool ExistFile(this string? path, in string? extension) => Path.GetExtension(path).IsMatch(extension);
    public static void DeleteFolder(this string directoryPath)
    {
        var entries = Directory.GetFileSystemEntries(directoryPath);
        for (var i = (int)default; i < entries.Length; i++)
        {
            var path = entries[i];
            if (File.Exists(path))
            {
                var attributes = File.GetAttributes(path);
                if ((attributes & FileAttributes.ReadOnly) is FileAttributes.ReadOnly)
                {
                    File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                }
                File.Delete(path);
            }
            else path.DeleteFolder();
        }
        Directory.Delete(directoryPath);
    }
    public static T? ReadJsonFile<T>(this string filePath, in string fileName)
    {
        var builder = new ConfigurationBuilder().SetBasePath(filePath).AddJsonFile(fileName, optional: true, reloadOnChange: true);
        return builder.AddEnvironmentVariables().Build().GetSection(typeof(T).Name).Get<T>()!;
    }
    public static T? ReadXmlFile<T>(this string filePath, in string fileName)
    {
        try
        {
            using var reader = XmlReader.Create(Path.Combine(filePath, fileName));
            XmlSerializer serializer = new(typeof(T));
            return (T?)serializer.Deserialize(reader);
        }
        catch (Exception)
        {
            return default;
        }
    }
    public static ValueTask<T?> ReadYamlFileAsync<T>(this string filePath, in string fileName, bool notExistCreate, CancellationToken ct)
    {
        return ReadYamlFileAsync<T>(FileLayout.GetFullFilePath(filePath, fileName, FileExtension.YAML), notExistCreate, ct);
    }
    public static ValueTask<T?> ReadYamlFileAsync<T>(this string fullPath, bool notExistCreate, CancellationToken ct)
    {
        return ReadYamlFileAsync(Activator.CreateInstance<T>(), fullPath, notExistCreate, ct);
    }
    public static async ValueTask<T?> ReadYamlFileAsync<T>(this T content, string fullPath, bool notExistCreate, CancellationToken ct)
    {
        try
        {
            if (!File.Exists(fullPath) && notExistCreate is false)
            {
                await fullPath.WriteYamlFileAsync(content, ct).ConfigureAwait(false);
            }
            ConfigurationBuilder builder = new();
            YamlConfigure configure = new()
            {
                Path = fullPath,
                Optional = default,
                FileProvider = default,
                ReloadOnChange = default,
            };
            configure.ResolveFileProvider();
            builder.Add(configure).Deserialize(content);
            return content;
        }
        catch (Exception)
        {
            return default;
        }
    }
    public static ValueTask WriteYamlFileAsync<T>(this string filePath, in string fileName, T contente, CancellationToken ct)
    {
        return WriteYamlFileAsync(FileLayout.GetFullFilePath(filePath, fileName, FileExtension.YAML), contente, ct);
    }
    public static async ValueTask WriteYamlFileAsync<T>(this string fullPath, T content, CancellationToken ct)
    {
        var fileStream = File.Create(fullPath);
        await using (fileStream.ConfigureAwait(false))
        {
            var buffers = Encoding.UTF8.GetBytes(new SerializerBuilder().Build().Serialize(content));
            var memory = buffers.AsMemory(default, buffers.Length);
            {
                await fileStream.WriteAsync(memory, ct).ConfigureAwait(false);
            }
        }
    }
    public static string GetFolderPath(this Folder type) => Directory.CreateDirectory(type switch
    {
        Folder.Log => Path.Combine(Folder.Root.GetFolderPath(), upperPath, nameof(Folder.Log)),
        Folder.Module => Path.Combine(Folder.Root.GetFolderPath(), nameof(Folder.Module)),
        Folder.Config => Path.Combine(Folder.Root.GetFolderPath(), upperPath, nameof(Folder.Config)),
        Folder.EywaLink => Path.Combine(Folder.Config.GetFolderPath(), nameof(Folder.EywaLink)),
        Folder.Retention => Path.Combine(Folder.Config.GetFolderPath(), nameof(Folder.Retention)),
        _ => AppContext.BaseDirectory
    }).FullName;
    public static string GetFolderPath(this string directoryName, in Folder type = Folder.Root) => Directory.CreateDirectory(type switch
    {
        Folder.Log => Path.Combine(Folder.Log.GetFolderPath(), directoryName),
        Folder.Module => Path.Combine(Folder.Root.GetFolderPath(), nameof(Folder.Module), directoryName),
        Folder.Config => Path.Combine(Folder.Config.GetFolderPath(), directoryName),
        Folder.EywaLink => Path.Combine(Folder.Config.GetFolderPath(), nameof(Folder.EywaLink), directoryName),
        Folder.Retention => Path.Combine(Folder.Config.GetFolderPath(), nameof(Folder.Retention), directoryName),
        _ => Path.Combine(Folder.Root.GetFolderPath(), directoryName)
    }).FullName;
    public static Task RecordAsync(in string directoryName, in string message, in string format = "yyyyMMdd", in int offset = 8)
    {
        var banner = new StringBuilder().AppendLine(message).ToString();
        var fileName = $"{DateTime.UtcNow.AddHours(offset).ToString(format, CultureInfo.InvariantCulture)}{FileExtension.Log}";
        return File.AppendAllTextAsync(Path.Combine(directoryName.GetFolderPath(Folder.Log), fileName), banner, Encoding.UTF8);
    }
    public static void ExecutionScavenger(in int retentionDays = 10, in string timeFormat = "yyyyMMdd")
    {
        if (Directory.Exists(FileLayout.LogFolderLocation))
        {
            foreach (var directoryInfo in new DirectoryInfo(FileLayout.LogFolderLocation).GetDirectories())
            {
                foreach (var filePath in Directory.GetFiles(directoryInfo.FullName))
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (DateTime.TryParseExact(fileName, timeFormat, default, DateTimeStyles.None, out var date))
                    {
                        if (Timestamp.Subtract(date).TotalDays > retentionDays) File.Delete(filePath);
                    }
                    else File.Delete(filePath);
                }
                if (Directory.GetFiles(directoryInfo.FullName).Length == default) Directory.Delete(directoryInfo.FullName);
            }
            FileLayout.ClearEmptyFolders(FileLayout.LogFolderLocation);
        }
    }
    public static DateTime Timestamp { get { return DateTime.UtcNow.AddHours(8); } }
    public static Channel<HistoryLetter> HistoryTransfer { get; } = Channel.CreateUnbounded<HistoryLetter>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = true,
    });
}