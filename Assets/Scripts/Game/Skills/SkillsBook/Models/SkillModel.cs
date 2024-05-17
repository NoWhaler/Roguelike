using System;
using System.IO;
using Game.Skills.SkillsBook.Views;
using UnityEngine;

namespace Game.Skills.SkillsBook.Models
{
    [Serializable]
    public class SkillModel
    {
        [NonSerialized] private readonly SkillView _skillView;

        public string SkillName { get; set; }

        public string Description { get; set; }
        
        public string ItemSpriteName { get; set; }
        
        [NonSerialized] public Sprite SkillSprite;
        
        public Sprite SkillIcon {
            get
            {
                try
                {
                    var imagePath = Path.Combine(Application.streamingAssetsPath, ItemSpriteName);
                    Texture2D texture = new Texture2D(2, 2);
                    byte[] data = null;
                    #if UNITY_ANDROID && !UNITY_EDITOR
                        using (UnityWebRequest www = UnityWebRequest.Get(imagePath))
                        {
                            www.SendWebRequest();
                            while (!www.isDone) { }
                            data = www.downloadHandler.data;
                        }
                    #else
                        data = File.ReadAllBytes(imagePath);
                    #endif
                    texture.LoadImage(data);
                    SkillSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    if (SkillSprite == null) {
                        Debug.LogError("Failed to load image: " + imagePath);
                    }
                }
                catch (Exception e)
                {
                    throw;
                }

                return SkillSprite;
            }
        }

        public SkillModel(SkillView skillView)
        {
            _skillView = skillView;
        }
    }
}