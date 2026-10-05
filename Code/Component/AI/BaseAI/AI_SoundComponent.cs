using UnityEngine;

namespace Code.Component.AI
{
    public interface IAI_Sound
    {
        void PlaySound(string soundName);
    }
    public class AI_SoundComponent : MonoBehaviour, IAI_Sound
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip OnHitSound;
        [SerializeField] private AudioClip OnDeathSound;
        [SerializeField] private AudioClip HealthRestoreSound;
        [SerializeField] private AudioClip SpeedBuffSound;
        [SerializeField] private AudioClip Attack1Sound;
        private void Awake()
        {
            if(audioSource == null) Debug.LogError("AI Audio Source missing");
        }

        public void PlaySound(string soundName)
        {
            switch (soundName)
            {
                case "OnHit":
                    audioSource.PlayOneShot(OnHitSound);
                    break;
                case "Death":
                    audioSource.PlayOneShot(OnDeathSound);
                    break;
                case "HealthRestore":
                    audioSource.PlayOneShot(OnHitSound);
                    break;
                case "SpeedBuff":
                    audioSource.PlayOneShot(SpeedBuffSound,2f);
                    break;
                case "Attack1":
                    audioSource.PlayOneShot(Attack1Sound);
                    break;
            }
        }
    }
}