using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FirebaseHandler
{
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
