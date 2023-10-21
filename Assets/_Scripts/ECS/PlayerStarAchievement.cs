using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS
{
    public struct PlayerStarAchievement : IComponentData
    {
        public int PlayerGoId;
        public int CollectedStarsAmount;
    }
}
