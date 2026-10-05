using UnityEngine;
using System.Collections;

namespace Code.Component.Items.Effects
{
    [CreateAssetMenu(fileName = "New Speed Potion", menuName = "Item Effects/Speed")]
    public class SpeedItemEffect : Item
    {
        [SerializeField] private float speedBuff = 5f;
        private float speedBuffDuration = 3f;

        public override void Use(GameObject user)
        {
            var move = user.GetComponent<CharacterMoveComponent>();
            var sound = user.GetComponent<CharacterSoundComponent>();
            if (move != null) move.BuffSpeed(speedBuff);
            if (sound != null) sound.PlaySound("SpeedBuffSound");

            var coroutineRunner = user.GetComponent<MonoBehaviour>();
            if (coroutineRunner != null)
            {
                coroutineRunner.StartCoroutine(RestoreOriginalSpeed(user));
            }
        }
                
        private IEnumerator RestoreOriginalSpeed(GameObject targetObject)
        {
            yield return new WaitForSeconds(speedBuffDuration);
            targetObject.GetComponent<CharacterMoveComponent>()?.RestoreOriginalSpeed();
            Debug.Log("RestoreOriginalSpeed called");
        }
    }
}
