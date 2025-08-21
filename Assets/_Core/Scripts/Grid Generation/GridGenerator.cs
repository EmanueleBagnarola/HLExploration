using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Generation settings")]
    [SerializeField] private int _gridSize = 50;
    [SerializeField] private float _gridSlotDistance = 3.5f;
    [SerializeField] private Transform _gridContainer;
    [SerializeField] private Transform _cameraTransform;
    
    private List<GridSlot> _gridSlots = new List<GridSlot>();
    
    // Path editing
    private HashSet<GridSlot> _selectedSlots = new HashSet<GridSlot>();
    private Dictionary<GridSlot, List<GridSlot>> _connections = new Dictionary<GridSlot, List<GridSlot>>();
    private bool _drawing = false;
    
    [Header("Debug")]
    [SerializeField] private GridPathGraph _debugPathGraph;
    
    private GridSlot[,] _gridMatrix;

    private void Start()
    {
        EventsHandler.OnGenerateButton?.AddListener(GenerateNewGrid);
        EventsHandler.OnStartEditGridButton?.AddListener(StartPathEditing);
        EventsHandler.OnStopEditGridButton?.AddListener(StopPathEditing);
    }

    private async UniTask GenerateGrid(List<GridSlot> loadedGridSlots = null)
    {
        ClearGridContainer();
        
        _gridSlots.Clear();
        _selectedSlots.Clear();
        _connections.Clear();
        
        _gridMatrix = new GridSlot[_gridSize, _gridSize];

        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                if (loadedGridSlots != null)
                {
                    if (!FoundLoadedSlot(loadedGridSlots, new Vector2Int(x, y)))
                    {
                        continue;
                    }
                }
                
                Vector3 pos = new Vector3(x * _gridSlotDistance, 0, y * _gridSlotDistance);
                
                GridSlot slot = SpawnableFactory.Create<GridSlot>(_gridContainer, pos, Quaternion.identity);
                slot.OnClick?.AddListener(OnSlotClicked);

                slot.InitForGeneration(new Vector2Int(x, y));

                _gridSlots.Add(slot);
                
                _gridMatrix[x, y] = slot;
            }
        }
        
        Vector3 gridCenter = new Vector3(
            (_gridSize - 1) * _gridSlotDistance * 0.5f,
            0,
            (_gridSize - 1) * _gridSlotDistance * 0.5f
        );
        
        _cameraTransform.position = new Vector3(gridCenter.x, gridCenter.x * 2, gridCenter.z);

        await UniTask.DelayFrame(1);

        GenerateNeighboursReference();
    }

    private bool FoundLoadedSlot(List<GridSlot> loadedGridSlots, Vector2Int position)
    {
        for (int i = 0; i < loadedGridSlots.Count; i++)
        {
            if (loadedGridSlots[i].GridPosition == position)
                return true;
        }
        
        return false;
    }

    private async void GenerateNeighboursReference()
    {
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                GridSlot up    = (y < _gridSize - 1) ? _gridMatrix[x, y + 1] : null;
                GridSlot down  = (y > 0)              ? _gridMatrix[x, y - 1] : null;
                GridSlot left  = (x > 0)              ? _gridMatrix[x - 1, y] : null;
                GridSlot right = (x < _gridSize - 1) ? _gridMatrix[x + 1, y] : null;

                _gridMatrix[x, y].SetConnections(up, down, left, right);
            }
        }
        
        await UniTask.DelayFrame(1);
        
        StartPathEditing();
    }
    
    private void LoadGraph(GridPathGraph graph)
    {
        List<GridSlot> loadedSlots = new List<GridSlot>();
        
        foreach (var node in graph.nodes)
        {
            loadedSlots.Add(SpawnableFactory.Create<GridSlot>(_gridContainer, new Vector3(node.position.x, 0, node.position.y), Quaternion.identity));
        }
        
        _ = GenerateGrid(loadedSlots);
    }
    
    private void RemoveSlot(GridSlot slot)
    {
        _gridSlots.Remove(slot);
    }
    
    private void OnSlotClicked(GridSlot slot)
    {
        if (!_drawing) return;

        if (slot.IsSelected)
        {
            RemoveConnections(slot);
        }
        else
        {
            if (CanSelectSlot(slot))
            {
                slot.SetSelected(true);
                _selectedSlots.Add(slot);
            }
        }
    }
    
    private bool CanSelectSlot(GridSlot slot)
    {
        // Primo slot sempre consentito
        if (_selectedSlots.Count == 0)
        { 
            return true;
        }
        
        // Se ha un vicino selezionato, può essere aggiunto
        foreach (var other in _selectedSlots)
        {
            if (Mathf.Approximately(Vector2Int.Distance(other.GridPosition, slot.GridPosition), 1))
            {
                return true;
            }
        }

        return false;
    }

    private void EnsureConnections(GridSlot slot)
    {
        if (!_connections.ContainsKey(slot))
            _connections[slot] = new List<GridSlot>();

        foreach (var selectedSlot in _selectedSlots)
        {
            if (selectedSlot == slot) continue;
        
            if (Mathf.Approximately(Vector2Int.Distance(selectedSlot.GridPosition, slot.GridPosition), 1))
            {
                if (!_connections[slot].Contains(selectedSlot))
                    _connections[slot].Add(selectedSlot);
        
                if (!_connections.ContainsKey(selectedSlot))
                    _connections[selectedSlot] = new List<GridSlot>();
        
                if (!_connections[selectedSlot].Contains(slot))
                    _connections[selectedSlot].Add(slot);
            }
        }
    }

    private void SaveGraph()
    {
        // GridPathGraph graph = ScriptableObject.CreateInstance<GridPathGraph>();
        //
        // foreach (var kvp in connections)
        // {
        //     Debug.Log($"SaveGraph | {kvp.Key} | connections: {kvp.Value.Count}");
        //
        //     var node = new GridPathGraph.Node
        //     {
        //         position = kvp.Key.GridPosition
        //     };
        //
        //     foreach (var neighbor in kvp.Value)
        //         node.connectedTo.Add(neighbor.GridPosition);
        //
        //     graph.nodes.Add(node);
        // }
        //
        // string pathToSave = EditorUtility.SaveFilePanelInProject(
        //     "Save Grid Path Graph",
        //     "NewGridPathGraph",
        //     "asset",
        //     "Choose where to save the path graph"
        // );
        //
        // if (!string.IsNullOrEmpty(pathToSave))
        // {
        //     AssetDatabase.CreateAsset(graph, pathToSave);
        //     AssetDatabase.SaveAssets();
        //     Debug.Log($"Path graph saved at {pathToSave}");
        // }
    }

    private void ResetSlots()
    {
        foreach (var slot in _gridSlots)
            slot.SetSelected(false);
    }
    
    private async void DestroyUnusedSlots()
    {
        for (int i = _gridSlots.Count - 1; i >= 0; i--)
        {
            var slot = _gridSlots[i];
            if (!slot.IsSelected)
            {
                Destroy(slot.gameObject);
                RemoveSlot(slot);
            }
        }
        
        await UniTask.DelayFrame(1);

        for (int i = 0; i < _gridSlots.Count; i++)
        {
            _gridSlots[i].Confirm();
        }
        
        Debug.Log("Removed all unused slots");
    }
    
    private void RemoveConnections(GridSlot slot)
    {
        if (!_connections.TryGetValue(slot, out var connection)) return;

        foreach (var neighbor in connection)
        {
            _connections[neighbor].Remove(slot);
        }

        _connections[slot].Clear();
        Debug.Log($"Removed all connections for {slot.name}");
    }

    private void ClearGridContainer()
    {
        for (int i = 0; i < _gridContainer.childCount; i++)
        {
            Destroy(_gridContainer.GetChild(i).gameObject);
        }
    }

    #region Path Editing Buttons

    [Button]
    public void GenerateNewGrid()
    {
        GenerateGrid();
    }
    
    [Button]
    public void StartPathEditing()
    {
        _drawing = true;
        _selectedSlots.Clear();
        _connections.Clear();
        ResetSlots();

        for (int i = 0; i < _gridSlots.Count; i++)
        {
            _gridSlots[i].SetSelected(false);
        }
    }

    [Button]
    public void StopPathEditing()
    {
        _drawing = false;
            
        foreach (GridSlot slot in _selectedSlots)
        {
            EnsureConnections(slot);
        }
            
        DestroyUnusedSlots();
    }

    [Button]
    public void SavePath()
    {
        SaveGraph();
    }
    #endregion
    
    [ContextMenu("Debug Load")]
    public void Debug_LoadGraph()
    {
        LoadGraph(_debugPathGraph);
    }
}