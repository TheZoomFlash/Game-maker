using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Reference
    public Canvas story;
    public Canvas UIMain;
    public float fadeTime = 1f;

    bool isStory = false;
    Animator scene_animator;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        scene_animator = UIMain.GetComponent<Animator>();
    }

    //temp
    void Update()
    {
        if(PlayerInput.Instance.Pause.Down)
        {
            TurnStory();
        }
        // temp
        if(Input.GetKeyDown(KeyCode.P))
        {
            LoadNextScene();
        }
    }

    public void TurnStory()
    {
        isStory = !isStory;
        if (!isStory)
        {
            story.gameObject.SetActive(false);
            UIMain.gameObject.SetActive(true);
            PlayerInput.Instance.GainControl();
        }
        else
        {
            story.gameObject.SetActive(true);
            UIMain.gameObject.SetActive(false);
            PlayerInput.Instance.ReleaseControl();
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
