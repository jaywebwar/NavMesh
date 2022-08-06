using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(RTSUnitCommandProcessor))]
[RequireComponent(typeof(NavMeshAgent))]
public class RTSUnit : MonoBehaviour, IRTSUnit
{
    NavMeshAgent _navMeshAgent;
    RTSUnitCommandProcessor _commandProcessor;
    [SerializeField] bool isPlayersUnit = false;
    [SerializeField] GameObject _selectionCircle;
    private bool isSelected = false;

    event Action OnSelectionChanged; 

    public bool IsPlayersUnit { get; set; }

    void Awake()
    {
        _commandProcessor = GetComponent<RTSUnitCommandProcessor>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        IsPlayersUnit = isPlayersUnit;
        _selectionCircle.SetActive(isSelected);
    }

    void OnEnable() => OnSelectionChanged += SetSelectionCircleVisibility;
    void OnDisable() => OnSelectionChanged -= SetSelectionCircleVisibility;

    void SetSelectionCircleVisibility() => _selectionCircle.SetActive(isSelected);

    public void SelectUnit()
    {
        isSelected = true;
        OnSelectionChanged?.Invoke();
    }

    public void DeSelectUnit()
    {
        isSelected = false;
        OnSelectionChanged?.Invoke();
    }
}

