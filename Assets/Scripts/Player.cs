using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _nav;
    public NavMeshAgent _Nav => _nav;

    [SerializeField] private Transform _destination;
    public Transform _Destination => _destination;

    [SerializeField] private Movement _movement;
    public Movement _Movement => _movement;

    [SerializeField] private UIManager _uIManager;
    public UIManager _UIManager => _uIManager;

    public void GetCaught()
    {
        _uIManager.GameOverTextShow();
    }

    public void Win()
    {
        _uIManager.YouWonTextShow();
    }

    private void Start()
    {
        _movement.MoveFurther(transform.position + transform.forward);
    }
}
