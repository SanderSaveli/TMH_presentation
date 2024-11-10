using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterpriseUiManager : MonoBehaviour
{
    public void LoadMenuScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }
}
