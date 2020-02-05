using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Reference
    public Canvas story;
    public Canvas UITransition;
    public float fadeTime = 1f;

    bool isStory = true;
    Animator scene_animator;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
        scene_animator = UITransition.GetComponent<Animator>();
    }

    void Start()
    {
        UpdateStory();
    }

    //temp
    void Update()
    {
        // temp
        if (PlayerInput.Instance.Pause.Down)
        {
            TurnStory();
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            LoadNextScene();
        }
    }

    public void TurnStory()
    {
        isStory = !isStory;
        UpdateStory();
    }

    public void UpdateStory()
    {
        if (!isStory)
        {
            story.gameObject.SetActive(false);
            UITransition.gameObject.SetActive(true);
            PlayerInput.Instance.GainControl();
        }
        else
        {
            story.gameObject.SetActive(true);
            UITransition.gameObject.SetActive(false);
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
