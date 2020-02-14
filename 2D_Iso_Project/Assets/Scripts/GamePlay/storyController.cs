using Cradle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class storyController : MonoBehaviour
{
    public static storyController Instance { get; private set; }

    public Story story;

    void Awake()
    {
        if (story == null)
            story = GetComponent<Story>();
        Instance = this;
    }

    void Start()
    {
        //story.OnOutput += story_OnOutput;
        //story.OnPassageEnter += Story_OnPassageEnter;
        //story.OnStateChanged += story_OnStateChanged;
        //story.Begin();
    }

    // Specifies that this method is an Enter cue for the passage named "A large empty yard"
    [StoryCue("Dungeon - Entrance", "Enter")]
    [StoryCue("Dungeon - Sword room", "Enter")]
    [StoryCue("hidden area room", "Enter")]
    void enter_FirstRoom()
    {
        Debug.Log("Enter :" + story.CurrentPassage.Name);
        GameManager.Instance.TurnStory();
    }

    public void GoNext()
    {
        //Debug.Log("GoNext");
        IEnumerable<StoryLink> links = story.GetCurrentLinks();
        var emtor = links.GetEnumerator();
        if (emtor.MoveNext())
        {
            //Debug.Log("DoLink : " + emtor.Current.Name);
            story.DoLink(emtor.Current);
        }
        //GameManager.Instance.TurnStory();
    }

    //void story_OnOutput(StoryOutput output)
    //{
    //    // Do something with the output here
    //    //Debug.Log(output.Text);
    //}

    //void Story_OnPassageEnter(StoryPassage passage)
    //{
    //    // Do something with the output here
    //    Debug.Log("Story_OnPassageEnter :" + passage.Name);
    //    if (passage.Name)
    //    {
    //        Debug.Log("yes!!!!");
    //        //story.Pause();
    //    }
    //}

    //void story_OnStateChanged(StoryState state)
    //{
    //Debug.Log("story_OnStateChanged " + story.State);
    //if (story.State == StoryState.Idle)
    //{
    //    Debug.Log("Idle");
    //    // Interaction methods can be called now
    //    //story.DoLink("enterTheCastle");
    //}
    //else if (story.State == StoryState.Paused)
    //{
    //    Debug.Log("Paused");
    //    // Interaction methods can be called now
    //    //story.DoLink("enterTheCastle");
    //    //story.Resume();
    //}
    //else if (story.State == StoryState.Playing)
    //{
    //    Debug.Log("Playing");
    //    // Interaction methods can be called now
    //    //story.DoLink("enterTheCastle");
    //}
    //}
}
