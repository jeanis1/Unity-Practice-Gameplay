using UnityEngine;
using System.Collections;

namespace Code.Component.Items.Effects
{
    [CreateAssetMenu(fileName = "Damage Potion", menuName = "Inventory/Items/Damage Potion")]
    public class DamageItemEffect : Item
    {
        [SerializeField] private float buffAmount = 20f;
        private float buffDuration = 10f;
        
        
        public override void Use(GameObject user)
        {
            var status = user.GetComponent<CharacterStatusComponent>();
            if(status != null) status.AddStrength(buffAmount);

            var coroutineRunner = user.GetComponent<MonoBehaviour>();
            if (coroutineRunner != null)
            {
                coroutineRunner.StartCoroutine(RestoreOriginalStrength(user));
            }
        }
        
        private IEnumerator RestoreOriginalStrength(GameObject user)
        {
            yield return new WaitForSeconds(buffDuration);
            user.GetComponent<CharacterStatusComponent>().SubtractStrength(buffAmount);
        }
    }
}
