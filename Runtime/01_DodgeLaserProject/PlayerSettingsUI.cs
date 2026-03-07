using Sobia.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Sobia.LaserProject
{
    public class PlayerSettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider SpeedSlider;
        [SerializeField] private TMP_Text SpeedText;
        [SerializeField] private Slider JumpSlider;
        [SerializeField] private TMP_Text JumpText;

        private void Start()
        {
            SobiaUtils.IsAssigned(SpeedSlider, nameof(SpeedSlider), gameObject);
            SobiaUtils.IsAssigned(SpeedText, nameof(SpeedText), gameObject);
            SobiaUtils.IsAssigned(JumpSlider, nameof(JumpSlider), gameObject);
            SobiaUtils.IsAssigned(JumpText, nameof(JumpText), gameObject);

            SpeedSlider.minValue = 1f;
            SpeedSlider.maxValue = 25f;
            JumpSlider.minValue = 1f;
            JumpSlider.maxValue = 15f;

            if (!NetworkManager.Singleton.IsServer)
            {
                SpeedSlider.interactable = false;
                JumpSlider.interactable = false;
            }

            SpeedSlider.value = GameManager.Instance.PlayerMovementSettings.Value.Speed;
            JumpSlider.value = GameManager.Instance.PlayerMovementSettings.Value.JumpHeight;

            SpeedSlider.onValueChanged.AddListener(UpdateSpeedDisplayAndRequest);
            JumpSlider.onValueChanged.AddListener(UpdateJumpDisplayAndRequest);

            UpdateJumpDisplay(JumpSlider.value);
            UpdateSpeedDisplay(SpeedSlider.value);
        }

        private void UpdateSpeedDisplayAndRequest(float newSpeed)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                GameManager.Instance.SetMovementSettingsServerRpc(newSpeed);
            }

            UpdateSpeedDisplay(newSpeed);
        }

        private void UpdateJumpDisplayAndRequest(float newJumpHeight)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                GameManager.Instance.SetJumpSettingsServerRpc(newJumpHeight);
            }
            UpdateJumpDisplay(newJumpHeight);
        }

        private void UpdateSpeedDisplay(float newSpeed)
        {
            if (SpeedText != null) SpeedText.text = $"Speed: {newSpeed:0.0} m/s";
        }

        private void UpdateJumpDisplay(float newJump)
        {
            if (JumpText != null) JumpText.text = $"Jump: {newJump:0.0} units";
        }
    }
}