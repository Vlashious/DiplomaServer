using Leopotam.EcsLite;

namespace DiplomaServer.World;

public sealed class MainWorld : IAsyncDisposable
{
    public readonly EcsWorld World;
    public readonly EcsSystems Systems;

    public MainWorld()
    {
        World = new EcsWorld();
        Systems = new EcsSystems(World);

        Systems
           .Init();
    }

    public void Update()
    {
        Systems.Run();
    }

    public async ValueTask DisposeAsync()
    {
        Systems.Destroy();
        World.Destroy();
    }
}