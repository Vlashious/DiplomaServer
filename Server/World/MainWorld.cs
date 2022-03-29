using System.Collections.Concurrent;
using DiplomaServer.World.Commands;
using DiplomaServer.World.Systems;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.World;

public sealed class MainWorld : IAsyncDisposable
{
    public const float Delta = 0.1f;
    public readonly EcsWorld World;
    public readonly EcsSystems Systems;
    public readonly ConcurrentQueue<ICommand> Commands = new();
    public Func<Func<Hub, Task>, Task> NeedSendData;
    public bool IsTicking = true;

    public MainWorld()
    {
        World = new EcsWorld();
        Systems = new EcsSystems(World);

        Systems
           .Add(new SpawnWhaleSystem())
           .Add(new ProjectileSystem())
           .Add(new DamageEventSystem(this))
           .Init();
    }

    public async Task Start()
    {
        await Task.Run(Update);
    }

    private async Task Update()
    {
        while (true)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Delta));

                if (!IsTicking)
                {
                    continue;
                }

                Systems.Run();

                while (Commands.TryDequeue(out var command))
                {
                    await command.Execute();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        Systems.Destroy();
        World.Destroy();
    }
}