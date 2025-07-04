using System.Collections.Generic;
using _scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _scripts
{
    public class RaycastShooter : MonoBehaviour
    {
        [SerializeField] private HeliSystem _heliSystem;
        private Camera mainCamera;
        private EventSystem eventSystem;
        private GraphicRaycaster graphicRaycaster;
        private bool _isVipChoise = false;
        private float _time;
        private bool _touchIsBegan;


        void Start()
        {
            mainCamera = Camera.main;
            eventSystem = EventSystem.current;
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        void Update()
        {
            if (_touchIsBegan)
            {
                _time += Time.deltaTime;
            }
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchIsBegan = true;                   
                    //ShootRaycast(touch.position);
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    if (_time <0.5f)
                    {
                        ShootRaycast(touch.position);
                    }
                    _touchIsBegan = false;
                    _time = 0;
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _touchIsBegan = true;
                //ShootRaycast(Input.mousePosition);
            }
            else
                if (Input.GetMouseButtonUp(0))
            {
                if (_time < 0.5f)
                {
                    ShootRaycast(Input.mousePosition);
                }
                _touchIsBegan = false;
                _time = 0;
            }
        }


        void ShootRaycast(Vector2 screenPosition)
        {
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                    {
                        return;
                    }
                }
            }

            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                IRaycastTarget target = hit.collider.GetComponent<IRaycastTarget>();
                if (target != null)
                {
                    if (!_isVipChoise)
                        target.Iterract();
                    else
                    {
                        _isVipChoise = false;
                        _heliSystem.SetBus(target);
                    }
                }
            }
        }

        public void ActivateVipChoise() => _isVipChoise = true;
    }
}