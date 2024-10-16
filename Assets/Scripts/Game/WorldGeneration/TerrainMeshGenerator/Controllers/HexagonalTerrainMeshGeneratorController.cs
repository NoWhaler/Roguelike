using System.Collections.Generic;
using Game.WorldGeneration.ChunkGeneration.Model;
using Game.WorldGeneration.Hex;
using Game.WorldGeneration.Hex.Struct;
using Game.WorldGeneration.Nodes;
using Game.WorldGeneration.TerrainMeshGenerator.Models;
using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.TerrainMeshGenerator.Controllers
{
    public class HexagonalTerrainMeshGeneratorController
    {
        private HexagonalTerrainMeshGeneratorModel _hexagonalTerrainMeshGeneratorModel;
        private HexMouseDetector _hexMouseDetector;
        private HexGridController _hexGridController;
        private Dictionary<Vector2Int, HexagonalTerrainMeshGeneratorModel.HexChunk> _chunks;

        private DiContainer _diContainer;
        
        [Inject]
        private void Constructor(HexagonalTerrainMeshGeneratorModel hexagonalTerrainMeshGeneratorModel, HexMouseDetector hexMouseDetector, 
            HexGridController hexGridController, DiContainer diContainer)
        {
            _hexagonalTerrainMeshGeneratorModel = hexagonalTerrainMeshGeneratorModel;
            _hexMouseDetector = hexMouseDetector;
            _hexGridController = hexGridController;
            _diContainer = diContainer;
            _chunks = new Dictionary<Vector2Int, HexagonalTerrainMeshGeneratorModel.HexChunk>();
        }
        
        public void GenerateChunks(List<NodeModel> nodes, int chunksPerSide)
        {
            ClearExistingChunks();
            
            for (int x = -chunksPerSide; x <= chunksPerSide; x++)
            {
                for (int y = -chunksPerSide; y <= chunksPerSide; y++)
                {
                    Vector2Int chunkCoord = new Vector2Int(x, y);
                    GenerateChunk(chunkCoord, nodes);
                }
            }
        }
        
        private void GenerateChunk(Vector2Int chunkCoord, List<NodeModel> nodes)
        {
            var chunk = new HexagonalTerrainMeshGeneratorModel.HexChunk(chunkCoord);

            float chunkOffsetX = chunkCoord.x * (_hexagonalTerrainMeshGeneratorModel.hexWidth * 0.75f * _hexagonalTerrainMeshGeneratorModel.hexagonsPerChunkSide);
            
            float chunkOffsetZ = chunkCoord.y * (_hexagonalTerrainMeshGeneratorModel.hexHeight * _hexagonalTerrainMeshGeneratorModel.hexagonsPerChunkSide) + 
                         ((Mathf.Abs(chunkCoord.x) % 2) == 1 ? _hexagonalTerrainMeshGeneratorModel.hexHeight * 0.5f : 0);

            var chunkObject = CreateChunkGameObject(chunk);
            
            for (int x = 0; x < _hexagonalTerrainMeshGeneratorModel.hexagonsPerChunkSide; x++)
            {
                for (int y = 0; y < _hexagonalTerrainMeshGeneratorModel.hexagonsPerChunkSide; y++)
                {
                    Vector3 hexCenter = CalculateHexPosition(x, y, chunkOffsetX, chunkOffsetZ);
                    Color hexColor = GetColorForHexagon(hexCenter, nodes);
                    GenerateHexagonMesh(chunk, hexCenter, hexColor);
                }
            }

            RenderChunkObject(chunk, ref chunkObject);
            _chunks[chunkCoord] = chunk;
        }
        
        private Vector3 CalculateHexPosition(int x, int y, float offsetX, float offsetZ)
        {
            float xPos = (x * _hexagonalTerrainMeshGeneratorModel.hexWidth * 0.75f) + offsetX;
            float zPos = ((y + (x % 2) * 0.5f) * _hexagonalTerrainMeshGeneratorModel.hexHeight) + offsetZ;
            return new Vector3(xPos, 0, zPos);
        }
        
        private void GenerateHexagonMesh(HexagonalTerrainMeshGeneratorModel.HexChunk chunk, Vector3 center, Color color)
        {
            int vertexOffset = chunk.Vertices.Count;
            
            Matrix4x4 uvRotation = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.Euler(0, 30, 0),
                    Vector3.one
                );
            
            if (chunk.UVs == null)
            {
                chunk.UVs = new List<Vector2>();
            }
            
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI / 3f;
                Vector3 vertex = center + new Vector3(
                    Mathf.Cos(angle) * _hexagonalTerrainMeshGeneratorModel.hexRadius,
                    0,
                    Mathf.Sin(angle) * _hexagonalTerrainMeshGeneratorModel.hexRadius
                );
                chunk.Vertices.Add(vertex);
                chunk.Colors.Add(color);
                
                Vector3 normalizedPos = vertex - center;
                normalizedPos = uvRotation.MultiplyPoint3x4(normalizedPos);
                
                Vector2 uv = new Vector2(
                    (normalizedPos.x / _hexagonalTerrainMeshGeneratorModel.hexRadius + 1f) * 0.5f,
                    (normalizedPos.z / _hexagonalTerrainMeshGeneratorModel.hexRadius + 1f) * 0.5f
                );
                
                chunk.UVs.Add(uv);
            }
            
            chunk.Vertices.Add(center);
            chunk.Colors.Add(color);
            chunk.UVs.Add(new Vector2(0.5f, 0.5f));
            
            int centerVertex = chunk.Vertices.Count - 1;
            for (int i = 0; i < 6; i++)
            {
                int nextI = (i + 1) % 6;
                
                chunk.Triangles.Add(centerVertex);
                chunk.Triangles.Add(vertexOffset + i);
                chunk.Triangles.Add(vertexOffset + nextI);
            }
            
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.PI / 3f;
                Vector3 vertex = center + new Vector3(
                    Mathf.Cos(angle) * _hexagonalTerrainMeshGeneratorModel.hexRadius,
                    -0.1f,
                    Mathf.Sin(angle) * _hexagonalTerrainMeshGeneratorModel.hexRadius
                );
                chunk.Vertices.Add(vertex);
                chunk.Colors.Add(color);
                
                Vector2 uv = chunk.UVs[i];
                chunk.UVs.Add(uv);
            }
            
            for (int i = 0; i < 4; i++)
            {
                chunk.Triangles.Add(vertexOffset);
                chunk.Triangles.Add(vertexOffset + i + 1);
                chunk.Triangles.Add(vertexOffset + i + 2);
            }
            
            int bottomOffset = vertexOffset + 6;
            for (int i = 0; i < 4; i++)
            {
                chunk.Triangles.Add(bottomOffset);
                chunk.Triangles.Add(bottomOffset + i + 2);
                chunk.Triangles.Add(bottomOffset + i + 1);
            }
            
            for (int i = 0; i < 6; i++)
            {
                int nextI = (i + 1) % 6;
                
                chunk.Triangles.Add(vertexOffset + i);
                chunk.Triangles.Add(bottomOffset + i);
                chunk.Triangles.Add(vertexOffset + nextI);
                
                chunk.Triangles.Add(bottomOffset + i);
                chunk.Triangles.Add(bottomOffset + nextI);
                chunk.Triangles.Add(vertexOffset + nextI);
            }
            
            HexModel hexObject = _diContainer.InstantiatePrefabForComponent<HexModel>(
                    _hexagonalTerrainMeshGeneratorModel.HexPrefab, 
                    center, 
                    Quaternion.identity, 
                    chunk.ChunkObject.transform
                );
            
            hexObject.HexPosition = center;
            
            HexCoordinate logicalCoords = WorldToHexCoordinate(center);
            hexObject.HexPosition = center;
            hexObject.SetLogicalCoordinates(logicalCoords.Q, logicalCoords.R, logicalCoords.S);

            _hexGridController.SetHex(hexObject);
            hexObject.name = $"Hex({logicalCoords.Q}, {logicalCoords.R}, {logicalCoords.S})";
        }
        
        private HexCoordinate WorldToHexCoordinate(Vector3 worldPosition)
        {
            float q = (2f/3 * worldPosition.x) / _hexagonalTerrainMeshGeneratorModel.hexRadius;
            float r = (-1f/3 * worldPosition.x + Mathf.Sqrt(3)/3 * worldPosition.z) / _hexagonalTerrainMeshGeneratorModel.hexRadius;
            
            float x = q;
            float z = r;
            float y = -x - z;
    
            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);
    
            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);
    
            if (xDiff > yDiff && xDiff > zDiff)
                rx = -ry - rz;
            else if (yDiff > zDiff)
                ry = -rx - rz;
            else
                rz = -rx - ry;
    
            return new HexCoordinate(rx, rz);
        }
        
        private HexTerrainChunkModel CreateChunkGameObject(HexagonalTerrainMeshGeneratorModel.HexChunk chunk)
        {
            HexTerrainChunkModel hexTerrainChunkModel =
                _diContainer.InstantiatePrefabForComponent<HexTerrainChunkModel>(
                    _hexagonalTerrainMeshGeneratorModel.HexTerrainChunkPrefab, Vector3.zero, Quaternion.identity,
                    _hexagonalTerrainMeshGeneratorModel.transform);

            hexTerrainChunkModel.gameObject.name = $"Chunk({chunk.ChunkCoord.x}, {chunk.ChunkCoord.y})";
            chunk.ChunkObject = hexTerrainChunkModel;
            
            return hexTerrainChunkModel;
        }

        private void RenderChunkObject(HexagonalTerrainMeshGeneratorModel.HexChunk chunk, ref HexTerrainChunkModel hexTerrainChunkModel)
        {
            Mesh mesh = new Mesh
            {
                vertices = chunk.Vertices.ToArray(),
                triangles = chunk.Triangles.ToArray(),
                colors = chunk.Colors.ToArray(),
                uv = chunk.UVs.ToArray()
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            hexTerrainChunkModel.ChunkMeshFilter.mesh = mesh;
            
            hexTerrainChunkModel.ChunkMeshRenderer.material = _hexagonalTerrainMeshGeneratorModel.defaultMaterial;
        }
        
        private Color GetColorForHexagon(Vector3 hexCenter, List<NodeModel> nodes)
        {
            NodeModel closestNode = null;
            float closestDistance = float.MaxValue;
            
            foreach (var node in nodes)
            {
                float distance = Vector2.Distance(
                    new Vector2(hexCenter.x, hexCenter.z),
                    new Vector2(node.Position.x, node.Position.z));
                    
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }
            
            return closestNode?.NodeColor ?? Color.white;
        }
        
        private void ClearExistingChunks()
        {
            foreach (var chunk in _chunks.Values)
            {
                if (chunk.ChunkObject != null)
                {
                    Object.Destroy(chunk.ChunkObject);
                }
            }
            _chunks.Clear();
        }
    }
}