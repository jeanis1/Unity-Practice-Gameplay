using UnityEngine;
using UnityEngine.UI;
using Code.Game;

namespace Code.UI
{
    public class MainMenuUI : MonoBehaviour, IUIController
    {
        public UIType Type => UIType.MainMenu;
        [SerializeField] private GameManager _gm;
        [SerializeField] private GameObject panel; // assign main menu
        [SerializeField] Button playButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button quitButton;
        
        public void Show()
        {
            panel.SetActive(true);
            playButton.onClick.AddListener(_gm.StartGame);
            settingsButton.onClick.AddListener(_gm.ToSettings);
            quitButton.onClick.AddListener(Application.Quit);
        }
        
        public void Hide()
        {
            panel.SetActive(false);
            playButton.onClick.RemoveListener(_gm.StartGame);
            settingsButton.onClick.RemoveListener(_gm.ToSettings);
            quitButton.onClick.RemoveListener(Application.Quit);
        }

    }
}
