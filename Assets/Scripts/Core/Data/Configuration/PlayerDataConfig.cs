using System;
using System.IO;
using Core.Constants.FilePathes;
using Core.Data;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Device;

namespace Core.Configuration
{
    public class PlayerDataConfig
    {
        public PlayerData PlayerData { get; set; }

        public PlayerDataConfig InitializePlayerData()
        {
            var playerData = new PlayerData()
            {
                PlayerCurrentHealth = 50,
                PlayerMaxHealth = 50,
                PlayerDamage = 6
            };

            PlayerData = playerData;

            return this;
        }
        
        public async UniTask SaveData()
        {
            var filePath = Path.Combine(Application.persistentDataPath, FilePath.PlayerDataPath);
            
            await ClearData(filePath);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            await File.WriteAllTextAsync(filePath, json);
        }

        public async UniTask ClearData(string filePath)
        {
            await File.WriteAllTextAsync(filePath, String.Empty);
        }
    }
}