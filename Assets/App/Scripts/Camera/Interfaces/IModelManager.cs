using UnityEngine;

public interface IModelManager
{
    public void ShowModel(GameObject modelPrefab);
    Transform GetCurrentModelTransform();
    ObjectInfo GetCurrentObjectInfo();
    ObjectInfo GetObjectInfo(int index);
}