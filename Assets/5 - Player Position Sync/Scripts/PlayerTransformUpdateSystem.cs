using Unity.Entities;
using Unity.Transforms;

public class PlayerTransformUpdateSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		if (Settings.IsPlayerDead())
			return;

		Entities.WithAll<PlayerTag>().ForEach((ref Translation pos) => { pos.Value = Settings.PlayerPosition; });
	}
}