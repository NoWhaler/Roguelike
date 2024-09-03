using UnityEngine;

namespace Game.WorldGeneration.ChunkGeneration.Model
{
    public class ChunkModel: MonoBehaviour
    {
        [field: SerializeField] public MeshRenderer ChunkMeshRenderer { get; set; }
        
        [field: SerializeField] public MeshFilter ChunkMeshFilter { get; set; }
        
        [field: SerializeField] public MeshCollider ChunkMeshCollider { get; set; }
        
        public void DestroyChunk()
        {
            Destroy(gameObject);
        }
    }
}