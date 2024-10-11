using UnityEngine;
using Zenject;

namespace Game.WorldGeneration.Hex
{
    public class HexInteraction: IInitializable
    {
         private HexMouseDetector _hexMouseDetector;

         [Inject]
         private void Constructor(HexMouseDetector hexMouseDetector)
         {
             _hexMouseDetector = hexMouseDetector;
         }
             
         public void Initialize()
         {
             _hexMouseDetector.OnHexagonHovered.AddListener(HandleHexHovered);
             _hexMouseDetector.OnHexagonUnhovered.AddListener(HandleHexUnhovered);
             _hexMouseDetector.OnHexagonClicked.AddListener(HandleHexClicked);
         }
     
         private void HandleHexHovered(HexModel hexModel)
         {
             Debug.Log($"Hovering over hex at Q:{hexModel.Q}, R:{hexModel.R}, S:{hexModel.S}");
         }
         
         private void HandleHexUnhovered(HexModel hexModel)
         {
             Debug.Log($"Exited hex at Q:{hexModel.Q}, R:{hexModel.R}, S:{hexModel.S}");
         }
     
         private void HandleHexClicked(HexModel hexModel)
         {
             Debug.Log($"Clicked hex at Q:{hexModel.Q}, R:{hexModel.R}, S:{hexModel.S}");
         }   
    }
}