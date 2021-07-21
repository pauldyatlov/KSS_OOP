using Unity.Entities;

namespace Scripts
{
    [GenerateAuthoringComponent]
    internal struct UbisoftHealth : IComponentData
    {
        public float Value;
    }
}