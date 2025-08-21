using System;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerationGUI : MonoBehaviour
{
    [SerializeField] private Button _generateButton;
    [SerializeField] private Button _startEditButton;
    [SerializeField] private Button _stopEditButton;

    private void Start()
    {
        _generateButton.onClick.AddListener(()=>
        {
            EventsHandler.OnGenerateButton?.Invoke();
        });
        
        _startEditButton.onClick.AddListener(() =>
        {
            EventsHandler.OnStartEditGridButton?.Invoke();
        });
        
        _stopEditButton.onClick.AddListener(() =>
        {
            EventsHandler.OnStopEditGridButton?.Invoke();
        });
    }
}
