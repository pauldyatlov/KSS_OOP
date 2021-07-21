using Unity.Entities;
using Unity.Jobs;

public class TestDestroySystem : JobComponentSystem
{
	EndSimulationEntityCommandBufferSystem buffer;

	protected override void OnCreate()
	{
		buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var commands = buffer.CreateCommandBuffer().ToConcurrent();
		var handle = Entities.WithAll<TestDestroy>().ForEach((Entity entity, int entityInQueryIndex) =>
		{
			commands.DestroyEntity(entityInQueryIndex, entity);
		}).Schedule(inputDeps);
		buffer.AddJobHandleForProducer(handle);
		return handle;
	}
}