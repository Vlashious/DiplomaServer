using System.Collections.Concurrent;
using DiplomaServer.World.Commands;
using Leopotam.EcsLite;

namespace DiplomaServer.World;

public sealed class MainWorld : IAsyncDisposable
{
    public readonly EcsWorld World;
    public readonly EcsSystems Systems;
    public readonly ConcurrentQueue<ICommand> Commands = new();

    public MainWorld()
    {
        World = new EcsWorld();
        Systems = new EcsSystems(World);

        Systems
           .Init();
    }

    public void Start()
    {
        Task.Run(Update);
    }

    private async Task Update()
    {
        while (true)
        {
            await Task.Delay(40);
            Systems.Run();

            while (Commands.TryDequeue(out var command))
            {
                await command.Execute();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        Systems.Destroy();
        World.Destroy();
    }
}