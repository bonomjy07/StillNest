using UnityEngine;

public class DottedLineDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    public void Draw(Vector3 from, Vector3 to)
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, to);
    }

    public void Clear()
    {
        _lineRenderer.positionCount = 0;
    }
}
