using Unity.Entities;
using UnityEngine;

namespace Game.ECS
{
    class PlayerStarAchievementBaker : Baker<PlayerStarAchievementAuthoring>
    {
        public override void Bake(PlayerStarAchievementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerStarAchievement
            {
                PlayerGoId = authoring.gameObject.GetInstanceID(),
                CollectedStarsAmount = 0,
            });
            Debug.LogFormat("{0} star achievement has been baked", authoring.gameObject.name);
        }
    }

}
