using System;
using System.Collections;
using System.Collections.Generic;
using _scripts.UI;
using Assets._scripts;
using MirraGames.SDK;
using UnityEngine;
using UnityEngine.AI;

public enum BusType
{
    Small,
    Medium,
    Large
}

[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
public class Bus : MonoBehaviour, IRaycastTarget
{
    public int Capacity;
    public int PersonsInside = 0;
    private Material _color;
    public MeshRenderer meshRendererBody; 
    public GameObject arrow;
    public bool IsWayClear;
    public BusType Type;
    public bool _BusHided = false;
    public static Action<int> onScoreChanged;
    
    [SerializeField] private Transform[] _passangerPoints;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _returnSpeed = 3f;
    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private float raycastDistance = 30f;
    [SerializeField] private LayerMask busLayer;
    [SerializeField] private GameObject _fogEffect;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private GameObject _hidedSighes;

    private BusGenerator _busGenerator;
    private const string _shake = "Shake";
    private ParkingManager _parkingManager;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private bool _isReturning = false;
    private bool _isMovingForward = false;
    private bool _isGoAway = false;
    private ParkingSpot _targetSpot;
    private Coroutine _animationCoroutine;

    private Rigidbody _rigidbody;
    private bool _isClickable = true;
    public bool isVip = false;
    public ColorType ColorType;
    public Material Color
    {
        get => _color;
        set
        {
            _color = value;
            meshRendererBody.material = _color;
        }
    }
    
    private void Awake()
    {
        _animator.enabled = false;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.mass = 1000;
        if (_rigidbody == null)
        {
            Debug.LogError("Bus must have a Rigidbody component!");
        }

        if (_navAgent != null)
        {
            _navAgent.enabled = false;
        }
    }

    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    public void Initialize(ParkingManager parkingManager, BusGenerator busGenerator) // в старте
    {
        _parkingManager = parkingManager;
        _busGenerator = busGenerator;
    }

    // private void OnMouseDown()
    // {
    //     if (EventSystem.current.IsPointerOverGameObject())
    //     {
    //         return;
    //     }
    //     if (!_isClickable) return;
    //
    //     _targetSpot = _parkingManager.GetAvailableSpot();
    //     SoundManager.instance.PlayClickOnBusSound();
    //     if (_targetSpot != null)
    //     {
    //         _rigidbody.mass = 1;
    //         _isMovingForward = true;
    //         _fogEffect.SetActive(true);
    //         _rigidbody.isKinematic = false;
    //         _isClickable = false;
    //     }
    // }

    private void Update()
    {
        if(_isGoAway)
            return;
        if (_navAgent == null)
            return;
        if (_navAgent.enabled && _navAgent.remainingDistance <= _navAgent.stoppingDistance && !_navAgent.pathPending)
        {
            CompleteParking();
        }
    }

    private void FixedUpdate()
    {
        if (_isMovingForward)
        {
            MoveForward();
        }
        else if (_isReturning)
        {
            ReturnToInitialPositionStep();
        }
    }

    public void Rotate()
    {
        Vector3 rotation = gameObject.transform.eulerAngles;
        rotation.y += 180;
        gameObject.transform.eulerAngles = rotation;
    }
    
    public void Rotate90()
    {
        Vector3 rotation = gameObject.transform.eulerAngles;
        rotation.y += 90;
        gameObject.transform.eulerAngles = rotation;
    }
    

    public void TakePerson(Person person)
    {
        person.SetPlace(_passangerPoints[PersonsInside]);
        PersonsInside++;
    }

    public void LeaveParking(List<Vector3> path)
    {
        _fogEffect.SetActive(true);
        _isGoAway = true;
        _navAgent.enabled = true;
        int score = 0;
        switch(Type)
        {
            case BusType.Small:
                score = 4;
                break;
            case BusType.Medium:
                score = 6;
                break;
            case BusType.Large:
                score = 8;
                break;
        }
        MirraSDK.Achievements.SetScore("score", score);
        onScoreChanged?.Invoke(score);
        StartCoroutine(FollowPath(path));
    }

    public void CheckWay()
    {
        Vector3 leftPosition = transform.position - transform.right * 0.5f + transform.up * 0.5f;
        Vector3 rightPosition = transform.position + transform.right * 0.5f + transform.up * 0.5f;

        if (Physics.Raycast(leftPosition, transform.forward, out RaycastHit hitLeft, raycastDistance, busLayer))
        {
            IsWayClear = false;
        }
        else if (Physics.Raycast(rightPosition, transform.forward, out RaycastHit hitRight, raycastDistance, busLayer))
        {
            IsWayClear = false;
        }
        else
        {
            IsWayClear = true;
        }
    }

    public void SetCollider(bool value) => _collider.enabled = value;

    public void SetColliderXSize(float value)
    {
        _collider.size = new Vector3(value, _collider.size.y, _collider.size.z);
    }
    
    public void PlayShakeAnimaton()
    {
        
        _animator.enabled = true;
        _animator.SetTrigger(_shake);
        if (_animationCoroutine == null)
            _animationCoroutine = StartCoroutine(AnimationProcess());
    }

    private IEnumerator AnimationProcess()
    {
        _animator.enabled = true;
        _animator.SetTrigger(_shake);
        yield return new WaitForSeconds(2f);
        _animator.enabled = false;
        _animationCoroutine = null;
    }

    private void MoveForward()
    {
        _rigidbody.mass = 1;
        _rigidbody.MovePosition(transform.position + transform.forward * _moveSpeed * Time.fixedDeltaTime);
    }

    private void ReturnToInitialPositionStep()
    {
        transform.position =
            Vector3.MoveTowards(transform.position, _initialPosition, _returnSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _initialRotation,
            _returnSpeed * 50 * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, _initialPosition) < 0.1f)
        {
            _isReturning = false;
            _rigidbody.isKinematic = true;
            _rigidbody.mass = 1000;
            _isClickable = true;
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
            _fogEffect.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isMovingForward && collision.collider.CompareTag("Bus"))
        {
            Bus bus = collision.collider.GetComponent<Bus>();
            if (bus != null)
            {
                bus.PlayShakeAnimaton();
                SoundManager.instance.PlayBusDamagedSound();
            }
            _isMovingForward = false;
            _isReturning = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isMovingForward)
        {
            _targetSpot = _parkingManager.GetAvailableSpot();
            if (_targetSpot != null)
            {
                _isMovingForward = false;
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                StartFollowToParking();
            }
            else
            {
                _isMovingForward = false;
                _isReturning = true;
            }
        }
    }

    private void StartFollowToParking()
    {
        _collider.enabled = false;
        _rigidbody.isKinematic = true;
        _navAgent.enabled = true;
        _navAgent.SetDestination(_targetSpot.transform.position);
        _parkingManager.MarkSpotOccupied(_targetSpot, this);
    }


    private void CompleteParking()
    {
        _busGenerator.RemoveBusFromZones(this);
        arrow.SetActive(false);
        _navAgent.enabled = false;
        transform.position = _targetSpot.transform.position;
        transform.rotation = _targetSpot.transform.rotation;
        _parkingManager.BusArrived(this);
        _fogEffect.SetActive(false);
    }
    
    private IEnumerator FollowPath(List<Vector3> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            yield break;
        }

        foreach (Vector3 waypoint in waypoints)
        {
            _navAgent.SetDestination(waypoint);

            while (_navAgent.pathPending || _navAgent.remainingDistance > _navAgent.stoppingDistance)
            {
                yield return null;
            }
        }
        _collider.enabled = true;
        _navAgent.enabled = false; 
    }

    public void Iterract()
    {
        // if (EventSystem.current.IsPointerOverGameObject())
        // {
        //     return;
        // }
        if (!_isClickable) return;

        _targetSpot = _parkingManager.GetAvailableSpot();
        SoundManager.instance.PlayClickOnBusSound();
        if (_targetSpot != null)
        {
            _rigidbody.mass = 1;
            _isMovingForward = true;
            _fogEffect.SetActive(true);
            _rigidbody.isKinematic = false;
            _isClickable = false;
        }
    }


    public void CompleteFly()
    {
        _isClickable = false;
        _busGenerator.RemoveBusFromZones(this);
        arrow.SetActive(false);
        _navAgent.enabled = false;
        _parkingManager.BusFlightArrived(this);
        isVip = true;
    }

    public void HideBus(Material busMaterial)
    {
        _hidedSighes.gameObject.SetActive(true);
        meshRendererBody.material = busMaterial;
        _parkingManager.BusInSpotChanged += CheckExit;
        IsWayClear = false;
        CheckWay();
        if (IsWayClear)
            OpenBus();
    }

    private void OpenBus()
    {
        _hidedSighes.gameObject.SetActive(false);
        _parkingManager.BusInSpotChanged -= CheckExit;
        meshRendererBody.material = _color;
    }

    private void CheckExit(ColorType arg1, bool arg2)
    {
        CheckWay();
        if (IsWayClear)
            OpenBus();
    }
}