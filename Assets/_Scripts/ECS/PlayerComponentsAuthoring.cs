using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Game.ECS
{
    public class PlayerComponentsAuthoring : MonoBehaviour
    {
        internal Unity.Collections.NativeArray<ComponentType> GetArchetypeTypes()
        {
            return new Unity.Collections.NativeArray<ComponentType>(new ComponentType[1] { typeof(PlayerStarAchievement) }, Unity.Collections.Allocator.Persistent);
        }
        internal PlayerStarAchievement GetStarAchievementAuthoringData()
        {
            var data = new PlayerStarAchievement
            {
                CollectedStarsAmount = 0,
            };
            return data;
        }
    }
}
