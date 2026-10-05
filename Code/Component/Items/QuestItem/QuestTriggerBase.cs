using Code.Game;
using UnityEngine;


/*
 * -onTrigger Collision -  Activate Sound Effect
 * -Call Quest Trigger in GameManager
 * -Set Parent Inactive/hidden
 */


namespace Code.Component.Items.QuestItem
{
    public class QuestTriggerBase : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _questTriggerSound;
        [SerializeField] private GameManager _gameManager;
        private GameObject _parentObject;


        void Awake()
        {
            if (!_audioSource || !_questTriggerSound)
            {
                Debug.LogError("Quest Trigger Pickup needs variables set!");
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            _audioSource.PlayOneShot(_questTriggerSound);
            _gameManager.ActivateQuest();
            gameObject.SetActive(false);
        }
    }
}
