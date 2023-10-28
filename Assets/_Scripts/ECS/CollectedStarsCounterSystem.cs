using Unity.Entities;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Game.ECS
{
    public partial class CollectedStarsCounterSystem : SystemBase
    {
        SystemHandle m_EndSimulationECBSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            // This is how we make changes to the EntityManager within a job
            m_EndSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            Entities.WithAll<PlayerStarAchievement>().ForEach((ref PlayerStarAchievement achievement, in Entity entity) =>
            {
                Debug.Log(string.Format("Entity({0}): !!!", entity.Index));
            }).ScheduleParallel();
        }
    }
}
