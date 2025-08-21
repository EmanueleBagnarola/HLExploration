using System;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private float gridSize = 50f;
    [SerializeField] private float gridSpotSize = 3.5f;
    [SerializeField] private Transform gridContainer;
    
    [ContextMenu("Generate Grid")]
    private void GenerateGrid()
    {
        for (int x = 0; x < gridSize / 2; x++)
        {
            for (int y = 0; y < gridSize / 2; y++)
            {
                SpawnableFactory.Create<GridSlot>(gridContainer, new Vector3(x * gridSpotSize, 0 , y * gridSpotSize), Quaternion.identity);
            }
        }
    }
}