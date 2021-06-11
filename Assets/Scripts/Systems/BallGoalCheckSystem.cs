using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class BallGoalCheckSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithAll<BallTag>()
            .WithoutBurst()
            .ForEach((Entity e, in Translation t) =>
            {
                float3 pos = t.Value;
                float bound = GameManager.main.xBound;

                if (pos.x >= bound)
                {
                    GameManager.main.PlayerScored(0);
                    ecb.DestroyEntity(e);
                } 
                else if (pos.x <= -bound)
                {
                    GameManager.main.PlayerScored(1);
                    ecb.DestroyEntity(e);
                }
            })
            .Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        return default;
    }
}
