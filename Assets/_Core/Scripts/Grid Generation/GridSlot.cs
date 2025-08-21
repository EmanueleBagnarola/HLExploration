using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridSlot : MonoBehaviour, ISpawnable
{
    public UnityEvent<GridSlot> OnClick;
    
    public GridSlot Up => _up;
    public GridSlot Down => _down;
    public GridSlot Left => _left;

    public GridSlot Right => _right;
    
    public Vector2Int GridPosition { get; private set; }
    
    public bool IsSelected { get; private set; }

    [Header("Neighbours slots")]
    [SerializeField] private GridSlot _up;
    [SerializeField] private GridSlot _down;
    [SerializeField] private GridSlot _left;
    [SerializeField] private GridSlot _right;
    
    [Header("Generation settings")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material notSelectedMaterial;
    [SerializeField] private Material selectedMaterial;

    [Header("General settings")]
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private List<GridSlotDirectionIndicator> directionIndicators = new List<GridSlotDirectionIndicator>();
    
    public void InitForGeneration(Vector2Int pos)
    {
        GridPosition = pos;
        IsSelected = false;
        name = $"GridSlot_{pos.x}_{pos.y}";
        renderer.material = notSelectedMaterial;
        HideDirectionIndicators();
    }
    
    public void SetConnections(GridSlot up, GridSlot down, GridSlot left, GridSlot right)
    {
        _up = up;
        _down = down;
        _left = left;
        _right = right;
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        
        renderer.material = IsSelected ? selectedMaterial : notSelectedMaterial;
    }

    public void Confirm()
    {
        IsSelected = true;
        renderer.material = defaultMaterial;
        
        ShowIndicator(Direction.Up, _up != null);
        ShowIndicator(Direction.Down, _down != null);
        ShowIndicator(Direction.Right, _right != null);
        ShowIndicator(Direction.Left, _left != null);
    }

    private void OnMouseDown()
    {
        OnClick.Invoke(this);
    }

    private void HideDirectionIndicators()
    {
        for (int i = 0; i < directionIndicators.Count; i++)
        {
            directionIndicators[i].ShowIndicator(false);
        }
    }

    private void ShowIndicator(Direction direction, bool show)
    {
        for (int i = 0; i < directionIndicators.Count; i++)
        {
            if (direction == directionIndicators[i].Direction)
            {
                directionIndicators[i].ShowIndicator(show);
            }
        }
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class GridSlotData
{
    public Vector3 Position;
    public GridSlotData(Vector3 position)
    {
        Position = position;   
    }
}

[System.Serializable]
public class GridSlotDirectionIndicator
{
    public Direction Direction => _direction;
    
    [SerializeField] private Direction _direction;
    [SerializeField] private GameObject _indicator;

    public void ShowIndicator(bool _show)
    {
        _indicator.SetActive(_show);
    }
}
