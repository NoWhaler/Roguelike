using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.WorldGeneration.Hex
{
    public class HexMouseDetector: IInitializable, ITickable
    {
        private Camera _mainCamera;
        private const float HEX_RADIUS = 7f;

        private readonly List<HexModel> _allHexes = new List<HexModel>();

        public UnityEvent<HexModel> OnHexagonHovered;
        public UnityEvent<HexModel> OnHexagonUnhovered;
        public UnityEvent<HexModel> OnHexagonClicked;

        private HexModel _currentHoveredHex;

        public void Initialize()
        {
            _mainCamera = Camera.main;

            OnHexagonHovered ??= new UnityEvent<HexModel>();
            OnHexagonUnhovered ??= new UnityEvent<HexModel>();
            OnHexagonClicked ??= new UnityEvent<HexModel>();
        }

        public void Tick()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            var hitHex = DetectHexWithPointInHexagon(ray);
            HandleHexInteraction(hitHex);
        }
        
        private HexModel DetectHexWithPointInHexagon(Ray ray)
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 worldPoint = ray.GetPoint(enter);
                
                HexModel closestHex = null;
                float closestDistance = float.MaxValue;

                foreach (var hex in _allHexes)
                {
                    if (IsPointInHexagon(worldPoint, hex.HexPosition, HEX_RADIUS))
                    {
                        float distance = Vector3.Distance(worldPoint, hex.HexPosition);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestHex = hex;
                        }
                    }
                }

                return closestHex;
            }
            return null;
        }

        private bool IsPointInHexagon(Vector3 point, Vector3 hexCenter, float radius)
        {
            Vector2 point2D = new Vector2(point.x, point.z);
            Vector2 center2D = new Vector2(hexCenter.x, hexCenter.z);

            Vector2 offset = point2D - center2D;

            float q = (2f/3 * offset.x) / radius;
            float r = (-1f/3 * offset.x + Mathf.Sqrt(3)/3 * offset.y) / radius;
            float s = -q - r;

            float q_rounded = Mathf.Round(q);
            float r_rounded = Mathf.Round(r);
            float s_rounded = Mathf.Round(s);

            float q_diff = Mathf.Abs(q_rounded - q);
            float r_diff = Mathf.Abs(r_rounded - r);
            float s_diff = Mathf.Abs(s_rounded - s);

            if (q_diff > r_diff && q_diff > s_diff)
            {
                q_rounded = -r_rounded - s_rounded;
            }
            else if (r_diff > s_diff)
            {
                r_rounded = -q_rounded - s_rounded;
            }

            return q_rounded == 0 && r_rounded == 0;
        }

        private void HandleHexInteraction(HexModel hitHex)
        {
            if (hitHex != _currentHoveredHex)
            {
                if (_currentHoveredHex != null)
                {
                    _currentHoveredHex.SetOutline();
                    OnHexagonUnhovered.Invoke(_currentHoveredHex);
                }

                if (hitHex != null)
                {
                    hitHex.SetOutline();
                    OnHexagonHovered.Invoke(hitHex);
                }

                _currentHoveredHex = hitHex;
            }

            if (hitHex != null && Input.GetMouseButtonDown(0))
            {
                OnHexagonClicked.Invoke(hitHex);
            }
        }

        public void SetHexes(ref HexModel hex)
        {
            _allHexes.Add(hex);
        }
    }
}