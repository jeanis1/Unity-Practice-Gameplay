using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.Component
{
    public class CharacterHealthUIComponent : MonoBehaviour
    {
        [SerializeField] private CharacterHealthComponent _healthComponent;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Slider _healthSlider;

        void Start()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnHealthChanged += UpdateUI;
                UpdateUI(_healthComponent.Health, _healthComponent.MaxHealth);
            }
        }

        void OnDestroy()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnHealthChanged -= UpdateUI;
            }
        }

        private void UpdateUI(float current, float max)
        {
            _healthSlider.value = current / max;
            _healthText.text = current.ToString();
        }
    }
}
