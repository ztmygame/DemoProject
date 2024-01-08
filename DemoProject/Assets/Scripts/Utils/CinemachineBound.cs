using UnityEngine;
using Cinemachine;

public class CinemachineBound : MonoBehaviour
{
    void Start()
    {
        SwitchCinemachineConfinerShape();
    }

    private void SwitchCinemachineConfinerShape()
    {
        PolygonCollider2D confinerCollider = GameObject.FindGameObjectWithTag("CinemachineBound").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerCollider;

        confiner.InvalidatePathCache(); //Call this if the bounding shape's points change at runtime
    }
}
