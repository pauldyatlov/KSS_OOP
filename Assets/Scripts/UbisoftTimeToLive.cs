using Unity.Entities;

namespace Scripts
{
    [GenerateAuthoringComponent]
    public struct UbisoftTimeToLive : IComponentData
    {
        public float Value;
    }
}