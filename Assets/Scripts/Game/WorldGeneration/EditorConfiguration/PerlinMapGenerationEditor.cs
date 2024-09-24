using Game.WorldGeneration.ProceduralGenerator.PerlinNoiseGeneration.Models;
using UnityEditor;
using UnityEngine;

namespace Game.WorldGeneration.EditorConfiguration
{
    [CustomEditor(typeof(PerlinNoiseGeneratorModel))]
    public class PerlinMapGenerationEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            PerlinNoiseGeneratorModel mapGen = (PerlinNoiseGeneratorModel)target;
            
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