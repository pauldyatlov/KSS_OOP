using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Scripts
{
    internal sealed class UbisoftMoveSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;

            return Entities.WithAll<UbisoftMoveForwardTag>().ForEach(
                    (ref Translation translation, ref Rotation rotation, in UbisoftMoveSpeed moveSpeed) =>
                    {
                        translation.Value += moveSpeed.Value * deltaTime * math.forward(rotation.Value);
                    })
                .Schedule(inputDeps);
        }
    }
}