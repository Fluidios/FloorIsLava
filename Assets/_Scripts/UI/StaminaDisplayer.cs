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
        private PlayerController _observedPlayerController;
        private Stamina _observedStamina;
        private bool _valid;

        private void Awake()
        {
            SystemsManager.GetSystemOfType<CameraSystem>().TargetSettled += TrySubscribeToCameraTarget;
        }

        private void TrySubscribeToCameraTarget(Transform cameraTarget)
        {
            if(cameraTarget.TryGetComponent(out _observedPlayerController))
            {
                _valid = true;
                _observedStamina = _observedPlayerController.Stamina;
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
                if(_observedPlayerController.IsDead) { _valid = false;  return; }
                _staminaDisplayerFillImage.fillAmount = _observedStamina.Normalized;
            }
        }
    }
}
