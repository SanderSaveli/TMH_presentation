using UnityEngine;

public interface IModelManager
{
    public void ShowModel(int index);
    Transform GetCurrentModelTransform();
    ModelData GetCurrentModelData();
    ModelData GetModelData(int index);
}