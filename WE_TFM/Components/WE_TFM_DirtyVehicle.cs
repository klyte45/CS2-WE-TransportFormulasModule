using Unity.Entities;

namespace WE_TFM.Components
{
    public struct WE_TFM_DirtyVehicle : IComponentData
    {
        public Entity oldTarget;
    }
}
