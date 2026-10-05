using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Code.UI.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Transform choicesContainer;
        [SerializeField] private Button choiceButtonPrefab;

        private DialogueNode currentNode;
        private CursorLockMode previousLockState;
        private bool previousVisibleState;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            else
            {
                Debug.LogError("DialoguePanel is not assigned in DialogueManager.");
            }
        }

        public void StartDialogue(DialogueNode node)
        {
            previousLockState = Cursor.lockState;
            previousVisibleState = Cursor.visible;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            dialoguePanel.SetActive(true);
            currentNode = node;
            DisplayCurrentNode();
        }

        private void DisplayCurrentNode()
        {
            if (currentNode == null)
            {
                EndDialogue();
                return;
            }
            speakerNameText.text = currentNode.speakerName;
            dialogueText.text = currentNode.dialogueText;

            // Clear all existing choice buttons immediately
            foreach (Transform child in choicesContainer)
            {
                DestroyImmediate(child.gameObject);
            }

            if (currentNode.choices != null && currentNode.choices.Count > 0)
            {
                continueButton.gameObject.SetActive(false);
                CreateChoiceButtons();
            }
            else
            {
                continueButton.gameObject.SetActive(true);
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() => ContinueDialogue());
            }
        }

        private void CreateChoiceButtons()
        {
            for (int i = 0; i < currentNode.choices.Count; i++)
            {
                int index = i;
                Button button = Instantiate(choiceButtonPrefab, choicesContainer);
                button.gameObject.SetActive(true);

                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = currentNode.choices[i].choiceText;
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectChoice(index));
            }
        }

        public void SelectChoice(int choiceIndex)
        {
            currentNode = currentNode.choices[choiceIndex].nextNode;
            DisplayCurrentNode();
        }

        private void ContinueDialogue()
        {
            currentNode = currentNode.nextNode;
            DisplayCurrentNode();
        }

        private void EndDialogue()
        {
            Cursor.lockState = previousLockState;
            Cursor.visible = previousVisibleState;

            dialoguePanel.SetActive(false);
            currentNode = null;
        }
    }
}
