using System.Collections.Generic;


namespace Code.Game.GameState
{
    public enum GameStates
    {
        MainMenu,
        Settings,
        QuitConfirm, //could be removed and instead a simple UI event toggle
        InGame,
        PlayerWin,
        PlayerLose,
        Restart
    }

    public interface IGameState
    {
        GameStates States { get; }
        void Enter();
        void Exit();
        void Update(); 
    }

    public class GameStateManager
    {
        private readonly Dictionary<GameStates, IGameState> _states;
        IGameState _current;

        public GameStateManager(IEnumerable<IGameState> states)
        {
            _states = new Dictionary<GameStates, IGameState>();
            foreach (var s in states) _states[s.States] = s;
        }

        public void ChangeState(GameStates next)
        {
            if (_current != null) _current.Exit();
            _current = _states[next];
            _current.Enter();
        }

        public void Update()
        {
            _current?.Update();
        }
}
}

