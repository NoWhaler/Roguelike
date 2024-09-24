using Game.WorldGeneration.ProceduralGenerator.SimplexNoiseGeneration.Models;
using UnityEditor;
using UnityEngine;

namespace Game.WorldGeneration.EditorConfiguration
{
    
    [CustomEditor(typeof(SimplexNoiseGeneratorModel))]
    public class SimplexMapGenerationEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            SimplexNoiseGeneratorModel mapGen = (SimplexNoiseGeneratorModel)target;
            
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