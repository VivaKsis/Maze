using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [SerializeField] private Transform _firstRaycastPoint;
    public Transform _FirstRaycastPoint => _firstRaycastPoint;

    [SerializeField] private List<Transform> _checkWallRaycastPoints;
    public List<Transform> _CheckWallPoints => _checkWallRaycastPoints;

    [SerializeField] private List<Transform> _checkPlayerRaycastPoints;
    public List<Transform> _OuterRaycastPoints => _checkPlayerRaycastPoints;

    [SerializeField] private LayerMask _maskPlayer;
    public LayerMask _MaskPlayer => _maskPlayer;

    [SerializeField] private LayerMask _maskWall;
    public LayerMask _MaskWall => _maskWall;

    private Action onPlayerCaught;
    private Action<GameObject> onFaceWall;

    public void SetOnPlayerCaught(Action action)
    {
        onPlayerCaught = action;
    }

    public void SetOnFaceWall(Action<GameObject> action)
    {
        onFaceWall = action;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int a = 0; a < _checkPlayerRaycastPoints.Count; a++)
        {
            Gizmos.DrawLine(_firstRaycastPoint.position, _checkPlayerRaycastPoints[a].position);
        }

        Gizmos.color = Color.black;
        for (int a = 0; a < _checkWallRaycastPoints.Count; a++)
        {
            Gizmos.DrawLine(_firstRaycastPoint.position, _checkWallRaycastPoints[a].position);
        }
    }

    private void Update()
    {
        for (int a = 0; a < _checkPlayerRaycastPoints.Count; a++)
        {
            if (Physics.Linecast(_firstRaycastPoint.position, _checkPlayerRaycastPoints[a].position, _maskPlayer))
            {
                onPlayerCaught?.Invoke();
            }
        }

        RaycastHit hitInfo;

        for (int a = 0; a < _checkWallRaycastPoints.Count; a++)
        {
            if (Physics.Linecast(_firstRaycastPoint.position, _checkWallRaycastPoints[a].position, out hitInfo, _maskWall))
            {
                onFaceWall?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }
}
