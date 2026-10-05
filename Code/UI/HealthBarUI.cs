using UnityEngine;

namespace Code.UI
{
    public class HealthBarUI : MonoBehaviour, IUIController
    {
        public UIType Type => UIType.HUD;
        [SerializeField] private GameObject healthbar;

        public void Show()
        {
            healthbar.SetActive(true);
        }

        public void Hide()
        {
            healthbar.SetActive(false);
        }
    }
}
