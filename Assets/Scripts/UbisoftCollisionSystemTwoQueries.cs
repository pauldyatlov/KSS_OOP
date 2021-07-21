using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Scripts
{
    internal sealed class UbisoftCollisionSystemTwoQueries : JobComponentSystem
    {
        [BurstCompile]
        private struct CollisionJob : IJobForEach<UbisoftHealth, Translation>
        {
            private readonly float _radius;
            [DeallocateOnJobCompletion] [ReadOnly] private readonly NativeArray<Translation> _possibleCollisions;

            public CollisionJob(float radius, NativeArray<Translation> possibleCollisions)
            {
                _radius = radius;
                _possibleCollisions = possibleCollisions;
            }

            public void Execute(ref UbisoftHealth health, [ReadOnly] ref Translation translation)
            {
                var damage = 0f;

                foreach (var collisionPosition in _possibleCollisions)
                {
                    if (CheckCollision(translation.Value, collisionPosition.Value, _radius))
                        damage += 1f;
                }

                if (damage > 0)
                    health.Value -= damage;
            }

            private bool CheckCollision(float3 first, float3 second, float radius)
            {
                var delta = first - second;
                var distance = delta.x * delta.x + delta.z * delta.z;

                return distance <= radius;
            }
        }

        private EntityQuery _playerGroup;
        private EntityQuery _enemyGroup;
        private EntityQuery _bulletGroup;

        protected override void OnCreate()
        {
            _playerGroup = GetEntityQuery(typeof(UbisoftHealth),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<UbisoftPlayerTag>());

            _enemyGroup = GetEntityQuery(typeof(UbisoftHealth),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<UbisoftEnemyTag>());

            _bulletGroup = GetEntityQuery(typeof(UbisoftTimeToLive),
                ComponentType.ReadOnly<Translation>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var enemyJob = new CollisionJob(Settings.EnemyCollisionRadius,
                _bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob));
            var handle = enemyJob.Schedule(_enemyGroup, inputDeps);

            if (Settings.IsPlayerDead())
                return handle;

            var playerJob = new CollisionJob(Settings.EnemyCollisionRadius,
                _playerGroup.ToComponentDataArray<Translation>(Allocator.TempJob));
            return playerJob.Schedule(_playerGroup, handle);
        }
    }
}