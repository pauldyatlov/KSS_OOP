using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Scripts
{
    public class UbisoftPlayerMoveSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Settings.IsPlayerDead())
                return inputDeps;

            Vector3 playerPosition = Settings.PlayerPosition;

            return Entities.WithAll<UbisoftPlayerTag>().ForEach((ref Translation translation) =>
                {
                    translation.Value = playerPosition;
                })
                .Schedule(inputDeps);
        }
    }
}