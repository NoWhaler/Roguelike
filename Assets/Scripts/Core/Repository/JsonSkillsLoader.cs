using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Initialization.SkillsInitialization.Model;
using Core.Repository.Interfaces;
using Game.Skills.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Repository
{
    public class JsonSkillsLoader: MonoBehaviour, IRepository
    {
        private SkillDataWrapper _skillDataWrapper;
        
        public Queue<SkillModel> LoadSkills()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "skills.json");
            byte[] data = null;
            #if UNITY_ANDROID && !UNITY_EDITOR
                            
                using (UnityWebRequest www = UnityWebRequest.Get(filePath))
                {
                    www.SendWebRequest();
                    while (!www.isDone) { }
                    data = www.downloadHandler.data;
                }
            #else
                data = File.ReadAllBytes(filePath);
            #endif

            string loadedJsonString = Encoding.UTF8.GetString(data);
            
            try
            {
                _skillDataWrapper = JsonConvert.DeserializeObject<SkillDataWrapper>(loadedJsonString);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to deserialize items from JSON: " + e.Message);
                throw;
            }
            
            return _skillDataWrapper.Skills;
        }
    }
    
    [Serializable]
    public class SkillDataWrapper
    {
        public Queue<SkillModel> Skills { get; set; }
    }
}