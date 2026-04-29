using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PocketDungeons.Data;

namespace PocketDungeons.UI.Views
{
    /// <summary>
    /// Power-up selection UI. Shows 3 cards after floor clear. Player picks one.
    /// Cards show icon, name, description, rarity color, and stack count.
    /// </summary>
    public class PowerUpSelectionView : MonoBehaviour
    {
        [System.Serializable]
        public struct PowerUpCard
        {
            public GameObject Root;
            public Image Icon;
            public Image RarityBorder;
            public TextMeshProUGUI NameText;
            public TextMeshProUGUI DescriptionText;
            public TextMeshProUGUI CategoryText;
            public Button SelectButton;
        }

        [Header("Cards")]
        [SerializeField] private PowerUpCard[] _cards = new PowerUpCard[3];

        [Header("Rarity Colors")]
        [SerializeField] private Color _commonColor = Color.white;
        [SerializeField] private Color _uncommonColor = Color.green;
        [SerializeField] private Color _rareColor = Color.blue;
        [SerializeField] private Color _epicColor = new(0.6f, 0f, 0.8f);
        [SerializeField] private Color _legendaryColor = new(1f, 0.84f, 0f);

        private PowerUpData[] _currentChoices;
        public System.Action<PowerUpData> OnPowerUpSelected;

        private void Awake()
        {
            for (int i = 0; i < _cards.Length; i++)
            {
                int index = i;
                _cards[i].SelectButton?.onClick.AddListener(() => SelectCard(index));
            }

            gameObject.SetActive(false);
        }

        public void Show(PowerUpData[] choices)
        {
            _currentChoices = choices;
            gameObject.SetActive(true);
            Time.timeScale = 0f;

            for (int i = 0; i < _cards.Length; i++)
            {
                if (i < choices.Length)
                {
                    _cards[i].Root.SetActive(true);
                    PopulateCard(_cards[i], choices[i]);
                }
                else
                {
                    _cards[i].Root.SetActive(false);
                }
            }
        }

        private void PopulateCard(PowerUpCard card, PowerUpData data)
        {
            if (card.Icon != null && data.Icon != null)
                card.Icon.sprite = data.Icon;

            if (card.NameText != null)
                card.NameText.text = data.DisplayName;

            if (card.DescriptionText != null)
                card.DescriptionText.text = data.Description;

            if (card.CategoryText != null)
                card.CategoryText.text = data.Category.ToString();

            if (card.RarityBorder != null)
                card.RarityBorder.color = GetRarityColor(data.Rarity);
        }

        private void SelectCard(int index)
        {
            if (_currentChoices == null || index >= _currentChoices.Length) return;

            Time.timeScale = 1f;
            gameObject.SetActive(false);
            OnPowerUpSelected?.Invoke(_currentChoices[index]);
        }

        private Color GetRarityColor(Rarity rarity) => rarity switch
        {
            Rarity.Common => _commonColor,
            Rarity.Uncommon => _uncommonColor,
            Rarity.Rare => _rareColor,
            Rarity.Epic => _epicColor,
            Rarity.Legendary => _legendaryColor,
            _ => _commonColor
        };
    }
}
