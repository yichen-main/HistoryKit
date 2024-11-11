namespace Eywa.Core.Architects.Primaries.Protections;
public static class FeatureExpand
{
    public static int Minus(this int value, in int number = 1) => value - number;
    public static string ToBinary(this int value, in byte bits = 16) => Convert.ToString(value, 2).PadLeft(bits, '0');
    public static string ToBinary(this byte value, in byte bits = 8) => Convert.ToString(value, 2).PadLeft(bits, '0');
    public static double ToDecimalPlaces(this int value, in int offset = 0) => value / offset.GetIntegerDigits();
    public static double ToDecimalPlaces(this double value, in int offset = 0) => value / offset.GetIntegerDigits();
    public static double TakeDecimal(this double value, in int precision = 2) => Math.Round(value, precision, MidpointRounding.AwayFromZero);
    public static bool TryDouble(this string? str, out double result) => double.TryParse(str, CultureInfo.InvariantCulture, out result);
    public static bool TryDouble(this object obj, out double result)
    {
        switch (obj)
        {
            case double value:
                result = value;
                return true;

            case string str:
                return str.TryDouble(out result);

            case IConvertible convertible:
                try
                {
                    result = convertible.ToDouble(provider: null);
                    return true;
                }
                catch (InvalidCastException)
                {
                    result = 0.0;
                    return false;
                }

            default:
                result = 0.0;
                return false;
        }
    }
    static double GetIntegerDigits(this int value) => Convert.ToDouble("1".PadRight(value + 1, '0'), CultureInfo.InvariantCulture);
    public static bool[]? WithBinaryToBooleanArray(this string value, bool reverse = default)
    {
        var binaries = value.ToCharArray();
        var binaryLength = binaries.Length;
        var results = new bool[binaryLength];
        if (reverse) Array.Reverse(binaries);
        for (int i = default; i < binaryLength; i++) results[i] = binaries[i] is '1';
        return results;
    }
    public static bool VerifyPort(this IPAddress ip, in int port)
    {
        try
        {
            using TcpClient client = new(ip.ToString(), port);
            return client.Connected;
        }
        catch (Exception)
        {
            return default;
        }
    }
    public static IEnumerable<string> GetLocalIPv4(this NetworkInterfaceType networkInterfaceType)
    {
        List<string> results = [];
        Array.ForEach(NetworkInterface.GetAllNetworkInterfaces(), item =>
        {
            if (item.NetworkInterfaceType == networkInterfaceType && item.OperationalStatus is OperationalStatus.Up)
            {
                results.AddRange(item.GetIPProperties().UnicastAddresses.Where(info =>
                info.Address.AddressFamily is AddressFamily.InterNetwork).Select(info => info.Address.ToString()));
            }
        });
        return results;
    }
    public static string Merge(this string[] args)
    {
        var length = args.Length;
        System.Runtime.CompilerServices.DefaultInterpolatedStringHandler handler = new(default, length);
        for (int item = default; item < length; item++) handler.AppendFormatted(args[item]);
        return handler.ToStringAndClear();
    }
    public static bool StartCharsetExists(this string content, in string flag)
    {
        var length = flag.Length;
        if (content.Length >= length && flag.IsMatch(content[..length])) return true;
        return default;
    }
    public static string ToUrlAddress(this string path, in int port)
    {
        if (!string.IsNullOrEmpty(path) && !path.StartsWith('/')) path = $"/{path}";
        return string.Create(CultureInfo.InvariantCulture, $"http://localhost:{port}{path}");
    }
    public static async ValueTask PrintAsync(this string content, ConsoleColor color = ConsoleColor.White, bool newline = true)
    {
        Console.ForegroundColor = color;
        if (newline)
        {
            await Console.Out.WriteLineAsync(content).ConfigureAwait(false);
        }
        else await Console.Out.WriteAsync(content).ConfigureAwait(false);
    }
    public static bool IsDefault(this string? input) => input == default;
    public static bool IsNotDefault(this string? input) => input != default;
    public static bool IsTrue(this string? input) => input.IsMatch("1");
    public static bool IsFalse(this string? input) => input.IsMatch("0");
    public static bool IsFuzzy(this string input, in string pattern, in StringComparison comparison = StringComparison.Ordinal) => input.Contains(pattern, comparison);
    public static bool IsFuzzy(this string input, StringComparison comparison = StringComparison.Ordinal, params string[] patterns) => patterns.Any(pattern => input.IsFuzzy(pattern, comparison));
    public static bool IsMatch(this string? str1, in string? str2, in StringComparison type = StringComparison.Ordinal) => string.Equals(str1, str2, type);
    public static string FirstCharLowercase(this string content) => char.ToLower(content[default], CultureInfo.CurrentCulture) + content[1..];
}