using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FirebaseHandler
{
    public static class Achievements
    {
        public static AchivementsMask GetEmptyPlayerAchievementsMask()
        {
            var mask = new Dictionary<string, float>();

            mask.Add("first-victory", 0);
            mask.Add("brawler", 0);
            mask.Add("super-star", 0);

            return new AchivementsMask(mask);
        }

        [System.Serializable]
        public class AchivementsMask
        {
            public Dictionary<string, float> Mask;

            public AchivementsMask()
            {
                Mask = new Dictionary<string, float>();
            }
            public AchivementsMask(Dictionary<string, float> mask)
            {
                Mask = mask;
            }
        }
    }
}
