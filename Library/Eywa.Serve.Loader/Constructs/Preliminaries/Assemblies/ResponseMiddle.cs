using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Eywa.Serve.Loader.Constructs.Preliminaries.Assemblies;
internal sealed class ResponseMiddle(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(HeaderName.AccessControlAllowOrigin, "*");
            context.Response.Headers.Append(HeaderName.AccessControlAllowHeaders, "*");
            context.Response.Headers.Append(HeaderName.AccessControlAllowMethods, "*");
            context.Response.Headers.Append(HeaderName.AccessControlAllowCredentials, "true");
            return Task.CompletedTask;
        });

        var stopwatch = Stopwatch.StartNew();
        var originalBody = context.Response.Body;
        using MemoryStream responseBody = new();
        context.Response.Body = responseBody;
        await next(context).ConfigureAwait(false);
        stopwatch.Stop();

        context.Response.Body.Seek(default, SeekOrigin.Begin);
        var requestBodyTextAsync = new StreamReader(context.Request.Body).ReadToEndAsync(context.RequestAborted);
        var requestText = await requestBodyTextAsync.ConfigureAwait(false);
        if (!string.IsNullOrEmpty(requestText))
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(requestText));
            if (JsonDocument.TryParseValue(ref reader, out JsonDocument? jsonDocument))
            {
                requestText = JsonSerializer.Serialize(jsonDocument.RootElement, FormatLayout.SerialOption(indented: true));
            }
        }
        var responseTextAsync = new StreamReader(context.Response.Body).ReadToEndAsync(context.RequestAborted);
        var responseText = await responseTextAsync.ConfigureAwait(false);
        if (!string.IsNullOrEmpty(responseText))
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(responseText));
            if (JsonDocument.TryParseValue(ref reader, out JsonDocument? jsonDocument))
            {
                responseText = JsonSerializer.Serialize(jsonDocument.RootElement, FormatLayout.SerialOption(indented: true));
            }
        }
        context.Response.Body.Seek(default, SeekOrigin.Begin);

        var messageTemplate = "[{@method}] {@path} in {elapsed} ms. {@body}";
        Log.Debug(messageTemplate, context.Request.Method, context.Request.Path.ToString(), stopwatch.ElapsedMilliseconds, new
        {
            query = context.Request.QueryString.ToString(),
            request = requestText,
            response = responseText,
        });
        await responseBody.CopyToAsync(originalBody, context.RequestAborted).ConfigureAwait(false);
    }
}