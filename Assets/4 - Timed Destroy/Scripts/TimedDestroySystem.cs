using Unity.Entities;
using Unity.Jobs;

public class TimedDestroySystem : JobComponentSystem
{
	EndSimulationEntityCommandBufferSystem bufferSystem;

	protected override void OnCreate()
	{
		bufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var buffer = bufferSystem.CreateCommandBuffer().ToConcurrent();
		var dt = Time.DeltaTime;

		var handle = Entities.ForEach((Entity entity, int entityInQueryIndex, ref TimeToLive timeToLive) =>
		{
			timeToLive.Value -= dt;
			if (timeToLive.Value <= 0f)
				buffer.DestroyEntity(entityInQueryIndex, entity);
		}).Schedule(inputDeps);
		
		bufferSystem.AddJobHandleForProducer(handle);
		return handle;
	}
}