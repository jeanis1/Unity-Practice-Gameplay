

namespace Code.State
{
    //State Base Class
    public interface IState
    {
        void Enter();
        void Execute();
        void Exit();
    }

    //Manage State
    public class BaseStateMachine
    {
        private IState currentState;

        public void ChangeState(IState newState)
        {
            if (currentState != null) currentState.Exit();
            currentState = newState;
            if (currentState != null) currentState.Enter();
        }

        public void Update()
        {
            currentState?.Execute();
        }
    }
}