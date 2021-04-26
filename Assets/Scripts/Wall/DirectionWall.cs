using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionWall : MonoBehaviour, IPoolable
{
    #region ObjectPool

    public GameObject GameObject { get; set; }

    public ObjectPool.Pool Pool { get; set; }

    public void OnAquire()
    {
        GameObject = this.gameObject;
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    #endregion

    [SerializeField] private Direction _direction;
    public Direction _Direction => _direction;

    [SerializeField] private GameObject _wallUnit;
    public GameObject _WallUnit => _wallUnit;

    public enum Direction
    {
        up, 
        down, 
        left, 
        right
    }

    private const float PI = Mathf.PI,
                        UNIT_Y_COORDINATE = 0.51f;

    private Vector3 eulerAnglesDesired, step, unitScale;
    private ObjectPool.Pool poolTransferer = new ObjectPool.Pool();
    
    private void SetRotation(Transform wall)
    {
        wall.localEulerAngles = eulerAnglesDesired;
    }

    private void CalculateDirection(float angle)
    {
        float unitLength = unitScale.z;

        if (angle > -45 && angle <= 45)
        {
            //right swipe
            eulerAnglesDesired = new Vector3(0, 180, 0);
            _direction = Direction.right;
            step = new Vector3(0, 0, unitLength);
        }
        else if (angle > 45 && angle <= 135)
        {
            //up swipe
            eulerAnglesDesired = new Vector3(0, 90, 0);
            _direction = Direction.up;
            step = new Vector3(-unitLength, 0, 0);
        }
        else if (angle > 135 || angle <= -135)
        {
            //left swipe
            eulerAnglesDesired = new Vector3(0, 0, 0);
            _direction = Direction.left;
            step = new Vector3(0, 0, -unitLength);
        }
        else if (angle < -45 && angle >= -135)
        {
            //down swipe
            eulerAnglesDesired = new Vector3(0, 270, 0);
            _direction = Direction.down;
            step = new Vector3(unitLength, 0, 0);
        }
    }

    private void SetWallUnit(Transform wall)
    {
        wall.parent = transform;
        wall.localScale = unitScale;
        wall.localScale = _wallUnit.transform.localScale;
        SetRotation(wall);
    }

    public void BuildBetween(Vector3 firstCoordinate, Vector3 secondCoordinate)
    {
        float xAbs = Mathf.Abs(secondCoordinate.x - firstCoordinate.x);
        float zAbs = Mathf.Abs(secondCoordinate.z - firstCoordinate.z);
        float length = xAbs > zAbs ? xAbs : zAbs;
        int amountOfUnits = (int)(length / 0.5f);

        float angle = Mathf.Atan2(secondCoordinate.z - firstCoordinate.z, secondCoordinate.x - firstCoordinate.x) * 180 / PI;
        CalculateDirection(angle);

        transform.position = new Vector3(firstCoordinate.x, 0, firstCoordinate.z);

        Transform firstWallUnit = poolTransferer.Aquire(_wallUnit).transform;
        SetWallUnit(firstWallUnit);

        Vector3 nextPosition = firstWallUnit.localPosition = new Vector3(0, UNIT_Y_COORDINATE, 0);

        for(int a = 0; a < amountOfUnits - 1; a++)
        {
            Transform wallUnit = poolTransferer.Aquire(_wallUnit).transform;
            SetWallUnit(wallUnit);

            nextPosition += step;
            wallUnit.localPosition = new Vector3(nextPosition.x, UNIT_Y_COORDINATE, nextPosition.z);
        }
    }

    private void Awake()
    {
        unitScale = _wallUnit.transform.localScale;
    }
}
