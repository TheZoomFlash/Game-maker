using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Reference
    public Animator scene_animator;
    public float fadeTime = 1f;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
    }

    //temp
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            LoadNextScene();
        }
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        scene_animator.SetTrigger("start");

        yield return new WaitForSeconds(fadeTime);

        SceneManager.LoadScene(levelIndex);
    }
}
