using Game.WorldGeneration.RandomGenerator.Models;
using UnityEditor;
using UnityEngine;

namespace Game.WorldGeneration
{
    [CustomEditor(typeof(GeneratorModel))]
    public class MapGenerationEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            GeneratorModel mapGen = (GeneratorModel)target;
            Debug.Log(mapGen);
    
            if (DrawDefaultInspector())
            {
                mapGen.OnGenerateMap?.Invoke();
            }
    
            if (GUILayout.Button("Generate"))
            {
                mapGen.OnGenerateMap?.Invoke();
            }
        }
    }
}