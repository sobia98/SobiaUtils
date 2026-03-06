using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sobia.Utils
{
    public class CopyJoinCodeUI : MonoBehaviour
    {
        [SerializeField] private Button CopyButton;
        [SerializeField] private TMP_Text CopyButtonText;

        private void Awake()
        {
            SobiaUtils.IsAssigned(CopyButton, nameof(CopyButton), gameObject);
            SobiaUtils.IsAssigned(CopyButtonText, nameof(CopyButtonText), gameObject);
        }

        public void CopyJoinCodeToClipboard()
        {
            if (string.IsNullOrEmpty(NetworkLobby.CurrentJoinCode))
            {
                Debug.LogWarning("No join code generated yet to copy.");
                return;
            }

            GUIUtility.systemCopyBuffer = NetworkLobby.CurrentJoinCode;
            StartCoroutine(CopyFeedbackRoutine());
        }

        private IEnumerator CopyFeedbackRoutine()
        {
            if (CopyButtonText == null) yield break;
            string originalText = CopyButtonText.text;

            if (CopyButton != null) CopyButton.interactable = false;

            CopyButtonText.text = GlobalConstants.COPIED;

            yield return new WaitForSeconds(2f);

            CopyButtonText.text = originalText;
            if (CopyButton != null) CopyButton.interactable = true;
        }
    }
}