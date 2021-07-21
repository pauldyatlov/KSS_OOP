using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Scripts
{
    internal sealed class UbisoftLookAtPlayerSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var playerPosition = new float3(Settings.PlayerPosition);

            return Entities.WithAll<UbisoftEnemyTag>().ForEach((ref Translation translation, ref Rotation rotation) =>
            {
                var result = playerPosition - translation.Value;
                result.y = 0;

                rotation.Value = quaternion.LookRotation(result, Vector3.up);
            }).Schedule(inputDeps);
        }
    }
}