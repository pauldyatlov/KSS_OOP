using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TurnTowardsPlayerSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		if (Settings.IsPlayerDead())
			return inputDeps;

		var playerPosition = (float3)Settings.PlayerPosition;
		
		return Entities.WithAll<EnemyTag>().ForEach((ref Rotation rot, in Translation pos) =>
		{
			float3 heading = playerPosition - pos.Value;
			heading.y = 0f;
			rot.Value = quaternion.LookRotation(heading, math.up());
		}).Schedule(inputDeps);
	}
}