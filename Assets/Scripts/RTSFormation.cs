using System;
using System.Collections.Generic;
using UnityEngine;

public class RTSFormation : MonoBehaviour, IRTSFormation
{
    [SerializeField] GameObject TwoManFormation;
    [SerializeField] GameObject ThreeManFormation;
    [SerializeField] GameObject FourManFormation;
    [SerializeField] GameObject FiveManFormation;
    [SerializeField] GameObject SixManFormation;
    [SerializeField] GameObject SevenManFormation;
    [SerializeField] GameObject EightManFormation;
    [SerializeField] GameObject NineManFormation;
    [SerializeField] GameObject TenManFormation;
    [SerializeField] GameObject ElevenManFormation;
    [SerializeField] GameObject TwelveManFormation;

    public List<RTSUnit> SelectedUnits { get; private set; }

    private void Awake()
    {
        SelectedUnits = new List<RTSUnit>();
    }

    public void Add(RTSUnit unit)
    {
        SelectedUnits.Add(unit);
        Debug.Log(unit.name + " added to selection...");
    }

    public void ShowSelected()
    {
        foreach (var unit in SelectedUnits)
        {
            unit.SelectUnit();
        }
        Debug.Log("Show selected");
    }

    public void AddMovementCommand(Vector3 point, bool shouldQueueCommands)
    {
        switch (SelectedUnits.Count)
        {
            case 1:
                if (SelectedUnits[0].IsPlayersUnit)
                    SelectedUnits[0].AddMovementCommand(point, shouldQueueCommands);
                break;
            case 2:
                HandleFormationMovement(TwoManFormation, point, shouldQueueCommands);
                break;
            case 3:
                HandleFormationMovement(ThreeManFormation, point, shouldQueueCommands);
                break;
            case 4:
                HandleFormationMovement(FourManFormation, point, shouldQueueCommands);
                //foreach (var unit in _selectedUnits)
                //{
                //    unit.AddMovementCommand(point);
                //}
                break;
            case 5:
                HandleFormationMovement(FiveManFormation, point, shouldQueueCommands);
                break;
            case 6:
                HandleFormationMovement(SixManFormation, point, shouldQueueCommands);
                break;
            case 7:
                HandleFormationMovement(SevenManFormation, point, shouldQueueCommands);
                break;
            case 8:
                HandleFormationMovement(EightManFormation, point, shouldQueueCommands);
                break;
            case 9:
                HandleFormationMovement(NineManFormation, point, shouldQueueCommands);
                break;
            case 10:
                HandleFormationMovement(TenManFormation, point, shouldQueueCommands);
                break;
            case 11:
                HandleFormationMovement(ElevenManFormation, point, shouldQueueCommands);
                break;
            case 12:
                HandleFormationMovement(TwelveManFormation, point, shouldQueueCommands);
                break;
            default:
                break;
        }
    }
    public void AddAttackCommand(RTSUnit unit, bool shouldQueueCommands)
    {
        foreach(var u in SelectedUnits)
        {
            u.AddAttackCommand(unit, shouldQueueCommands);
        }
    }


    private void HandleFormationMovement(GameObject formation, Vector3 point, bool shouldQueueCommands)
    {
        //Place formation at point
        formation.transform.position = point;

        //create new lists and fill them with the formation locations and our selected units
        List<Vector3> positionsInFormation = new List<Vector3>();
        foreach (var pos in GetPositionsInFormation(formation))
            positionsInFormation.Add(pos);
        List<RTSUnit> unitsYetToBeAllocated = new List<RTSUnit>();
        foreach (var unit in SelectedUnits)
            unitsYetToBeAllocated.Add(unit);

        //compare distances among formation and remove units and formation locations until done

        //starting with the unit who has the furtherest distance, start allocating formation slots to
        //the closest available slot.
        RecursiveFormationSlotAllocation(positionsInFormation, unitsYetToBeAllocated, shouldQueueCommands);
    }

    private void RecursiveFormationSlotAllocation(List<Vector3> positionsInFormation, List<RTSUnit> unitsYetToBeAllocated, bool shouldQueueCommands)
    {
        //Are we out of units to allocate?
        if (unitsYetToBeAllocated.Count == 0)
        {
            return;
        }
        else
        {
            RTSUnit currentUnitToAllocate = null;
            float maxDistanceInFormation = 0f;
            Vector3 positionAllocated = Vector3.zero;
            foreach (var unit in unitsYetToBeAllocated)
            {
                float minDistanceInFormation = Mathf.Infinity;
                Vector3 closestPositionToThisUnit = Vector3.zero;
                foreach (var position in positionsInFormation)
                {
                    float distance = Vector3.Distance(unit.transform.position, position);
                    if (distance < minDistanceInFormation)
                    {
                        minDistanceInFormation = distance;
                        closestPositionToThisUnit = position;
                    }
                    if (distance > maxDistanceInFormation)
                    {
                        currentUnitToAllocate = unit;
                        maxDistanceInFormation = distance;
                    }
                }
                if (unit == currentUnitToAllocate)
                {
                    positionAllocated = closestPositionToThisUnit;
                }
            }
            currentUnitToAllocate.AddMovementCommand(positionAllocated, shouldQueueCommands);
            positionsInFormation.Remove(positionAllocated);
            unitsYetToBeAllocated.Remove(currentUnitToAllocate);

            RecursiveFormationSlotAllocation(positionsInFormation, unitsYetToBeAllocated, shouldQueueCommands);
        }
    }

    private List<Vector3> GetPositionsInFormation(GameObject formation)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < formation.transform.childCount; i++)
        {
            positions.Add(formation.transform.GetChild(i).position);
        }
        return positions;
    }

    public void DeSelectAllUnits()
    {
        foreach (var unit in SelectedUnits)
            unit.DeSelectUnit();
        SelectedUnits.Clear();
        Debug.Log("Deselect all units");
    }

    public void Remove(int index)
    {
        SelectedUnits.Remove(SelectedUnits[index]);
    }
}


