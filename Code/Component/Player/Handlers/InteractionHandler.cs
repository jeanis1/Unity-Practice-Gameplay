using Code.Component.AI;
using Code.Component.Items;
using UnityEngine;

namespace Code.Component.Player.Handlers
{
    public class InteractHandler
    {
        private readonly IInteract _interact;
        private readonly CharacterInventoryComponent _inventory;

        public InteractHandler(IInteract interact, CharacterInventoryComponent inventory)
        {
            _interact = interact;
            _inventory = inventory;
        }

        public void TryInteract()
        {
            //Pickup - Check for Pickup Item & Add to Inventory
            if (_interact.HasOverlappedInteractable())
            {
                GameObject interactTarget = _interact.GetOverlappedInteractable();
                if (interactTarget == null)
                {
                    return;
                }

                if (interactTarget.TryGetComponent(out AI_DialogueComponent dialogueTrigger))
                {
                    dialogueTrigger.StartDialogue();
                    return;
                }

                if (interactTarget.TryGetComponent(out PickupItem pickupItem))
                {
                    HandlePickupItem(pickupItem, interactTarget);
                    return;
                }
            }
        }
        
        private void HandlePickupItem(PickupItem pickupItem, GameObject interactTarget)
        {
            if (pickupItem.item == null || _inventory == null)
            {
                return;
            }

            //Create instance to avoid shared stack count between pickup item
            Items.Item itemInstance = Object.Instantiate(pickupItem.item);
            itemInstance.CurrentStack = 1;

            if (_inventory.AddItem(itemInstance))
            {
                interactTarget.SetActive(false);
            }
        }
    }
}
