using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class PlayerResultUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nickname;
        [SerializeField] private TextMeshProUGUI _stars;
        public void Set(string playerNickName, int collectedStars)
        {
            _nickname.text = playerNickName;
            _stars.text = collectedStars.ToString();
        }
    }
}
