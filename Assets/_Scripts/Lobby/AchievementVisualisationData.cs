using UnityEngine;

namespace Game.Lobby
{
    [CreateAssetMenu(menuName = "Data/AchievementVisualisationData")]
    public class AchievementVisualisationData : ScriptableObject
    {
        [SerializeField] Sprite _icon;
        [SerializeField] string _description;

        public Sprite Icon { get => _icon; }
        public string Description { get => _description; }
    }
}
