namespace Eywa.Serve.Constructs.Grindstones.Protections;
public static class CultureHelper
{
    public static IDisposable Use(string culture)
    {
        var currentCulture = CultureInfo.CurrentCulture;
        var currentUICulture = CultureInfo.CurrentUICulture;
        CultureInfo newCulture = new(culture);
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;
        return new DisposableCulture(currentCulture, currentUICulture);
    }
    private class DisposableCulture(CultureInfo originalCulture, CultureInfo originalUICulture) : IDisposable
    {
        public void Dispose()
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }
}