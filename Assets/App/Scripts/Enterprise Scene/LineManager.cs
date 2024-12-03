using UnityEngine;

public class LineManager : MonoBehaviour, ILineManager
{
    [SerializeField] private LineRenderer lineRenderer;

    public void UpdateLine(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        float distance = Vector3.Distance(start, end);
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = Mathf.Lerp(0.5f, 0.01f, distance / 10f);
    }

    public void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }
}