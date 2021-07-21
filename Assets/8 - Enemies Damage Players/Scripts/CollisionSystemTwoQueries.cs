using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SceneManagement;

public class CollisionSystemTwoQueries : JobComponentSystem
{
    EntityQuery enemyGroup;
    EntityQuery playerGroup;

    protected override void OnCreate()
    {
        Enabled = SceneManager.GetActiveScene().name.Contains("8"); //Dirty Hack for simplifying this samples 
        
        playerGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PlayerTag>());
        enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<EnemyTag>());
    }

    [BurstCompile]
    struct CollisionJob : IJobForEach<Health, Translation>
    {
        public float radius;
        
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> transToTestAgainst;

        public void Execute(ref Health health, [ReadOnly] ref Translation pos)
        {
            float damage = 0f;
            for (int j = 0; j < transToTestAgainst.Length; j++)
            {
                Translation pos2 = transToTestAgainst[j];

                if (CheckCollision(pos.Value, pos2.Value, radius))
                {
                    damage += 1;
                }
            }

            if (damage > 0)
            {
                health.Value -= damage;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var jobEvB = new CollisionJob();
        jobEvB.radius = Settings.EnemyCollisionRadius;
        jobEvB.transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob);
        return jobEvB.Schedule(playerGroup, inputDependencies);
    }
	
    static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
    {
        float3 delta = posA - posB;
        float distanceSquare = delta.x * delta.x + delta.z * delta.z;
        return distanceSquare <= radiusSqr;
    }
}