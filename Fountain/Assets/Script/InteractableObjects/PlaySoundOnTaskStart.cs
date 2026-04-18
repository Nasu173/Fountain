using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTaskStart : MonoBehaviour
{
    public AudioClip clip;
    public AudioTrack track;
    public string taskID = "17";
    public GameObject lights;
    private void OnEnable()
    {
        GameEventBus.Subscribe<TaskStartEvent>(PlaySound);
    }
    private void OnDisable()
    {
        GameEventBus.Unsubscribe<TaskStartEvent>(PlaySound);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void PlaySound(TaskStartEvent e)
    {
        if (e.TaskId==this.taskID)
        {
            GameEventBus.Publish<PlaySoundEvent>(new PlaySoundEvent()
            { Clip = clip, Track = track, IsLoop = true });
            lights.SetActive(true);
        } 
    }
}
