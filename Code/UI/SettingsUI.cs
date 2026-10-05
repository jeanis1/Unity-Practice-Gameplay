using UnityEngine;

namespace Code.UI
{
    public class SettingsUI : MonoBehaviour, IUIController
    {
        public UIType Type => UIType.Settings;
        [SerializeField] private GameObject panel; //assign Settings panel here...

        public void Show()
        {
            panel.SetActive(true);
            //load current settings into UI Fields...
        }

        public void Hide()
        {
            panel.SetActive(false);
            //save or reset settings as needed...
        }
    }
}
