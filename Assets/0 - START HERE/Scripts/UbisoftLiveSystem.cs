using Unity.Entities;
using Unity.Jobs;

namespace Scripts
{
    public class UbisoftLiveSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _bufferSystem;

        protected override void OnCreate()
        {
            _bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var buffer = _bufferSystem.CreateCommandBuffer();
            var deltaTime = Time.DeltaTime;

            var handler = Entities.WithAll<UbisoftTimeToLive>().ForEach((Entity entity, ref TimeToLive timeToLive) =>
            {
                timeToLive.Value -= deltaTime;

                if (timeToLive.Value <= 0)
                    buffer.DestroyEntity(entity);
            }).Schedule(inputDeps);

            _bufferSystem.AddJobHandleForProducer(handler);
            return handler;
        }
    }
}