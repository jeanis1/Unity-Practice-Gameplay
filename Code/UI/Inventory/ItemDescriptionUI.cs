using Code.Component.Items;
using UnityEngine;
using TMPro;

namespace Code.UI.Inventory
{
    public class ItemDescriptionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Vector2 offset = new Vector2(10f, 0f); //offset from slot position

        private RectTransform rectTransform;
        private Canvas canvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            Hide();
            Debug.Log("ItemDescription Awake called.");
        }

        public void Show(Item item, RectTransform slotTransform)
        {
            if (item == null) return;

            itemNameText.text = item.ItemName;
            descriptionText.text = item.Description;
            
            //Position at top right of slot
            Vector3[] slotCorners = new Vector3[4];
            slotTransform.GetWorldCorners(slotCorners);
            Vector2 slotTopRight = slotCorners[2]; //top right of corner
            
            //Convert to canvas space
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                rectTransform.position = slotTopRight + offset;
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    slotTopRight,
                    canvas.worldCamera,
                    out Vector2 localPoint
                    );
                rectTransform.localPosition = localPoint + offset;
            }
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
