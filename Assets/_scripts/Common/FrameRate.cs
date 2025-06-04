using UnityEngine;

public class FrameRate : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 90;
    void Awake()
    {
        Application.targetFrameRate = _targetFrameRate;
    }

}
