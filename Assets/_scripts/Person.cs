using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Person : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _distanceThreshold = 0.1f;
    [SerializeField] private Transform _viewTransform;
    public SkinnedMeshRenderer meshRenderer;
    private Collider _collider;
    private PersonLoader _personLoader;

    private Material _color;
    private const string _isRunning = "isRunning";
    private Vector3 _previusPosition = Vector3.zero;

    private bool _isOnBus = false;

    public Material Color
    {
        get => _color;
        set
        {
            _color = value;
            meshRenderer.material = _color;
        }
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if(_isOnBus)
            return;
        
        if (_previusPosition != transform.position)
        {
            SetRunningAnimation(true);

            Vector3 dir = transform.position - _previusPosition;
            if (dir != Vector3.zero)
            {
                dir.Normalize();
            }

            if (dir != Vector3.zero)
            {
                transform.forward = dir;
            }

            _previusPosition = transform.position;
        }
        else
        {
            SetRunningAnimation(false);
        }
    }

    public void SetPlace(Transform transform)
    {
        _isOnBus = true;
        _collider.enabled = false;
        StartCoroutine(GoToPlace(transform));
    }

    public void SetRunningAnimation(bool isRunning) => _animator.SetBool(_isRunning, isRunning);

    private IEnumerator GoToPlace(Transform targetTransform)
    {
        while (Vector3.Distance(transform.position, targetTransform.position) > _distanceThreshold)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, targetTransform.position, _speed * Time.deltaTime);

            Vector3 direction = (targetTransform.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _speed * Time.deltaTime);
            }

            yield return null;
        }
        SetRunningAnimation(false);
        //_animator.enabled = false;
        _viewTransform.localScale = Vector3.one;
        transform.rotation = targetTransform.rotation;
        transform.position = targetTransform.position;
        transform.parent = targetTransform.parent;
    }
}