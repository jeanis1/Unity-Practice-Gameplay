using System.Collections.Generic;
using UnityEngine;

namespace Code.UI.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue/Node")]
    public class DialogueNode : ScriptableObject
    {
        public string speakerName;
        [TextArea(3, 10)]
        public string dialogueText;
        public List<DialogueChoice> choices;
        public DialogueNode nextNode; //for linear dialogue
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public DialogueNode nextNode;
    }

}
