namespace Eywa.Vehicle.Defender.Informations;
public class LoggingProxy<T> : DispatchProxy
{
    private T _target;

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        var logExceptionAttribute = targetMethod.GetCustomAttribute<LogAttribute>();

        try
        {
            return targetMethod.Invoke(_target, args);
        }
        catch (Exception ex)
        {
            if (logExceptionAttribute != null)
            {
                Console.WriteLine($"Exception caught in method {targetMethod.Name}: {ex.Message}");
            }
            throw;
        }
    }

    public static T Create(T target)
    {
        object proxy = Create<T, LoggingProxy<T>>();
        ((LoggingProxy<T>)proxy).SetParameters(target);
        return (T)proxy;
    }

    private void SetParameters(T target)
    {
        _target = target ?? throw new ArgumentNullException(nameof(target));
    }
}