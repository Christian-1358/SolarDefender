using UnityEngine;
using SolarDefender.Database;

namespace SolarDefender.Database
{
    /// <summary>
    /// Garante que o banco de dados seja inicializado antes de qualquer outro script.
    /// Deve ser adicionado à primeira cena do jogo.
    /// </summary>
    public class DatabaseBootstrapper : MonoBehaviour
    {
        [Header("Configurações")]
        public string defaultPlayerName = "Commander";

        void Awake()
        {
            // Cria o DatabaseManager se não existir
            if (FindObjectOfType<DatabaseManager>() == null)
            {
                GameObject dbManager = new GameObject("[DatabaseManager]");
                dbManager.AddComponent<DatabaseManager>();
            }

            // Cria o DatabaseAccess se não existir
            if (FindObjectOfType<DatabaseAccess>() == null)
            {
                GameObject dbAccess = new GameObject("[DatabaseAccess]");
                dbAccess.AddComponent<DatabaseAccess>();
            }

            Debug.Log("Sistema de banco de dados inicializado!");
        }

        void Start()
        {
            // Verifica se há dados salvos
            if (DatabaseAccess.Instance != null)
            {
                var player = DatabaseAccess.Instance.GetOrCreatePlayer(defaultPlayerName);
                Debug.Log($"Banco de dados pronto! Jogador: {player.PlayerName} (ID: {player.Id})");
            }
        }
    }
}
