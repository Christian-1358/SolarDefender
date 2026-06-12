using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Settings
{
    [System.Serializable]
    public class Keybind
    {
        public string actionName;
        public KeyCode key;
        public KeyCode altKey;
        public bool shiftRequired;
        public bool ctrlRequired;
        public bool altRequired;
    }

    public class KeybindManager : MonoBehaviour
    {
        public static KeybindManager Instance { get; private set; }

        [Header("Keybinds")]
        public List<Keybind> keybinds = new List<Keybind>();

        [Header("UI")]
        public GameObject keybindPanel;
        public Transform keybindListContent;
        public GameObject keybindPrefab;

        private Dictionary<string, Keybind> keybindDict = new Dictionary<string, Keybind>();

        public event Action OnKeybindsChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeDefaultKeybinds();
            }
        }

        void InitializeDefaultKeybinds()
        {
            keybinds = new List<Keybind>
            {
                // Movement
                new Keybind { actionName = "Move Forward", key = KeyCode.W },
                new Keybind { actionName = "Move Backward", key = KeyCode.S },
                new Keybind { actionName = "Move Left", key = KeyCode.A },
                new Keybind { actionName = "Move Right", key = KeyCode.D },
                new Keybind { actionName = "Jump", key = KeyCode.Space },
                new Keybind { actionName = "Crouch", key = KeyCode.C },
                new Keybind { actionName = "Sprint", key = KeyCode.LeftShift },

                // Combat
                new Keybind { actionName = "Fire", key = KeyCode.Mouse0 },
                new Keybind { actionName = "Aim", key = KeyCode.Mouse1 },
                new Keybind { actionName = "Reload", key = KeyCode.R },
                new Keybind { actionName = "Switch Weapon 1", key = KeyCode.Alpha1 },
                new Keybind { actionName = "Switch Weapon 2", key = KeyCode.Alpha2 },
                new Keybind { actionName = "Switch Weapon 3", key = KeyCode.Alpha3 },

                // Abilities
                new Keybind { actionName = "Ability 1", key = KeyCode.Q },
                new Keybind { actionName = "Ability 2", key = KeyCode.E },
                new Keybind { actionName = "Ability 3", key = KeyCode.R },
                new Keybind { actionName = "Ability 4", key = KeyCode.T },
                new Keybind { actionName = "Ability 5", key = KeyCode.Y },
                new Keybind { actionName = "Ability 6", key = KeyCode.U },

                // UI
                new Keybind { actionName = "Pause", key = KeyCode.Escape },
                new Keybind { actionName = "Inventory", key = KeyCode.Tab },
                new Keybind { actionName = "Shop", key = KeyCode.M },
                new Keybind { actionName = "Map", key = KeyCode.M },
                new Keybind { actionName = "Scoreboard", key = KeyCode.Tab },

                // Special
                new Keybind { actionName = "First Person Mode", key = KeyCode.F },
                new Keybind { actionName = "Dash", key = KeyCode.Shift },
                new Keybind { actionName = "Melee", key = KeyCode.V }
            };

            // Build dictionary
            foreach (var keybind in keybinds)
            {
                keybindDict[keybind.actionName] = keybind;
            }

            LoadKeybinds();
        }

        public bool IsKeyPressed(string actionName)
        {
            if (!keybindDict.ContainsKey(actionName)) return false;

            Keybind keybind = keybindDict[actionName];

            bool modifiersMatch = true;
            if (keybind.shiftRequired && !Input.GetKey(KeyCode.LeftShift)) modifiersMatch = false;
            if (keybind.ctrlRequired && !Input.GetKey(KeyCode.LeftControl)) modifiersMatch = false;
            if (keybind.altRequired && !Input.GetKey(KeyCode.LeftAlt)) modifiersMatch = false;

            if (!modifiersMatch) return false;

            return Input.GetKey(keybind.key) || Input.GetKey(keybind.altKey);
        }

        public bool IsKeyDown(string actionName)
        {
            if (!keybindDict.ContainsKey(actionName)) return false;

            Keybind keybind = keybindDict[actionName];

            bool modifiersMatch = true;
            if (keybind.shiftRequired && !Input.GetKey(KeyCode.LeftShift)) modifiersMatch = false;
            if (keybind.ctrlRequired && !Input.GetKey(KeyCode.LeftControl)) modifiersMatch = false;
            if (keybind.altRequired && !Input.GetKey(KeyCode.LeftAlt)) modifiersMatch = false;

            if (!modifiersMatch) return false;

            return Input.GetKeyDown(keybind.key) || Input.GetKeyDown(keybind.altKey);
        }

        public void SetKeybind(string actionName, KeyCode newKey, KeyCode altKey = KeyCode.None)
        {
            if (!keybindDict.ContainsKey(actionName)) return;

            keybindDict[actionName].key = newKey;
            keybindDict[actionName].altKey = altKey;

            SaveKeybinds();
            OnKeybindsChanged?.Invoke();
        }

        public void ResetToDefaults()
        {
            keybinds.Clear();
            keybindDict.Clear();
            InitializeDefaultKeybinds();
            SaveKeybinds();
        }

        public Keybind GetKeybind(string actionName)
        {
            return keybindDict.ContainsKey(actionName) ? keybindDict[actionName] : null;
        }

        public string GetKeybindDisplayString(string actionName)
        {
            if (!keybindDict.ContainsKey(actionName)) return "Not Bound";

            Keybind keybind = keybindDict[actionName];
            string modifiers = "";
            if (keybind.shiftRequired) modifiers += "Shift+";
            if (keybind.ctrlRequired) modifiers += "Ctrl+";
            if (keybind.altRequired) modifiers += "Alt+";

            string keyName = keybind.key.ToString();
            if (keybind.key == KeyCode.Mouse0) keyName = "LMB";
            else if (keybind.key == KeyCode.Mouse1) keyName = "RMB";
            else if (keybind.key == KeyCode.Mouse2) keyName = "MMB";

            return modifiers + keyName;
        }

        void SaveKeybinds()
        {
            string json = JsonUtility.ToJson(keybinds);
            PlayerPrefs.SetString("Keybinds", json);
            PlayerPrefs.Save();
        }

        void LoadKeybinds()
        {
            string json = PlayerPrefs.GetString("Keybinds", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    List<Keybind> savedKeybinds = JsonUtility.FromJson<List<Keybind>>(json);
                    foreach (var saved in savedKeybinds)
                    {
                        if (keybindDict.ContainsKey(saved.actionName))
                        {
                            keybindDict[saved.actionName].key = saved.key;
                            keybindDict[saved.actionName].altKey = saved.altKey;
                        }
                    }
                }
                catch { }
            }
        }
    }
}
