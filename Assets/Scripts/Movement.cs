using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    public Rigidbody _Rigidbody => _rigidbody;

    [SerializeField] private NavMeshAgent _nav;
    public NavMeshAgent _Nav => _nav;

    [SerializeField] private float _rotationSpeed;
    public float _RotationSpeed => _rotationSpeed;

    [SerializeField] private Vision _vision;
    public Vision _Vision => _vision;

    private const float ROTATION_TOLERANCE = 1f,
                        POINT_REACHED_TOLERANCE = 0.1f;

    public State _State;
    public Direction movementDirection;

    private NavMeshPath path;
    private Action onPointReached, onTurnFinished;
    private Vector3 destination, rotationDirection;

    public enum State
    {
        moving,
        stop,
        turning,
        agentControl
    }

    public enum Direction
    {
        up,
        down,
        left,
        right
    }

    public void SetOnPointReached(Action action)
    {
        onPointReached = action;
    }
    public void SetOnTurnFinished(Action action)
    {
        onTurnFinished = action;
    }

    private void SetNewDestination()
    {
        switch (movementDirection)
        {
            case Direction.left:
                MoveFurther(transform.position + new Vector3(-1f, 0, 0));
                break;
            case Direction.up:
                MoveFurther(transform.position + new Vector3(0, 0, 1f));
                break;
            case Direction.down:
                MoveFurther(transform.position + new Vector3(0, 0, -1f));
                break;
            case Direction.right:
                MoveFurther(transform.position + new Vector3(1f, 0, 0));
                break;
            default:
                break;
        }
    }

    private void SetDirection(float angle)
    {
        if (angle >= 315 || angle < 45)
        {
            //up
            movementDirection = Direction.up;
        }
        if (angle >= 45 && angle < 135)
        {
            //right
            movementDirection = Direction.right;
        }
        if (angle >= 135 && angle < 225)
        {
            //down
            movementDirection = Direction.down;
        }
        if (angle >= 225 && angle < 315)
        {
            //left
            movementDirection = Direction.left;
        }
    }

    private void FaceDirectionalWall(DirectionWall wall)
    {
        switch (wall._Direction)
        {
            case DirectionWall.Direction.up:
                rotationDirection = new Vector3(0, 0, 0);
                break;

            case DirectionWall.Direction.right:
                rotationDirection = new Vector3(0, 90, 0);
                break;

            case DirectionWall.Direction.down:
                rotationDirection = new Vector3(0, 180, 0);
                break;

            case DirectionWall.Direction.left:
                rotationDirection = new Vector3(0, 270, 0);
                break;

            default:
                Debug.LogError("Direction not found!");
                return;
        }

        _State = State.turning;
    }

    private void TurnInRandomDirection()
    {
        int random = UnityEngine.Random.Range(1, 6);
        if(random <= 2) // turn left
        {
            rotationDirection += new Vector3(0, -90, 0);
        }
        else if(random <= 4) // turn right
        {
            rotationDirection += new Vector3(0, 90, 0);
        }
        else // turn around
        {
            rotationDirection += new Vector3(0, 180, 0);
        }

        float y = rotationDirection.y;
        if (y >= 360)
        {
            rotationDirection += new Vector3(0, -360, 0);
        }
        else if(y < 0)
        {
            rotationDirection += new Vector3(0, 360, 0);
        }

        _State = State.turning;
    }

    private void FaceWall(GameObject wall)
    {
        if(_State != State.moving)
        {
            return;
        }
        _nav.ResetPath();

        _State = State.stop;

        DirectionWall directionWall = wall.GetComponentInParent<DirectionWall>();

        if (directionWall != null)
        {
            FaceDirectionalWall(directionWall);
        }
        else
        {
            TurnInRandomDirection();
        }
    }

    private bool NeedTurning()
    {
        return Mathf.Abs(transform.eulerAngles.y - rotationDirection.y) <= ROTATION_TOLERANCE ? false : true;
    }

    private bool IsNextPointReached()
    {
        return Vector3.Distance(transform.position, destination) <= POINT_REACHED_TOLERANCE ? true : false;
    }

    public void StartMoving()
    {
        _nav.SetDestination(destination);
        _State = State.moving;
    }

    public void MoveTo(Vector3 nextPoint)
    {
        destination = nextPoint;

        if (_nav.CalculatePath(destination, path))
        {
            if(path.corners.Length > 1)
            {
                float y = (int)(Mathf.Atan2(path.corners[1].x - transform.position.x, path.corners[1].z - transform.position.z) * 180 / Math.PI);
                if (y < 0)
                {
                    y += 360;
                }
                else if (y > 360)
                {
                    y -= 360;
                }

                rotationDirection = new Vector3(0, y, 0);
            }
        }

        _State = State.turning;
    }

    public void MoveFurther(Vector3 nextPoint)
    {
        destination = nextPoint;
        StartMoving();
    }
    
    private void Awake()
    {
        path = new NavMeshPath();
    }

    private void Start()
    {
        _vision.SetOnFaceWall(FaceWall);
        
        SetDirection(transform.eulerAngles.y);
    }

    private void Update()
    {
        switch (_State)
        {
            case State.moving:
                if (IsNextPointReached())
                {
                    _State = State.stop;
                    transform.position = destination;

                    if(onPointReached != null)
                    {
                        onPointReached();
                    }
                    else
                    {
                        SetNewDestination();
                    }
                }
                break;

            case State.turning:
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, rotationDirection.y, Time.deltaTime * _rotationSpeed);
                transform.eulerAngles = new Vector3(0, angle, 0);

                if (!NeedTurning())
                {
                    _State = State.stop;
                    transform.eulerAngles = new Vector3(0, rotationDirection.y, 0);
                    if (onTurnFinished != null)
                    {
                        onTurnFinished();
                    }
                    else
                    {
                        SetDirection(transform.eulerAngles.y);
                        SetNewDestination();
                    }
                }
                break;
        }
    }
}
