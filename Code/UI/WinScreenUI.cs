using System.Collections;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class WinScreenUI : MonoBehaviour, IUIController
    {
        public UIType Type => UIType.WinScreen;
        [SerializeField] private GameObject winScreen;
        [SerializeField] private TextMeshProUGUI countdownText;
        private float counter = 5f;
        
        
        public void Show()
        {
            winScreen.SetActive(true);
            StartCoroutine(UpdateCountdown());
        }

        public void Hide()
        {
            winScreen.SetActive(false);
            counter = 5f; // reset counter after use
        }

        private IEnumerator UpdateCountdown()
        {
            while (counter > 0)
            {
                countdownText.text = "New Game in " + Mathf.RoundToInt(counter);
                counter--;
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
