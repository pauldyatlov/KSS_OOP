using Unity.Entities;

namespace Scripts
{
    [GenerateAuthoringComponent]
    public struct UbisoftMoveSpeed : IComponentData
    {
        public float Value;
    }
}