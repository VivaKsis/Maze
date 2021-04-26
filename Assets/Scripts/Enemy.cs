using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private List<Vector3> _patrolPoints;
    public List<Vector3> _PatrolPoints => _patrolPoints;

    [SerializeField] private Vision _enemyVision;
    public Vision _EnemyVision => _enemyVision;

    [SerializeField] private Movement _movement;
    public Movement _Movement => _movement;

    private Player player;
    private int currentPoint;
    private Vector3 nextPoint;

    private void SetNextPoint()
    {
        currentPoint = (currentPoint + 2) > _patrolPoints.Count ? 0 : currentPoint + 1;
        nextPoint = _patrolPoints[currentPoint];

        _movement.MoveTo(nextPoint);
    }


    private void Awake()
    {
        player = FindObjectOfType<Player>();

        _enemyVision.SetOnPlayerCaught(() => { player.GetCaught(); });

        _movement.SetOnPointReached(SetNextPoint);
        _movement.SetOnTurnFinished(() => { _movement.StartMoving(); });
    }

    private void Start()
    {
        currentPoint = 1;
        nextPoint = _patrolPoints[currentPoint];

        _movement.MoveTo(nextPoint);
    }
}
