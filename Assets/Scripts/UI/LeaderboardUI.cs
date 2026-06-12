using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SolarDefender.Database;
using SolarDefender.Database.Models;

namespace SolarDefender.UI
{
    public class LeaderboardUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject leaderboardPanel;
        public Transform entriesContainer;
        public GameObject entryPrefab;
        public TextMeshProUGUI titleText;
        public Button closeButton;

        [Header("Config")]
        public int maxEntries = 10;

        void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
            Hide();
        }

        public void Show()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(true);
                PopulateLeaderboard();
            }
        }

        public void Hide()
        {
            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(false);
            }
        }

        void PopulateLeaderboard()
        {
            // Limpa entradas anteriores
            foreach (Transform child in entriesContainer)
            {
                Destroy(child.gameObject);
            }

            // Obtém scores do banco
            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
            if (DatabaseAccess.Instance != null)
            {
                entries = DatabaseAccess.Instance.Leaderboard.GetTopScores(maxEntries);
            }

            // Cria entradas na UI
            int rank = 1;
            foreach (var entry in entries)
            {
                GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
                LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();

                if (entryUI != null)
                {
                    entryUI.Setup(rank, entry.PlayerName, entry.Score, entry.LevelReached, entry.Combo);
                }

                rank++;
            }

            if (titleText != null)
            {
                titleText.text = $"Top {maxEntries} - Total: {DatabaseAccess.Instance.Leaderboard.GetTotalPlayers()} jogadores";
            }
        }
    }

    public class LeaderboardEntryUI : MonoBehaviour
    {
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI comboText;

        public void Setup(int rank, string name, int score, int level, int combo)
        {
            if (rankText != null) rankText.text = $"#{rank}";
            if (nameText != null) nameText.text = name;
            if (scoreText != null) scoreText.text = score.ToString("N0");
            if (levelText != null) levelText.text = $"Fase {level}";
            if (comboText != null) comboText.text = $"x{combo}";
        }
    }
}
