using System;
using System.IO;
using Core.Configuration;
using Core.Constants.FilePathes;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Core.Services
{
    public class DataService
    {
        public PlayerDataConfig PlayerDataConfig { get; set; }

        private readonly string _filePath = Path.Combine(Application.persistentDataPath, FilePath.PlayerDataPath);

        public async UniTask LoadPlayerData()
        {
            await IsFileExists();
            await IsPlayerDataExists();
        }
        
        private async UniTask IsPlayerDataExists()
        {
            var loadedData = await File.ReadAllTextAsync(_filePath);
            
            try
            {
                if (loadedData == "")
                {
                    PlayerDataConfig = new PlayerDataConfig().InitializePlayerData();

                    await PlayerDataConfig.SaveData();
                }
                
                if (loadedData != "")
                {
                    PlayerDataConfig = JsonConvert.DeserializeObject<PlayerDataConfig>(loadedData);
                }
               
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to deserialize items from JSON: " + e.Message);
            }
        }

        private async UniTask IsFileExists()
        {
            if (File.Exists(_filePath)) return;
            
            await File.Create(_filePath).DisposeAsync();

        }
    }
}