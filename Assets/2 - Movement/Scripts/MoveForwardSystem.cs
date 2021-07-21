using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveForwardSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var dt = Time.DeltaTime;
		return Entities.WithAll<MoveForward>().ForEach((ref Translation pos, in Rotation rot, in MoveSpeed speed) =>
		{
			pos.Value = pos.Value + dt * speed.Value * math.forward(rot.Value);
		}).Schedule(inputDeps);
	} 
}