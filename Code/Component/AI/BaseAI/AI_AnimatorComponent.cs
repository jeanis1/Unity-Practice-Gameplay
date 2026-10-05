using UnityEngine;

namespace Code.Component.AI
{
    public interface IAI_Animator
    {
        void Play(string animationName);
    }
    
    public class AI_AnimatorComponent : MonoBehaviour, IAI_Animator
    {
        [SerializeField] private Animator animator;
        private string currentAnimation = "";

        public void Play(string animationName)
        {
            if (currentAnimation == animationName ) return;
            currentAnimation = animationName;
            if (gameObject.activeInHierarchy == false) return;
            animator.CrossFade(animationName, 0.1f, 0,0f);
        }
    }
}