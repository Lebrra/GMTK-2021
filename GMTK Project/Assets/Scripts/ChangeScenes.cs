using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScenes : MonoBehaviour
{
    public void BackToTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
