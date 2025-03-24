using UnityEngine;

public interface IModelManager
{
    void ShowModel(int index);
    Transform GetCurrentModelTransform();
    ModelData GetCurrentModelData();
    ModelData GetModelData(int index);
}