// namespace Game.WorldGeneration
// {
//     public class Test
//     {
//         
//         private void SmoothChunkBorders()
//         {
//             foreach (var kvp in _chunkMeshes)
//             {
//                 Vector2Int chunkPos = kvp.Key;
//                 ChunkModel chunk = kvp.Value;
//
//                 if (_chunkMeshes.TryGetValue(chunkPos + Vector2Int.right, out ChunkModel rightNeighbor))
//                 {
//                     SmoothBorder(chunk, rightNeighbor, Vector3.right);
//                 }
//
//                 if (_chunkMeshes.TryGetValue(chunkPos + Vector2Int.up, out ChunkModel topNeighbor))
//                 {
//                     SmoothBorder(chunk, topNeighbor, Vector3.forward);
//                 }
//
//                 if (_chunkMeshes.TryGetValue(chunkPos + Vector2Int.right + Vector2Int.up, out ChunkModel cornerNeighbor))
//                 {
//                     SmoothCorner(chunk, rightNeighbor, topNeighbor, cornerNeighbor);
//                 }
//             }
//         }
//         
//         private void SmoothCorner(ChunkModel chunk, ChunkModel rightNeighbor, ChunkModel topNeighbor, ChunkModel cornerNeighbor)
//         {
//             Mesh mesh1 = chunk.ChunkMeshFilter.sharedMesh;
//             Mesh mesh2 = rightNeighbor.ChunkMeshFilter.sharedMesh;
//             Mesh mesh3 = topNeighbor.ChunkMeshFilter.sharedMesh;
//             Mesh mesh4 = cornerNeighbor.ChunkMeshFilter.sharedMesh;
//
//             Vector3[] vertices1 = mesh1.vertices;
//             Vector3[] vertices2 = mesh2.vertices;
//             Vector3[] vertices3 = mesh3.vertices;
//             Vector3[] vertices4 = mesh4.vertices;
//
//             int size = _generatorModel.ChunkSize + 1;
//             int smoothingRange = _generatorModel.SmoothRadius;
//
//             for (int i = 0; i < smoothingRange; i++)
//             {
//                 for (int j = 0; j < smoothingRange; j++)
//                 {
//                     int index1 = (size - 1 - i) * size + (size - 1 - j);
//                     int index2 = (size - 1 - i) * size + j;
//                     int index3 = i * size + (size - 1 - j);
//                     int index4 = i * size + j;
//
//                     float weightX = 1f - (float)j / smoothingRange;
//                     float weightY = 1f - (float)i / smoothingRange;
//
//                     float avgHeight = (
//                         vertices1[index1].y * weightX * weightY +
//                         vertices2[index2].y * (1 - weightX) * weightY +
//                         vertices3[index3].y * weightX * (1 - weightY) +
//                         vertices4[index4].y * (1 - weightX) * (1 - weightY)
//                     );
//
//                     vertices1[index1].y = avgHeight;
//                     vertices2[index2].y = avgHeight;
//                     vertices3[index3].y = avgHeight;
//                     vertices4[index4].y = avgHeight;
//                 }
//             }
//
//             mesh1.vertices = vertices1;
//             mesh2.vertices = vertices2;
//             mesh3.vertices = vertices3;
//             mesh4.vertices = vertices4;
//
//             mesh1.RecalculateNormals();
//             mesh2.RecalculateNormals();
//             mesh3.RecalculateNormals();
//             mesh4.RecalculateNormals();
//         }
//
//         private void SmoothBorder(ChunkModel chunk1, ChunkModel chunk2, Vector3 direction)
//         {
//             Mesh mesh1 = chunk1.ChunkMeshFilter.sharedMesh;
//             Mesh mesh2 = chunk2.ChunkMeshFilter.sharedMesh;
//             Vector3[] vertices1 = mesh1.vertices;
//             Vector3[] vertices2 = mesh2.vertices;
//
//             int size = _generatorModel.ChunkSize + 1;
//             int smoothingRange = _generatorModel.SmoothRadius;
//
//             for (int i = 0; i < size; i++)
//             {
//                 for (int j = 0; j < smoothingRange; j++)
//                 {
//                     int index1, index2;
//
//                     if (direction == Vector3.right)
//                     {
//                         index1 = i * size + (size - 1 - j);
//                         index2 = i * size;
//                     }
//                     else
//                     {
//                         index1 = (size - 1 - j) * size + i;
//                         index2 = j * size + i;
//                     }
//
//                     float weight = 1f - (float)j / smoothingRange;
//                     float avgHeight = vertices1[index1].y * weight + vertices2[index2].y * (1 - weight);
//
//                     vertices1[index1].y = avgHeight;
//                     vertices2[index2].y = avgHeight;
//                 }
//             }
//
//             mesh1.vertices = vertices1;
//             mesh2.vertices = vertices2;
//             mesh1.RecalculateNormals();
//             mesh2.RecalculateNormals();
//         }
//     }
// }