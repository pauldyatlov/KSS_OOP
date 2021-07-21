using Unity.Entities;
using Unity.Transforms;

namespace Scripts
{
    public class RemoveDeadSystem : ComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _bufferSystem;

        protected override void OnCreate()
        {
            _bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var buffer = _bufferSystem.CreateCommandBuffer();

            Entities.WithAll<UbisoftPlayerTag>().ForEach((ref UbisoftHealth health) =>
            {
                if (health.Value <= 0)
                    Settings.PlayerDied();
            });

            Entities.WithAll<UbisoftEnemyTag>()
                .ForEach((Entity entity, ref UbisoftHealth health, ref Translation pos) =>
                {
                    if (health.Value <= 0)
                    {
                        buffer.DestroyEntity(entity);
                        BulletImpactPool.PlayBulletImpact(pos.Value);
                    }
                });
        }
    }
}