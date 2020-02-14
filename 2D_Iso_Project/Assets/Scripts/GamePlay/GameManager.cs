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

        Application.targetFrameRate = 60;
        scene_animator = UITransition.GetComponent<Animator>();
    }

    void Start()
    {
        UpdateStory();
        CheckXboxController();
    }


    void CheckXboxController()
    {
        // Get Joystick Names
        string[] temp = Input.GetJoystickNames();

        //Check whether array contains anything
        if (temp.Length > 0)
        {
            //Iterate over every element
            for (int i = 0; i < temp.Length; ++i)
            {
                //Check if the string is empty or not
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    PlayerInput.Instance.inputType = PlayerInput.InputType.Controller;
                    //Not empty, controller temp[i] is connected
                    //Debug.Log("Controller " + i + " is connected using: " + temp[i]);
                }
                else
                {
                    PlayerInput.Instance.inputType = PlayerInput.InputType.MouseAndKeyboard;
                    //If it is empty, controller i is disconnected
                    //where i indicates the controller number
                    //Debug.Log("Controller: " + i + " is disconnected.");

                }
            }
        }
    }

    void Update()
    {
        CheckXboxController();
        StoryMoveOn();
        // temp
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    LoadNextScene();
        //}
    }

    void StoryMoveOn()
    {
        if (isStory)
        {
            if(PlayerInput.Instance.inputType == PlayerInput.InputType.MouseAndKeyboard)
            {
                if(Input.GetMouseButtonDown(0))
                    storyController.Instance.GoNext();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton0))
                    storyController.Instance.GoNext();
            }
        }
    }

    void SetStory(bool b)
    {
        isStory = b;
    }

    public void TurnStory()
    {
        SetStory(!isStory);
        UpdateStory();
    }

    public void UpdateStory()
    {
        if (!isStory)
        {
            story.gameObject.SetActive(false);
            //UITransition.gameObject.SetActive(true);
            PlayerInput.Instance.GainControl();
        }
        else
        {
            story.gameObject.SetActive(true);
            //UITransition.gameObject.SetActive(false);
            PlayerInput.Instance.ReleaseControl();
        }
    }


    public void ResetPlayer()
    {
        PlayerController.PlayerInstance.Teleport(Vector2.zero);
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
        scene_animator.SetTrigger("start");
        yield return new WaitForSeconds(fadeTime);

        ResetPlayer();
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(fadeTime);
        TurnStory();
    }
}
