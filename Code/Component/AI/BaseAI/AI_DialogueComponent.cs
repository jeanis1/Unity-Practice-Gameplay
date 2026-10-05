using Code.UI.Dialogue;
using UnityEngine;

namespace Code.Component.AI
{
    public class AI_DialogueComponent : MonoBehaviour
    {
        [SerializeField] private DialogueNode startNode;

        public void StartDialogue()
        {
            // Validate before using
            if (startNode == null)
                Debug.LogError("StartNode is not assigned on " + gameObject.name);
                return;
            {
            }

            if (DialogueManager.Instance == null)
            {
                Debug.LogError("DialogueManager.Instance is null!");
                return;
            }

            DialogueManager.Instance.StartDialogue(startNode);
            Debug.Log("StartDialogue");
        }
    }
}


