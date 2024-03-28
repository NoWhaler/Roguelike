using System;
using System.IO;
using Game.Skills.Views;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Skills.Models
{
    [Serializable]
    public class SkillModel
    {
        [NonSerialized] private readonly SkillView _skillView;

        public string SkillName { get; set; }

        public string Description { get; set; }
        
        public string ItemSpriteName { get; set; }
        
        [NonSerialized] private Sprite _itemSprite;
        
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
                    _itemSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    if (_itemSprite == null) {
                        Debug.LogError("Failed to load image: " + imagePath);
                    }
                }
                catch (Exception e)
                {
                    throw;
                }

                return _itemSprite;
            }
             }

        public SkillModel(SkillView skillView)
        {
            _skillView = skillView;
        }
    }
}