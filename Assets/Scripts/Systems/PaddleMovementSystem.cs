using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class PaddleMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float dTime = Time.DeltaTime;
        float yBound = GameManager.main.yBound;

        Entities.ForEach((ref Translation trans, in PaddleMovementData moveData) =>
        {
            trans.Value.y = math.clamp(trans.Value.y + (moveData.direction * moveData.speed * dTime), -yBound, yBound);
        }).Run();

        return default;
    }
}
