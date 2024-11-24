using System.Collections.Generic;
using UnityEngine;

namespace Game.WorldGeneration
{
    public class IslandBoundaryController
    {
        private Vector3 _center;
        private float _baseRadius;
        private float _noiseScale = 0.015f;
        private float _largeNoiseScale = 0.004f;
        private float _noiseThreshold = 0.3f;
        private float _coastalNoiseAmplitude = 0.3f;
        private List<Vector2> _peninsulaCenters;
        
        private float _xScale = 0.85f;
        private float _zScale = 0.76f;
        
        public IslandBoundaryController(Vector3 center, float radius, int peninsulaCount = 120)
        {
            _center = center;
            _baseRadius = radius;
            GeneratePeninsulaCenters(peninsulaCount);
        }

        private void GeneratePeninsulaCenters(int count)
        {
            _peninsulaCenters = new List<Vector2>();
            float angleStep = 360f / count;
            
            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float randomOffset = GetDirectionalOffset(angle);
                float randomAngleOffset = Random.Range(-20f, 20f) * Mathf.Deg2Rad;
                angle += randomAngleOffset;
                
                Vector2 position = new Vector2(
                    _center.x + Mathf.Cos(angle) * _baseRadius * randomOffset * _xScale,
                    _center.z + Mathf.Sin(angle) * _baseRadius * randomOffset * _zScale
                );
                _peninsulaCenters.Add(position);
            }
        }

        private float GetDirectionalOffset(float angle)
        {
            angle = angle % (2 * Mathf.PI);
            if (angle < 0) angle += 2 * Mathf.PI;

            if (IsWithinRange(angle, 0.25f * Mathf.PI, 0.75f * Mathf.PI) || 
                IsWithinRange(angle, 1.25f * Mathf.PI, 1.75f * Mathf.PI))
            {
                return Random.Range(0.9f, 1f);
            }
            else
            {
                return Random.Range(0.7f, 0.8f);
            }
        }

        private bool IsWithinRange(float angle, float start, float end)
        {
            return angle >= start && angle <= end;
        }

        public bool IsWithinBoundary(Vector3 position)
        {
            float xDist = (position.x - _center.x) / _xScale;
            float zDist = (position.z - _center.z) / _zScale;
            float distanceFromCenter = Mathf.Sqrt(xDist * xDist + zDist * zDist) / _baseRadius;
            
            float minPeninsulaDistance = float.MaxValue;
            foreach (Vector2 peninsulaCenter in _peninsulaCenters)
            {
                float xDiff = (position.x - peninsulaCenter.x) / _xScale;
                float zDiff = (position.z - peninsulaCenter.y) / _zScale;
                float dist = Mathf.Sqrt(xDiff * xDiff + zDiff * zDiff) / _baseRadius;
                minPeninsulaDistance = Mathf.Min(minPeninsulaDistance, dist);
            }
            
            float combinedDistance = Mathf.Min(distanceFromCenter, minPeninsulaDistance);
            float baseMask = Mathf.Clamp01(1.2f - combinedDistance);
            
            float largeNoise = Mathf.PerlinNoise(
                position.x * _largeNoiseScale, 
                position.z * _largeNoiseScale
            );
            
            float detailNoise = Mathf.PerlinNoise(
                position.x * _noiseScale + 1000, 
                position.z * _noiseScale + 1000
            );
            
            float edgeDistance = 1f - combinedDistance;
            float noiseInfluence = (largeNoise * 0.8f + detailNoise * 0.5f) * 
                                 _coastalNoiseAmplitude * 
                                 (1f + edgeDistance * 0.5f);
            
            float combinedValue = baseMask * (0.8f + noiseInfluence);
            
            float dynamicThreshold = _noiseThreshold + 
                Mathf.PerlinNoise(position.x * _noiseScale * 2, position.z * _noiseScale * 2) * 0.1f;
            
            return combinedValue > dynamicThreshold;
        }
    }
}