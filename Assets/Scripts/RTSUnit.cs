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
    enum AttackState
    {
        Follow,
        Wait,
        Attack
    };
    AttackState currentState = AttackState.Follow;


    NavMeshAgent _navMeshAgent;
    Animator _animator;
    RTSUnitCommandProcessor _commandProcessor;
    [SerializeField] bool isPlayersUnit = false;
    [SerializeField] GameObject _selectionCircle;
    [SerializeField] float attackRange;
    [SerializeField] float attackRate;
    bool isSelected = false;

    event Action OnSelectionChanged; 

    public bool IsPlayersUnit { get; set; }
    public float AttackRange { get; private set; }
    public bool ReadyToAttack { get; private set; }
    public bool InAttackRange { get; internal set; }
    public bool IsInAttackingState { get;  set; }
    public float AttackRate { get; private set; }

    void Awake()
    {
        _commandProcessor = GetComponent<RTSUnitCommandProcessor>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        IsPlayersUnit = isPlayersUnit;
        _selectionCircle.SetActive(isSelected);
        _animator = GetComponent<Animator>();
        AttackRange = attackRange;
        AttackRate = attackRate;
    }

    public void EnterAttackingState(RTSUnit enemy)
    {
        Debug.Log("Enter Attack State Machine");
        currentState = AttackState.Follow;
        InAttackRange = false;
        StartCoroutine(AttackStateMachine(enemy));
    }

    IEnumerator AttackStateMachine(RTSUnit enemy)
    {
        while (IsInAttackingState)
        {
            //State machine
            switch (currentState)
            {
                case AttackState.Follow:
                    Debug.Log("Follow State");
                    HandleMovement(enemy.transform.position);
                    if (InAttackRange)
                    {
                        currentState = AttackState.Attack;
                    }
                    break;
                case AttackState.Wait:
                    Debug.Log("Wait State");
                    yield return new WaitForSeconds(AttackRate);
                    if (!InAttackRange)
                    {
                        currentState = AttackState.Follow;
                    }
                    else
                    {
                        currentState = AttackState.Attack;
                    }
                    ReadyToAttack = true;
                    break;
                case AttackState.Attack:
                    Debug.Log("Attack State");
                    Attack(enemy);
                    currentState = AttackState.Wait;
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }

    public void HandleMovement(Vector3 destination)
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(destination);
        //This method is only ended when _navMeshAgent.isStopped = true.
        // or the command is interrupted.
    }

    void OnEnable() => OnSelectionChanged += SetSelectionCircleVisibility;
    void OnDisable() => OnSelectionChanged -= SetSelectionCircleVisibility;

    void Update()
    {
        AttackRange = attackRange;//remove once range is chosen
        AttackRate = attackRate;//remove once rate is chosen

        //End of handle movement
        if(_navMeshAgent.remainingDistance <= 0.1f)
        {
            _navMeshAgent.isStopped = true;
        }
        //End of handle attack movement
        if(_navMeshAgent.remainingDistance <= AttackRange)
        {
            if(IsInAttackingState)
            {
                _navMeshAgent.isStopped = true;
            }
            InAttackRange = true;
        }
        else
        {
            InAttackRange = false;
        }

        if (_navMeshAgent.isStopped)
            _animator.SetBool("isMoving", false);
        else
            _animator.SetBool("isMoving", true);
    }

    public void Attack(RTSUnit enemy)
    {
        _animator.SetTrigger("Attack");
        transform.forward = (enemy.transform.position - transform.position).normalized;
        ReadyToAttack = false;
    }

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

    public void AddMovementCommand(Vector3 point, bool shouldQueueCommands)
    {
        MovementCommand mveCmd = new MovementCommand(_commandProcessor, point, _navMeshAgent, !shouldQueueCommands);
    }

    public void AddAttackCommand(RTSUnit target, bool shouldQueueCommands)
    {
        Debug.Log("Create the attack command");
        AttackCommand atkCmd = new AttackCommand(_commandProcessor, _navMeshAgent, target, !shouldQueueCommands);
    }

    public IEnumerator FlashUnit()
    {
        _selectionCircle.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _selectionCircle.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        _selectionCircle.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        _selectionCircle.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        if (isSelected)
        {
            _selectionCircle.SetActive(true);
        }
    }
}


