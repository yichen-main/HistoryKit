global using SystemPath = System.IO.Path;

namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface IConfigurePicker
{
    string? GetAppSettings(in string key);
    T? GetAppSettings<T>() where T : class;
    T? GetXmlFile<T>(in string? fullPath);
    IEnumerable<IFileInfo> GetEywaLinkXmlFile(in string directoryName);
    IEnumerable<IFileInfo> GetEywaLinkCsvFile(in string directoryName);
    ValueTask SetEywaLinkXmlAsync<T>(in string directoryName, in string fileName, in T entity) where T : class;
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class ConfigurePicker(IConfiguration configuration) : IConfigurePicker
{
    public string? GetAppSettings(in string key) => configuration.GetSection(key).Value;
    public T? GetAppSettings<T>() where T : class => configuration.GetSection(typeof(T).Name).Get<T>();
    public T? GetXmlFile<T>(in string? fullPath)
    {
        try
        {
            if (string.IsNullOrEmpty(fullPath)) return default;
            using var reader = XmlReader.Create(fullPath);
            XmlSerializer serializer = new(typeof(T));
            return (T?)serializer.Deserialize(reader);
        }
        catch (Exception)
        {
            return default;
        }
    }
    public IEnumerable<IFileInfo> GetEywaLinkXmlFile(in string directoryName)
    {
        var directoryPath = SystemPath.Combine(FileLayout.EywaLinkFolderLocation, directoryName);
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        return EywaLinkProvider.GetDirectoryContents(directoryName).Where(x => FileLayout.IsXmlFile(x.PhysicalPath));
    }
    public IEnumerable<IFileInfo> GetEywaLinkCsvFile(in string directoryName) =>
        EywaLinkProvider.GetDirectoryContents(directoryName).Where(x => FileLayout.IsCsvFile(x.PhysicalPath));
    public ValueTask SetEywaLinkXmlAsync<T>(in string directoryName, in string fileName, in T entity)
        where T : class => FileLayout.WriteXmlFileAsync(directoryName, entity, fileName);
    static PhysicalFileProvider EywaLinkProvider => new(FileLayout.EywaLinkFolderLocation);
}