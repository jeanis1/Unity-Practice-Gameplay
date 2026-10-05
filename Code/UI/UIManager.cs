using UnityEngine;
using System.Linq;
using System.Collections.Generic;


namespace Code.UI
{
    public enum UIType
    {
        None, //Zero UI
        MainMenu,
        HUD, //In Game HUD
        Settings,
        QuitConfirm,
        WinScreen,
        LoseScreen,
    }
    
    public interface IUIController
    {
        UIType Type { get; }
        void Show();
        void Hide();
    }

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> controllersMono;
        private Dictionary<UIType, IUIController> lookup;
        private Stack<UIType> history = new Stack<UIType>();

        void Awake()
        {
            //build lookup from injected controllers
            lookup = controllersMono
                .OfType<IUIController>()
                .ToDictionary(c => c.Type, c => c);
        }

        void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    public void Show(UIType type)
        {
            if (history.Count > 0)
            {
                lookup[history.Peek()].Hide(); //hide current panel
            }
            if (!lookup.ContainsKey(type))
            {
                // Debug.LogError($"[UIManager] No controller registered for {type}");
                return;
            }
            lookup[type].Show();
            history.Push(type);
        }

        public void GoBack()
        {
            if (history.Count <= 1) return;
            var current = history.Pop();
            lookup[current].Hide();
            lookup[history.Peek()].Show();
        }
    }
}
