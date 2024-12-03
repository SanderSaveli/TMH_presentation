using UnityEngine;

public interface ILineManager
{
    public void UpdateLine(Vector3 start, Vector3 end);
    public void ClearLine();
}