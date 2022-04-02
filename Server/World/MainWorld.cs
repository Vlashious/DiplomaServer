using System.Collections.Concurrent;
using System.Diagnostics;
using DiplomaServer.World.Commands;
using DiplomaServer.World.Systems;
using Leopotam.EcsLite;
using Microsoft.AspNetCore.SignalR;

namespace DiplomaServer.World;

public sealed class MainWorld : IAsyncDisposable
{
    public static float Delta;
    public const float TargetDelta = 1f / 60;
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
           .Add(new DelayedDamageSystem())
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
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                await Task.Delay(TimeSpan.FromSeconds(TargetDelta));

                if (!IsTicking)
                {
                    continue;
                }

                Systems.Run();

                while (Commands.TryDequeue(out var command))
                {
                    await command.Execute();
                }

                stopwatch.Stop();
                Delta = (float) stopwatch.Elapsed.TotalSeconds;
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