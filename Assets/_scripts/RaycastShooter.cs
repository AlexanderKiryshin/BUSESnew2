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


        void Start()
        {
            mainCamera = Camera.main;
            eventSystem = EventSystem.current;
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    ShootRaycast(touch.position);
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                ShootRaycast(Input.mousePosition);
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