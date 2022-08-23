using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] RectTransform selectionBox;
    [SerializeField] Transform worldSelectionBoxTransform;
    [SerializeField] Transform angledSelectionBoxTransform;

    RTSFormation _formation;

    //private Matrix4x4 originalGizmoMatrix;
    //private bool shouldDrawGizmo;
    private Vector3 selectionBoxSize;
    private Vector3 selectionBoxCorner;
    private Vector3 selectionBoxCenter;
    private Vector3 originOnCanvas;
    private Vector3 selectionBoxOrigin;

    //Masks so that only layer 6 is selectable. If I wanted to make everything but 6 selectable I should follow up this line with
    // layerMask = ~layermask;
    int unitMask = 1 << 6;
    private float angle;
    private int groundMask = 1 << 7;

    void Awake()
    {
        _formation = GetComponent<RTSFormation>();
        selectionBox.sizeDelta = Vector2.zero;

        //originalGizmoMatrix = Gizmos.matrix;
    }

    //Draws in scene the selection box cast.
    //void OnDrawGizmos()
    //{
    //    if (shouldDrawGizmo)
    //    {
    //        angledSelectionBoxTransform.position = selectionBoxCenter;
    //        worldSelectionBoxTransform.position = selectionBoxCenter;

    //        Gizmos.matrix = angledSelectionBoxTransform.localToWorldMatrix;
    //        Gizmos.color = Color.blue;
    //        //Gizmos.DrawCube(Vector3.zero, angledBoxSize);


    //        Gizmos.matrix = worldSelectionBoxTransform.localToWorldMatrix;
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(Vector3.zero, selectionBoxSize);

    //        Gizmos.matrix = originalGizmoMatrix;

    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawLine(selectionBoxOrigin, selectionBoxCorner);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        //Begin Selection
        if (Input.GetMouseButtonDown(0))
        {
            _formation.DeSelectAllUnits();

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                originOnCanvas = Input.mousePosition;
                selectionBox.position = originOnCanvas;
                selectionBoxOrigin = hit.point;
            }
            selectionBox.sizeDelta = Vector2.zero;//sizeDelta holds rect transform width and height info
            selectionBoxSize = Vector3.zero;
            //Debug.Log("We've started a selection box!");
        }
        //Drag selection box
        if (Input.GetMouseButton(0))
        {
            float width = Input.mousePosition.x - originOnCanvas.x;
            float height = -1 * (Input.mousePosition.y - originOnCanvas.y);
            angle = Mathf.Atan(height / width);

            selectionBox.sizeDelta = new Vector2(width, height);

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                selectionBoxCorner = hit.point;
                selectionBoxCenter = (selectionBoxCorner + selectionBoxOrigin) / 2;
                float hypot = Vector3.Distance(selectionBoxOrigin, selectionBoxCorner);
                if (hypot > 0.1f)
                {
                    //float x = Mathf.Cos(angle) * hypot;
                    //float z = Mathf.Sqrt(Mathf.Pow(hypot, 2f) - Mathf.Pow(x, 2f));

                    //Debug.Log("Old X = " + x);
                    //Debug.Log("Old Z = " + z);

                    Vector3 cameraPlaneForward = cam.transform.parent.forward;
                    Vector3 hypotDirection = (selectionBoxCorner - selectionBoxOrigin);
                    Debug.Log("Before normalization = " + hypotDirection);
                    hypotDirection.Normalize();
                    Debug.Log("After normalization = " + hypotDirection);

                    float angleBetweenHypotAndCamFwd = Mathf.Acos((Vector3.Dot(hypotDirection, cameraPlaneForward) / (hypotDirection.magnitude * cameraPlaneForward.magnitude)));
                    float x2 = Mathf.Abs(Mathf.Sin(angleBetweenHypotAndCamFwd) * hypot);
                    float z2 = Mathf.Abs(Mathf.Cos(angleBetweenHypotAndCamFwd) * hypot);

                    Debug.Log("New X = " + x2);
                    Debug.Log("New Z = " + z2);


                    //angledBoxSize = new Vector3(x, angledY, 4f);
                    selectionBoxSize = new Vector3(x2, .2f, z2);
                }

                //shouldDrawGizmo = true;
            }


        }
        //End Selection
        if (Input.GetMouseButtonUp(0))
        {

            _formation.DeSelectAllUnits();

            //Player simply clicked
            if (selectionBoxSize.x < 0.5f && selectionBoxSize.y < 0.5f)
            {
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, unitMask))
                {
                    if (hit.collider.gameObject.GetComponent<RTSUnit>())
                    {
                        _formation.Add(hit.collider.gameObject.GetComponent<RTSUnit>());
                    }
                }
            }
            else
            {
                //This code below will instantiate a cube with the same dimensions as the BoxCastAll further below
                //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                //cube.position = selectionBoxCenter;
                //cube.localScale = selectionBoxSize;
                //cube.up = cam.transform.parent.up;
                //cube.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
                //Destroy(cube.GetComponent<BoxCollider>());
                //cube.name = "DEBUG BOX CAST BOX";


                //Add all units in the selection box to List of selected units.
                foreach (var hit in Physics.BoxCastAll(selectionBoxCenter, selectionBoxSize/2, cam.transform.parent.up, Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f), Mathf.Infinity, unitMask))
                {
                    if (hit.collider.gameObject.GetComponent<RTSUnit>())
                    {
                        if (_formation.SelectedUnits.Count == 0)
                        {
                            _formation.Add(hit.collider.gameObject.GetComponent<RTSUnit>());
                        }
                        else if (hit.collider.gameObject.GetComponent<IRTSUnit>().IsPlayersUnit)
                        {
                            _formation.Add(hit.collider.gameObject.GetComponent<RTSUnit>());
                        }
                    }
                }
                if(_formation.SelectedUnits.Count > 1)
                {
                    if(!_formation.SelectedUnits[0].IsPlayersUnit)
                    {
                        _formation.Remove(0);
                    }
                }
            }
            //show units as selected
            _formation.ShowSelected();


            //reset selectionBox
            selectionBox.sizeDelta = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if(_formation.SelectedUnits.Count != 0)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitMask))
                {
                    if(hit.collider.gameObject.GetComponent<IRTSUnit>().IsPlayersUnit)
                    {
                        Debug.Log("Right clicked an allied unit.");
                        StartCoroutine(hit.collider.gameObject.GetComponent<RTSUnit>().FlashUnit());
                    }
                    else
                    {
                        Debug.Log("Right clicked an enemy unit.");
                        StartCoroutine(hit.collider.gameObject.GetComponent<RTSUnit>().FlashUnit());
                        _formation.AddAttackCommand(hit.collider.gameObject.GetComponent<RTSUnit>(), Input.GetKey(KeyCode.LeftShift));
                    }
                
                }
                else if(Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, groundMask))
                {
                    Debug.Log("Clicking to move");
                    _formation.AddMovementCommand(hit2.point, Input.GetKey(KeyCode.LeftShift));
                }


            }
        }
    }
}
