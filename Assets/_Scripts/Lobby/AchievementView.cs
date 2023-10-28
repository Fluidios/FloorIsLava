using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class AchievementView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _progressFill;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        public void UpdateAchievementView(string name, string description, float progress, Sprite icon)
        {
            _icon.sprite = icon;
            _nameText.text = name;
            _descriptionText.text = description;
            _progressFill.fillAmount = progress;
        }
    }
}
