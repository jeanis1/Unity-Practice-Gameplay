using UnityEngine;

namespace Code.Component
{
    public interface ISound
    {
        void PlaySound(string soundName);
    
    }
    public class CharacterSoundComponent : MonoBehaviour, ISound
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip SwordAttack1Sound;
        [SerializeField] private AudioClip HealthRestoreSound;
        [SerializeField] private AudioClip OnHitSound;
        [SerializeField] private AudioClip DeathSound;
        [SerializeField] private AudioClip SpeedBuffSound;
        [SerializeField] private AudioClip DaggerAttack1Sound;
        [SerializeField] private AudioClip DrawBowSound;
        private void Awake()
        {
            if(audioSource == null) Debug.LogError("Audio source is null");
        }
        
        public void PlaySound(string soundName)
        {
            switch (soundName)
            {
                case "SwordAtk1":
                    audioSource.PlayOneShot(SwordAttack1Sound,3f);
                    break;
                case "DaggerAtk1":
                    audioSource.PlayOneShot(DaggerAttack1Sound, 1f);
                    break;
                case "HealthRestore":
                    audioSource.PlayOneShot(HealthRestoreSound);
                    break;
                case "OnHit":
                    audioSource.PlayOneShot(OnHitSound);
                    break;
                case "Death":
                    audioSource.PlayOneShot(DeathSound);
                    break;
                case "SpeedBuff":
                    audioSource.PlayOneShot(SpeedBuffSound,2f);
                    break;
                case "DrawBow":
                    audioSource.PlayOneShot(DrawBowSound);
                    break;
            }
        }

    }
}