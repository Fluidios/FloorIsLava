using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS
{
    public class PlayerStarAchievementAuthoring : MonoBehaviour
    {
        public Action StarObtained;
    }

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
