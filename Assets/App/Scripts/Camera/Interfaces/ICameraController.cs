using UnityEngine;

public interface ICameraController
{
   public void SetTarget(Transform target);
   public void SetTarget(Transform target, Vector3 angle);
}