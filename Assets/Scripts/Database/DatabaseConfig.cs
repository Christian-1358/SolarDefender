using UnityEngine;

namespace SolarDefender.Database
{
    public static class DatabaseConfig
    {
        public static readonly string DatabaseName = "solar_defender.db";
        public static readonly string DatabasePath;

        static DatabaseConfig()
        {
            // path para standalone builds
            if (Application.isPlaying && !Application.isEditor)
            {
                DatabasePath = System.IO.Path.Combine(Application.persistentDataPath, DatabaseName);
            }
            // path para editor
            else
            {
                DatabasePath = System.IO.Path.Combine(Application.dataPath, "Database", DatabaseName);
            }
        }
    }
}
