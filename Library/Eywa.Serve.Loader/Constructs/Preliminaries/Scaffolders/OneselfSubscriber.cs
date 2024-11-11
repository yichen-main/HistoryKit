namespace Eywa.Serve.Loader.Constructs.Preliminaries.Scaffolders;
public class OneselfSubscriber
{
    [Subscribe, Topic(nameof(TestKanban))]
    public IEnumerable<TestKanban> OnTestKanban([EventMessage] IEnumerable<TestKanban> infos)
    {
        return infos;
    }
}