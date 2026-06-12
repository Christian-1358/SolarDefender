using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SolarDefender.FirstPerson
{
    public class HerbMixingSystem : MonoBehaviour
    {
        public static HerbMixingSystem Instance { get; private set; }

        [Header("Herb Mixing Recipes")]
        public List<HerbRecipe> recipes = new List<HerbRecipe>();

        [Header("UI")]
        public GameObject mixingPanel;
        public Transform herbSlotsContainer;
        public GameObject herbSlotPrefab;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI combineButtonText;
        public UnityEngine.UI.Button combineButton;
        public UnityEngine.UI.Button closeButton;

        [Header("Results")]
        public GameObject resultSlot;
        public TextMeshProUGUI resultNameText;
        public TextMeshProUGUI resultDescText;
        public UnityEngine.UI.Image resultIcon;

        private List<string> currentHerbs = new List<string>();
        private int maxHerbSlots = 3;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            InitializeRecipes();
        }

        void Start()
        {
            if (combineButton != null)
            {
                combineButton.onClick.AddListener(CombineHerbs);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }
        }

        void InitializeRecipes()
        {
            // Receitas de combinações de ervas
            recipes.Add(new HerbRecipe("herb_green", "herb_yellow", "herb_green_yellow", "Verde + Amarela", "Cura moderada."));
            recipes.Add(new HerbRecipe("herb_yellow", "herb_green", "herb_green_yellow", "Verde + Amarela", "Cura moderada."));

            recipes.Add(new HerbRecipe("herb_green", "herb_red", "herb_green_red", "Verde + Vermelha", "Cura alta!"));
            recipes.Add(new HerbRecipe("herb_red", "herb_green", "herb_green_red", "Verde + Vermelha", "Cura alta!"));

            recipes.Add(new HerbRecipe("herb_green", "herb_blue", "herb_green_blue", "Verde + Azul", "Cura com escudo."));
            recipes.Add(new HerbRecipe("herb_blue", "herb_green", "herb_green_blue", "Verde + Azul", "Cura com escudo."));

            recipes.Add(new HerbRecipe("herb_green_yellow", "herb_red", "herb_green_yellow_red", "VerdeAmarela + Vermelha", "Cura muito alta!"));
            recipes.Add(new HerbRecipe("herb_red", "herb_green_yellow", "herb_green_yellow_red", "Vermelha + VerdeAmarela", "Cura muito alta!"));

            recipes.Add(new HerbRecipe("herb_green_red", "herb_yellow", "herb_green_yellow_red", "VerdeVermelha + Amarela", "Cura máxima!"));
            recipes.Add(new HerbRecipe("herb_yellow", "herb_green_red", "herb_green_yellow_red", "Amarela + VerdeVermelha", "Cura máxima!"));

            recipes.Add(new HerbRecipe("herb_green_yellow_red", "herb_green", "herb_green_yellow_red", "Todas + Verde", "Cura completa!"));
            recipes.Add(new HerbRecipe("herb_green", "herb_green_yellow_red", "herb_green_yellow_red", "Verde + Todas", "Cura completa!"));
        }

        public void OpenPanel()
        {
            if (mixingPanel != null)
            {
                mixingPanel.SetActive(true);
                currentHerbs.Clear();
                UpdateUI();
            }
        }

        public void ClosePanel()
        {
            if (mixingPanel != null)
            {
                mixingPanel.SetActive(false);
            }
        }

        public void AddHerb(string herbId)
        {
            if (currentHerbs.Count >= maxHerbSlots) return;

            // Se já tem uma erva, não aceita duplicata
            if (currentHerbs.Contains(herbId)) return;

            currentHerbs.Add(herbId);
            UpdateUI();
        }

        public void RemoveHerb(int index)
        {
            if (index >= 0 && index < currentHerbs.Count)
            {
                currentHerbs.RemoveAt(index);
                UpdateUI();
            }
        }

        public void ClearHerbs()
        {
            currentHerbs.Clear();
            UpdateUI();
        }

        void UpdateUI()
        {
            // Atualiza slots de ervas
            if (herbSlotsContainer != null)
            {
                // Limpa slots
                foreach (Transform child in herbSlotsContainer)
                {
                    Destroy(child.gameObject);
                }

                // Cria slots para cada erva
                for (int i = 0; i < maxHerbSlots; i++)
                {
                    GameObject slot = Instantiate(herbSlotPrefab, herbSlotsContainer);

                    var slotUI = slot.GetComponent<HerbSlotUI>();
                    if (slotUI != null)
                    {
                        if (i < currentHerbs.Count)
                        {
                            var item = MerchantItemsDatabase.Instance.GetItem(currentHerbs[i]);
                            slotUI.Setup(item, i);
                        }
                        else
                        {
                            slotUI.SetupEmpty(i);
                        }
                    }
                }
            }

            // Atualiza resultado
            string resultId = GetRecipeResult();
            if (!string.IsNullOrEmpty(resultId))
            {
                var resultItem = MerchantItemsDatabase.Instance.GetItem(resultId);
                if (resultItem != null)
                {
                    if (resultText != null) resultText.text = resultItem.itemName;
                    if (resultNameText != null) resultNameText.text = resultItem.itemName;
                    if (resultDescText != null) resultDescText.text = resultItem.description;
                    if (resultIcon != null && resultItem.icon != null) resultIcon.sprite = resultItem.icon;

                    if (combineButtonText != null) combineButtonText.text = "MISTURAR";
                    if (combineButton != null) combineButton.interactable = true;
                }
            }
            else
            {
                if (resultText != null) resultText.text = "???";
                if (resultNameText != null) resultNameText.text = "-";
                if (resultDescText != null) resultDescText.text = "Misture ervas para criar medicamentos";
                if (combineButton != null) combineButton.interactable = currentHerbs.Count >= 2;
                if (combineButtonText != null) combineButtonText.text = currentHerbs.Count < 2 ? "2+ ervas" : "MISTURAR";
            }
        }

        string GetRecipeResult()
        {
            if (currentHerbs.Count < 2) return null;

            // Tenta encontrar receita
            foreach (var recipe in recipes)
            {
                if (MatchesRecipe(recipe))
                {
                    return recipe.resultId;
                }
            }

            return null;
        }

        bool MatchesRecipe(HerbRecipe recipe)
        {
            var sortedHerbs = currentHerbs.OrderBy(h => h).ToList();
            var sortedRecipe = new List<string> { recipe.herb1, recipe.herb2 }.OrderBy(h => h).ToList();

            if (sortedHerbs.Count == 2)
            {
                return sortedHerbs[0] == sortedRecipe[0] && sortedHerbs[1] == sortedRecipe[1];
            }
            else if (sortedHerbs.Count == 3 && !string.IsNullOrEmpty(recipe.herb3))
            {
                var sortedRecipe3 = new List<string> { recipe.herb1, recipe.herb2, recipe.herb3 }.OrderBy(h => h).ToList();
                return sortedHerbs[0] == sortedRecipe3[0] &&
                       sortedHerbs[1] == sortedRecipe3[1] &&
                       sortedHerbs[2] == sortedRecipe3[2];
            }

            return false;
        }

        public void CombineHerbs()
        {
            string resultId = GetRecipeResult();
            if (string.IsNullOrEmpty(resultId)) return;

            var resultItem = MerchantItemsDatabase.Instance.GetItem(resultId);
            if (resultItem == null) return;

            // Adiciona resultado ao inventário
            if (BackpackInventory.Instance != null)
            {
                if (BackpackInventory.Instance.AddItem(resultId, 1))
                {
                    // Limpa ervas usadas
                    foreach (var herb in currentHerbs)
                    {
                        BackpackInventory.Instance.RemoveItem(herb, 1);
                    }

                    // Feedback
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayBuy();
                    }

                    ClearHerbs();
                    ClosePanel();
                }
            }
        }

        public int GetCurrentHerbCount() => currentHerbs.Count;
        public List<string> GetCurrentHerbs() => new List<string>(currentHerbs);
    }

    [System.Serializable]
    public class HerbRecipe
    {
        public string herb1;
        public string herb2;
        public string herb3; // Opcional para receitas de 3 ervas
        public string resultId;
        public string recipeName;
        public string description;

        public HerbRecipe(string h1, string h2, string result, string name, string desc)
        {
            herb1 = h1;
            herb2 = h2;
            herb3 = null;
            resultId = result;
            recipeName = name;
            description = desc;
        }

        public HerbRecipe(string h1, string h2, string h3, string result, string name, string desc)
        {
            herb1 = h1;
            herb2 = h2;
            herb3 = h3;
            resultId = result;
            recipeName = name;
            description = desc;
        }
    }

    public class HerbSlotUI : MonoBehaviour
    {
        public UnityEngine.UI.Image iconImage;
        public TextMeshProUGUI nameText;
        public UnityEngine.UI.Button removeButton;
        public GameObject emptyIndicator;

        private int slotIndex;

        public void Setup(MerchantItem item, int index)
        {
            slotIndex = index;

            if (item != null)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = item.icon;
                    iconImage.enabled = true;
                }
                if (nameText != null)
                {
                    nameText.text = item.itemName;
                }
                if (emptyIndicator != null) emptyIndicator.SetActive(false);
            }

            if (removeButton != null)
            {
                removeButton.onClick.RemoveAllListeners();
                removeButton.onClick.AddListener(() => RemoveHerb());
            }
        }

        public void SetupEmpty(int index)
        {
            slotIndex = index;

            if (iconImage != null) iconImage.enabled = false;
            if (nameText != null) nameText.text = "-";
            if (emptyIndicator != null) emptyIndicator.SetActive(true);
            if (removeButton != null) removeButton.interactable = false;
        }

        void RemoveHerb()
        {
            if (HerbMixingSystem.Instance != null)
            {
                HerbMixingSystem.Instance.RemoveHerb(slotIndex);
            }
        }
    }
}
