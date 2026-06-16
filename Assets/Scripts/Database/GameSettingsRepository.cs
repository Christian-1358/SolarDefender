using UnityEngine;

namespace SolarDefender.Database
{
    public class GameSettingsRepository
    {
        private DatabaseManager db;

        public GameSettingsRepository()
        {
            db = DatabaseManager.Instance;
        }

        public float GetMasterVolume() { return db.GetMasterVolume(); }
        public void SetMasterVolume(float volume) { db.SetMasterVolume(volume); }
        public float GetMusicVolume() { return db.GetMusicVolume(); }
        public void SetMusicVolume(float volume) { db.SetMusicVolume(volume); }
        public float GetSFXVolume() { return db.GetSFXVolume(); }
        public void SetSFXVolume(float volume) { db.SetSFXVolume(volume); }
    }
}
