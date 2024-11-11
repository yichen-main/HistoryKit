namespace Eywa.Serve.Loader.Constructs.Preliminaries.Scaffolders;
public class OneselfMutator
{
    public ValueTask<string> SetMongoDBAsync(string body, [Service] IMediator mediator)
    {
        return new();
    }
}