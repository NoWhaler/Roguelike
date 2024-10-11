using UnityEngine;

namespace Game.WorldGeneration.ChunkGeneration.Model
{
    public class HexTerrainChunkModel: MonoBehaviour
    {
        [field: SerializeField] public MeshRenderer ChunkMeshRenderer { get; private set; }
        
        [field: SerializeField] public MeshFilter ChunkMeshFilter { get; private set; }
    }
}