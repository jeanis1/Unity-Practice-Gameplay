using System.Collections;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class LoseScreenUI : MonoBehaviour, IUIController
    {
        public UIType Type => UIType.LoseScreen;
        [SerializeField] private GameObject loseScreen;
        [SerializeField] private TextMeshProUGUI countdownText;
        private float counter = 5f;

        public void Show()
        {
            loseScreen.SetActive(true);
            StartCoroutine(UpdateCountdown());
        }

        public void Hide()
        {
            loseScreen.SetActive(false);
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
