using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpiderMoving : MonoBehaviour
{
    [Header("Moving")]
    public float speed;
    public LayerMask GroundLayer;

    [Space]
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float checkWallDistance = 2f;

    [Space]
    public GameObject WebObject;
    public GameObject PointOfSupportWeb;
    [SerializeField] private float webSpawnInterval;
    private bool _makingWeb;
    private GameObject _startPointOfSupport;

    [Space]
    [Header("Events")]
    public UnityEvent onSpiderJumped = new();
    public UnityEvent onSpiderLanded = new();
    public UnityEvent onSpiderStartMakingWeb = new();

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private Vector2 _moveVector;
    private bool _rotating;
    private float _angleToRotate;


    private int _currentMoveDirection;
    public int CurrentMoveDirection
    {
        get => _currentMoveDirection;
        set
        {
            _currentMoveDirection = Mathf.Clamp(value, -1, 1);
        }
    }

    public Vector2 Upward { get; private set; }
    public bool Climbing { get; private set; }
    public bool OnChasm { get; private set; }
    public bool Jumping { get; private set; }


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _velocity = _rb.velocity;
        _moveVector = Vector2.right;
        Upward = Vector2.up;
        _angleToRotate = transform.rotation.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        if (CurrentMoveDirection != 0 && !Jumping)
        {
            Move();
            OnChasm = CheckChasm();
            CheckWalls();
        }

        else
        {
            if (!Jumping)
                _velocity = Vector2.zero;
        }

        _rotating = CheckRotation();

        if (_rotating)
        {
            var nextAngle = Quaternion.Euler(0, 0, _angleToRotate);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, nextAngle, rotationSpeed * Time.deltaTime);
        }

        _moveVector = transform.right * CurrentMoveDirection;

        if (!Jumping)
            StickToGround();

        else
            MakeGravity();

        //Debug.Log(Jumping);
        //Debug.Log("Up - " + Upward);
        //Debug.Log("right - " + transform.right);
        //Debug.Log("rotating - " + _rotating);
        //Debug.Log("Angle to rotate - " + _angleToRotate);
        //Debug.Log(_velocity);

        Climbing = CheckClimbing();

        _rb.velocity = _velocity;
    }
                                                                                                        
    private void Move()
    {
        var targetSpeed = _moveVector.normalized * speed;

        _velocity = targetSpeed;
    }

    public void Jump(Vector2 direction, float force)
    {
        Upward = -direction;
        _velocity = direction * force;
        Jumping = true;
        onSpiderJumped.Invoke();
    }

    public IEnumerator JumpAndMakeWeb(Vector2 direction, float force)
    {
        Jump(direction, force);
        onSpiderStartMakingWeb.Invoke();
        _makingWeb = true;
        _startPointOfSupport = Instantiate(PointOfSupportWeb, (Vector2)transform.position + Upward / 2, Quaternion.identity);

        while (Jumping)
        {
            if (_startPointOfSupport == null || _startPointOfSupport.GetComponent<PointOfSupportWeb>().Destroyed)
                yield break;

            var web = Instantiate(WebObject, (Vector2)transform.position, Quaternion.identity);
            _startPointOfSupport.GetComponent<PointOfSupportWeb>().ConnectedWebs.Add(web);
                
            yield return new WaitForSeconds(webSpawnInterval); 
        }
    }

    private bool CheckWalls()
    {
        var ray = new Ray2D((Vector2)transform.position, _moveVector.normalized);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, checkWallDistance, GroundLayer);

        if (hit)
        {
            Upward = hit.normal;
            return true;
        }
       
        return false;
    }

    private bool CheckChasm()
    {
        if (_rotating)
            return false;

        var rayPos = (Vector2)transform.position + ((Vector2)transform.right * CurrentMoveDirection * GetComponent<CircleCollider2D>().radius);

        var ray = new Ray2D(rayPos, -transform.up);
        //Debug.DrawRay(ray.origin, ray.direction);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 3f, GroundLayer);

        if (hit)
            return false;

        Upward = Utils.GetVectorRotated90(Upward, CurrentMoveDirection);
        return true;
    }

    private bool CheckRotation()
    {
        var directionModifier = CurrentMoveDirection == 0 ? 1 : CurrentMoveDirection;

        bool turnedInRightAngle = Utils.CompareNumsApproximately(Vector2.Angle(transform.right, Utils.GetVectorRotated90(Upward, 1)), 0, 2);
        if (turnedInRightAngle)
        {
            return false;
        }

        _angleToRotate = Vector2.SignedAngle(Vector2.right * directionModifier, Utils.GetVectorRotated90(Upward, directionModifier));
        
         return true;
    }

    private bool CheckClimbing() => Upward != Vector2.up;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Jumping)
        {
            Upward = collision.GetContact(0).normal;
            Jumping = false;
            onSpiderLanded.Invoke();

            if (_makingWeb)
            {
                if (_startPointOfSupport != null && !_startPointOfSupport.GetComponent<PointOfSupportWeb>().Destroyed)
                {
                    var endSupportPoint = Instantiate(PointOfSupportWeb, (Vector2)transform.position, Quaternion.identity);
                    var web = Instantiate(WebObject, transform.position, Quaternion.identity);

                    _startPointOfSupport.GetComponent<PointOfSupportWeb>().ConnectedWebs.Add(web);
                    _startPointOfSupport.GetComponent<PointOfSupportWeb>().ConnectedWebs.Add(endSupportPoint);

                    endSupportPoint.GetComponent<PointOfSupportWeb>().ConnectedWebs = GetReversedWebListVersion();
                }

                _makingWeb = false;
            }
        }
    }

    private List<GameObject> GetReversedWebListVersion()
    {
        var webListCopy = new List<GameObject>();
        foreach(var web in _startPointOfSupport.GetComponent<PointOfSupportWeb>().ConnectedWebs)
        {
            webListCopy.Add(web);
        }
        webListCopy.Reverse();
        webListCopy.RemoveAt(0);
        webListCopy.Add(_startPointOfSupport);

        return webListCopy;
    }

    private void StickToGround()
    {
        if (!_rotating)
            _velocity -= Upward;
    }

    private void MakeGravity()
    {
        _velocity -= Vector2.up;
    }

    public Vector2 GetLookVector()
    {
        return transform.right;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay((Vector2)transform.position, _moveVector.normalized);
    }  
}
