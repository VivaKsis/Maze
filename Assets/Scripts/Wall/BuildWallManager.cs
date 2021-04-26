using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWallManager : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab;
    public GameObject _WallPrefab => _wallPrefab;
    [SerializeField] private GameObject _wallParent;
    public GameObject _WallParent => _wallParent;

    [SerializeField] private RaycastEmitter _raycastEmitter;
    public RaycastEmitter _RaycastEmitter => _raycastEmitter;

    private GameObject wall;
    private RaycastHit _raycastHit;
    private Vector3 firstCoordinate, secondCoordinate;
    private ObjectPool.Pool poolTransferer = new ObjectPool.Pool();

    private void BuildWall()
    {
        wall = poolTransferer.Aquire(_wallPrefab);
        wall.transform.parent = _wallParent.transform;

        wall.transform.localPosition = _wallParent.transform.localPosition;
       // wall.transform.localScale = _wallParent.transform.localScale;
        wall.transform.localEulerAngles = _wallParent.transform.localEulerAngles;

        DirectionWall directionWall = wall.GetComponent<DirectionWall>();

        directionWall.BuildBetween(firstCoordinate, secondCoordinate);
    }

    private bool CanBuild()
    {
        Vector3 distance = secondCoordinate - firstCoordinate;
        
        if(Mathf.Abs(distance.x) >= 1 || Mathf.Abs(distance.z) >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(pos: Input.mousePosition);
            if (_raycastEmitter.Raycast(ray: ray, raycastHit: out _raycastHit))
            {
                firstCoordinate = _raycastHit.point;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(pos: Input.mousePosition);
            if (_raycastEmitter.Raycast(ray: ray, raycastHit: out _raycastHit))
            {
                secondCoordinate = _raycastHit.point;
                if (CanBuild())
                {
                    BuildWall();
                }
            }
        }
    }
}
