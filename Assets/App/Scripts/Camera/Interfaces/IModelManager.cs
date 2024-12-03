using UnityEngine;

public interface IModelManager
{
    public void ShowModel(int index);
    Transform GetCurrentModelTransform();
    ObjectInfo GetCurrentObjectInfo();
    ObjectInfo GetObjectInfo(int index);
}