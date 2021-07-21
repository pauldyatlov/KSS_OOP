using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SceneManagement;

public class CollisionSystem : JobComponentSystem
{
	EntityQuery enemyGroup;

	protected override void OnCreate()
	{
		Enabled = SceneManager.GetActiveScene().name.Contains("7"); //Dirty Hack for simplifying this samples 
		
		enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<EnemyTag>());
	}

	[BurstCompile]
	struct CollisionJob : IJobForEach<Health, Translation>
	{
		public float radius;
		public float3 playerPosition;

		public void Execute(ref Health health, [ReadOnly] ref Translation pos)
		{
			if(CheckCollision(pos.Value, playerPosition, radius))
				health.Value -= 1;
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDependencies)
	{
		var jobEvP = new CollisionJob();
		jobEvP.radius = Settings.EnemyCollisionRadius;
		jobEvP.playerPosition = Settings.PlayerPosition;
		return jobEvP.Schedule(enemyGroup, inputDependencies);
	}
	
	static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
	{
		float3 delta = posA - posB;
		float distanceSquare = delta.x * delta.x + delta.z * delta.z;
		return distanceSquare <= radiusSqr;
	}
}