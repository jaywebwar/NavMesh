using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] RectTransform selectionBox;

    List<RTSUnit> _selectedUnits;
    NavMeshAgent _nma;

    void Awake()
    {
        _selectedUnits = new List<RTSUnit>();
        selectionBox.sizeDelta = Vector2.zero;
        _nma = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Begin Selection
        if(Input.GetMouseButtonDown(0))
        {
            foreach (var unit in _selectedUnits)
                unit.DeSelectUnit();
            _selectedUnits.Clear();

            selectionBox.position = Input.mousePosition;
            selectionBox.sizeDelta = Vector2.zero;//sizeDelta holds rect transform width and height info
            Debug.Log("We've started a selection box!");
        }
        //Drag selection box
        if (Input.GetMouseButton(0))
        {
            float width = Input.mousePosition.x - selectionBox.position.x;
            float height = -1*(Input.mousePosition.y - selectionBox.position.y);

            selectionBox.sizeDelta = new Vector2(width, height);
        }
        //End Selection
        if (Input.GetMouseButtonUp(0))
        {
            //Find center of selection box (average of the two corners)
            Vector2 originOfSelectionBoxOnCanvas = new Vector2(selectionBox.position.x, selectionBox.position.y);
            Vector2 centerOfSelectionBoxOnCanvas = new Vector2(selectionBox.position.x + selectionBox.sizeDelta.x/2f, selectionBox.position.y - selectionBox.sizeDelta.y/2f);

            Ray originRay = cam.ScreenPointToRay(originOfSelectionBoxOnCanvas);
            Ray centerRay = cam.ScreenPointToRay(centerOfSelectionBoxOnCanvas);

            Vector3 originInWorld;
            Vector3 centerInWorld;

            if(Physics.Raycast(originRay, out RaycastHit originHit))
            {
                Debug.Log("OriginHit successful raycast!");
                originInWorld = originHit.transform.position;
                _nma.SetDestination(originInWorld);
                Debug.Log("Origin in World = " + originInWorld);

                Debug.DrawLine(cam.transform.position, originInWorld, Color.red);

                if (Physics.Raycast(centerRay, out RaycastHit centerHit))
                {
                    Debug.Log("CenterHit successful raycast!");
                    centerInWorld = centerHit.transform.position;
                    Debug.Log("Center in World = " + centerInWorld);

                    Debug.DrawLine(cam.transform.position, centerInWorld, Color.cyan);

                    Vector3 halfExtents = new Vector3(centerInWorld.x - originInWorld.x, 0f, centerInWorld.z - originInWorld.z);
                    Debug.Log("Half extents = " + halfExtents);

                    //cast over area to get all selected units
                    foreach (var hit in Physics.BoxCastAll(centerInWorld, halfExtents, transform.forward))
                    {
                        if(hit.collider.gameObject.GetComponent<RTSUnit>())
                        {
                            if(hit.collider.gameObject.GetComponent<IRTSUnit>().IsPlayersUnit)
                            {
                                _selectedUnits.Add(hit.collider.gameObject.GetComponent<RTSUnit>());
                            }
                        }
                    }
                    //show units as selected
                    foreach (var unit in _selectedUnits)
                    {
                        unit.SelectUnit();
                    }
                }
            }
            

            
            //add to selectedUnits
            //have selected units show circle
            //reset selectionBox
            selectionBox.sizeDelta = Vector2.zero;
        }
    }

    
}
