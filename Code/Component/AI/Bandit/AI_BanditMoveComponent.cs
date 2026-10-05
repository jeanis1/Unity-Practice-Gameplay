using UnityEngine;

namespace Code.Component.AI.Bandit
{
    public class IaiBanditMoveComponent : IAIMoveComponent
    {
        [SerializeField] private Transform questActivationPoint;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

    }
}

