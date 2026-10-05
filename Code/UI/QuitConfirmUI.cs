using Code.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class QuitConfirmUI : MonoBehaviour, IUIController
    {
        public UIType Type => UIType.QuitConfirm;
        [SerializeField] private GameManager _gm;
        [SerializeField] private GameObject panel;
        [SerializeField] private UIManager _ui;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        public void Show()
        {
            panel.SetActive(true);
            yesButton.onClick.AddListener(Application.Quit);
            noButton.onClick.AddListener(HideQuitMenu);
        }

        public void Hide()
        {
            panel.SetActive(false);
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();
        }

        private void HideQuitMenu()
        {
            _ui.GoBack();
            _ui.Show(UIType.HUD);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
