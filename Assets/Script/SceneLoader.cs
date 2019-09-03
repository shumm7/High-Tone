using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public string SceneName;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(SceneName);
    }
}
