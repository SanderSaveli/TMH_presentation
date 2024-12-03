using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterpriseUIManager : MonoBehaviour
{
    public void LoadMenuScene() => SceneManager.LoadScene(SceneNames.MenuScene);
}
