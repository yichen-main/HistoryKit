namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface ICiphertextPolicy
{
    string GenerateSalt();
    string GenerateRandomIV();
    string GenerateRandomPassword(in int length = 8);
    string PaddingKey(in string secret);
    string HashMD5(in string plainText);
    string HashSHA256(in string plainText);
    string EncodeBase64(in string plainText);
    string DecodeBase64(in string encryptedText);
    byte[] HmacSHA256(in string plainText, in byte[] salt);
    byte[] HmacSHA256(in string plainText, in string salt);
    string HmacSHA256ToHex(in string plainText, in string salt);
    string EncryptAES(in string plainText, in string salt, in string iv);
    string DecryptAES(in string encryptedText, in string salt, in string iv);
    ValueTask<EmployerProfile> GetRootFileAsync(CancellationToken ct);
    ValueTask SetRootAsync(in EmployerProfile profile, in CancellationToken ct);
    bool IsValidPassword(in string password, in byte minLength, in byte maxLength);
    string RootFilePath { get; }
}

[Dependent(ServiceLifetime.Singleton)]
file sealed partial class CiphertextPolicy : ICiphertextPolicy
{
    public string GenerateSalt()
    {
        var bytes = new byte[32];
        using (var generator = RandomNumberGenerator.Create()) generator.GetBytes(bytes);
        return bytes.ToEncodeBase64();
    }
    public string GenerateRandomIV() => ProfileExpand.RandomInteger64Bits();
    public string GenerateRandomPassword(in int length = 8)
    {
        const int matchQuantity = 3;
        StringBuilder password = new();
        var rng = RandomNumberGenerator.Create();
        var uintBuffer = new byte[sizeof(uint)];
        string[] charTypes = ["ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz", "0123456789", "!@#$%^&*()_+",];
        var selectedCharTypes = string.Join(string.Empty, charTypes.OrderBy(x => new Random().Next()).Take(matchQuantity));
        foreach (var charType in charTypes.Take(matchQuantity))
        {
            rng.GetBytes(uintBuffer);
            var num = BitConverter.ToUInt32(uintBuffer, default);
            password.Append(charType[(int)(num % (uint)charType.Length)]);
        }
        while (password.Length < length)
        {
            rng.GetBytes(uintBuffer);
            var num = BitConverter.ToUInt32(uintBuffer, default);
            password.Append(selectedCharTypes[(int)(num % (uint)selectedCharTypes.Length)]);
        }
        rng.GetBytes(uintBuffer);
        var passwordArray = password.ToString().ToCharArray();
        Random random = new(BitConverter.ToInt32(uintBuffer, default));
        for (var i = passwordArray.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (passwordArray[j], passwordArray[i]) = (passwordArray[i], passwordArray[j]);
        }
        return new string(passwordArray);
    }
    public string PaddingKey(in string secret) => secret.PadRight(32, '0');
    public string HashMD5(in string plainText) => plainText.ToMD5();
    public string HashSHA256(in string plainText) => plainText.ToSHA256();
    public string EncodeBase64(in string plainText) => plainText.ToEncodeBase64();
    public string DecodeBase64(in string encryptedText) => encryptedText.ToDecodeBase64();
    public byte[] HmacSHA256(in string plainText, in byte[] salt) => plainText.ToHmacSHA256(salt);
    public byte[] HmacSHA256(in string plainText, in string salt) => HmacSHA256(plainText, Encoding.UTF8.GetBytes(salt));
    public string HmacSHA256ToHex(in string plainText, in string salt) => HmacSHA256(plainText, salt).ToHexString();
    public string EncryptAES(in string plainText, in string salt, in string iv) => plainText.ToEncryptAES(salt, iv);
    public string DecryptAES(in string encryptedText, in string salt, in string iv) => encryptedText.ToDecryptAES(salt, iv);
    public async ValueTask<EmployerProfile> GetRootFileAsync(CancellationToken ct)
    {
        var profileAsync = FileLayout.ReadYamlFileAsync<EmployerProfile>(RootFilePath, notExistCreate: true, ct);
        var profile = await profileAsync.ConfigureAwait(false);
        return profile is null ? throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierSystemAccountNotSet)) : profile;
    }
    public ValueTask SetRootAsync(in EmployerProfile profile, in CancellationToken ct) => FileLayout.WriteYamlFileAsync(RootFilePath, profile, ct);
    public bool IsValidPassword(in string password, in byte minLength, in byte maxLength)
    {
        var hasDigit = false;
        var hasUpperCase = false;
        var hasLowerCase = false;
        var hasSpecialChar = false;
        if (password.Length < minLength || password.Length > maxLength) return false;
        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUpperCase = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (char.IsLower(c)) hasLowerCase = true;
            else if ("!@#$%^&*()_+".Contains(c, StringComparison.Ordinal)) hasSpecialChar = true;
        }
        var charTypesCount = (hasUpperCase ? 1 : 0) + (hasLowerCase ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSpecialChar ? 1 : 0);
        return charTypesCount >= 3;
    }
    public string RootFilePath => FileLayout.GetFullFilePath(FileLayout.RetentionFolderLocation, typeof(EmployerProfile).Name.ToMD5(), FileExtension.Log);
    public required IDurableSetup DurableSetup { get; init; }
}