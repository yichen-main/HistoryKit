namespace Eywa.Core.Architects.Primaries.Protections;
public static class ProfileExpand
{
    public static string RandomInteger64Bits()
    {
        var buffers = new byte[8];
        new Random().NextBytes(buffers);
        var longRand = BitConverter.ToInt64(buffers, default);
        var minValue = long.Parse(1.ToString().PadRight(16, '0'), CultureInfo.InvariantCulture);
        var maxValue = long.Parse(9.ToString().PadRight(16, '9'), CultureInfo.InvariantCulture);
        return (Math.Abs(longRand % (maxValue - minValue)) + minValue).ToString(CultureInfo.CurrentCulture);
    }
    public static string RandomEncode256Bits() => GenerateKey(32);
    public static string GenerateKey(in int keySizeInBytes)
    {
        var buffers = new byte[keySizeInBytes];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(buffers);
        return buffers.ToEncodeBase64();
    }
    public static string ToEncryptAES(this string plainText, in string salt, in string iv,
        in CipherMode mode = CipherMode.CBC, in PaddingMode padding = PaddingMode.PKCS7)
    {
        using Aes aes = Aes.Create();
        {
            aes.Key = Encoding.UTF8.GetBytes(salt);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Mode = mode;
            aes.Padding = padding;
        }
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, default, plainBytes.Length);
        return encryptedBytes.ToHexString();
    }
    public static string ToDecryptAES(this string encryptedText, in string salt, in string iv,
        in CipherMode mode = CipherMode.CBC, in PaddingMode padding = PaddingMode.PKCS7)
    {
        using Aes aes = Aes.Create();
        {
            aes.Key = Encoding.UTF8.GetBytes(salt);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Mode = mode;
            aes.Padding = padding;
        }
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var encryptedBytes = Convert.FromHexString(encryptedText);
        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, default, encryptedBytes.Length);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
    public static string ToMD5(this string text) => MD5.HashData(Encoding.UTF8.GetBytes(text)).ToHexString();
    public static string ToSHA256(this string text) => SHA256.HashData(Encoding.UTF8.GetBytes(text)).ToHexString();
    public static byte[] ToHmacSHA256(this string text, in byte[] salt)
    {
        using HMACSHA256 result = new(salt);
        return result.ComputeHash(Encoding.UTF8.GetBytes(text));
    }
    public static string ToHexString(this byte[] bytes) => Convert.ToHexString(bytes);
    public static string ToEncodeBase64(this string text) => Encoding.UTF8.GetBytes(text).ToEncodeBase64();
    public static string ToEncodeBase64(this byte[] bytes) => Convert.ToBase64String(bytes);
    public static string ToDecodeBase64(this string text) => Convert.FromBase64String(text).ToDecodeBase64();
    public static string ToDecodeBase64(this byte[] bytes)
    {
        try
        {
            return Encoding.UTF8.GetString(bytes);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
    public static void Deserialize<T>(this IConfigurationBuilder builder, in T profile) => builder.Build().Bind(profile);
    public static async ValueTask KeepLogAsync()
    {
        await foreach (var info in HistoryTransfer.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            var names = info.Type.Name.Split("__");
            var directory = names.Length is 2 ? names[1] : info.Type.Name;
            var title = $"[{Timestamp.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}]";
            var label = $" {info.Name} ({string.Create(CultureInfo.InvariantCulture, $"{info.Line}")})";
            var content = info.Content is not null ? $"{info.Message} {Regex.Unescape(info.Content.ToJson())}" : info.Message;
            await RecordAsync(directory, $"{title}{label} {content}").ConfigureAwait(false);
            await title.PrintAsync(ConsoleColor.DarkRed, newline: false).ConfigureAwait(false);
            await $" {directory}".PrintAsync(ConsoleColor.DarkGray, newline: false).ConfigureAwait(false);
            await label.PrintAsync(ConsoleColor.DarkGray).ConfigureAwait(false);
            await content.PrintAsync(ConsoleColor.DarkMagenta).ConfigureAwait(false);
            await string.Empty.PrintAsync(ConsoleColor.White).ConfigureAwait(false);
        }
    }
}