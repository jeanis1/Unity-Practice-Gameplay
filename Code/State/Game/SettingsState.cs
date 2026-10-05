using Code.UI;

namespace Code.Game.GameState
{
    public class SettingsState : IGameState
    {
        private readonly UIManager _ui;
        public GameStates States => GameStates.Settings;
        public SettingsState(UIManager ui) => _ui = ui;
        public void Enter() => _ui.Show(UIType.Settings);
        public void Exit() { }
        public void Update() { }


    }
}
