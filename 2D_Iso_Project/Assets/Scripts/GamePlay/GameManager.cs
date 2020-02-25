using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Reference
    public Canvas story;
    public Canvas UITransition;
    public GameObject Combo_Obj;

    bool isStory = true;

    const float fadeDuration = 1f;
    Animator scene_animator;

    TextMeshProUGUI combText;
    Text comboParText;
    int comboNum = 0;
    const float comboDuration = 2f;
    float comboTimer = 0f;

    void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 60;
        scene_animator = UITransition.GetComponent<Animator>();
        comboParText = Combo_Obj.GetComponent<Text>();
        combText = Combo_Obj.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        UpdateStory();
        CheckXboxController();
    }


    void Update()
    {
        CheckXboxController();
        StoryMoveOn();
        UpdateCombo();
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
                if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.JoystickButton2))
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

    void UpdateStory()
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



    void UpdateCombo()
    {
        if (comboTimer < 0)
            return;

        comboTimer -= Time.deltaTime;
        if(comboTimer < 0)
        {
            comboNum = 0;
            comboParText.DOFade(0, 0.3f);
            combText.DOFade(0, 0.3f);
        }
    }

    public void AddCombo(int num)
    {
        if (num == 0)
            return;

        if (comboNum == 0)
        {
            DOTween.Complete(comboParText);
            DOTween.Complete(combText);
            comboParText.color = Color.yellow;
            combText.color = Color.white;
        }
        comboNum += num;
        comboTimer = comboDuration;
        DOTween.Complete(combText); 
        combText.text = comboNum.ToString();
        combText.transform.localScale = Vector3.one;
        combText.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack);
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
        yield return new WaitForSeconds(fadeDuration);

        ResetPlayer();
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(fadeDuration);
        TurnStory();
    }

    public void ResetPlayer()
    {
        PlayerController.PlayerInstance.Teleport(Vector2.zero);
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

    PlayerIndex _type = PlayerIndex.One;
    const float LeftMotorRange = 1f;
    const float RightMotorRange = 0.5f;
    const float DurationTime = 0.15f;
    Coroutine cor_vib = null;

    public void StartVib(float duration = 0.15f)
    {
        if (cor_vib != null)
            StopCoroutine(cor_vib);

        cor_vib = StartCoroutine(SetVibration(duration));
    }

    public IEnumerator SetVibration(float duration = DurationTime)
    {
        GamePad.SetVibration(_type, LeftMotorRange, RightMotorRange);
        yield return new WaitForSeconds(duration);
        GamePad.SetVibration(_type, 0, 0);
    }
}
