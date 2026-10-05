using UnityEngine;

namespace Code.Component
{
    public interface IAnimator
    {
        void Play(string animationName);
    }
    public class CharacterAnimatorComponent : MonoBehaviour, IAnimator
    {
        [SerializeField] Animator animator;
        private string currentAnimation = "";

        void Start()
        {
            
        }
        
        public void Play(string animationName)
        {
            if (currentAnimation == animationName) return; 
            currentAnimation = animationName;
            animator.CrossFade(animationName,0.1f,0, 0f);
        }
    }
}