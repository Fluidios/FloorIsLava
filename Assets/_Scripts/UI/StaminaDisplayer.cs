using Game.Player;
using Game.Systems;
using Game.SystemsManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    class StaminaDisplayer :MonoBehaviour
    {
        [SerializeField] private Image _staminaDisplayerFillImage;
        private Stamina _observedStamina;
        private bool _valid;

        private void Awake()
        {
            SystemsManager.GetSystemOfType<CameraSystem>().TargetSettled += TrySubscribeToCameraTarget;
        }

        private void TrySubscribeToCameraTarget(Transform cameraTarget)
        {
            if(cameraTarget.TryGetComponent(out _observedStamina))
            {
                _valid = true;
                _staminaDisplayerFillImage.fillAmount = _observedStamina.Normalized;
            }
            else
            {
                _valid = false;
            }
        }

        private void Update()
        {
            if(_valid)
            {
                _staminaDisplayerFillImage.fillAmount = _observedStamina.Normalized;
            }
        }
    }
}
