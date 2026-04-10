using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup playerFootstepGroup;
    [SerializeField] private AudioMixerGroup fountain1Group;
    [SerializeField] private AudioMixerGroup otherGroup;
    [SerializeField] private AudioMixerGroup monsterFootstepGroup;
    [SerializeField] private AudioMixerGroup fountain2Group;

    private readonly Dictionary<AudioTrack, AudioSource> _sources = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sources[AudioTrack.BGM]             = CreateSource(bgmGroup);
        _sources[AudioTrack.PlayerFootstep]  = CreateSource(playerFootstepGroup);
        _sources[AudioTrack.Fountain1]       = CreateSource(fountain1Group);
        _sources[AudioTrack.Other]           = CreateSource(otherGroup);
        _sources[AudioTrack.MonsterFootstep] = CreateSource(monsterFootstepGroup);
        _sources[AudioTrack.Fountain2]       = CreateSource(fountain2Group);

        GameEventBus.Subscribe<PlaySoundEvent>(OnPlaySound);
    }

    private void OnDestroy() => GameEventBus.Unsubscribe<PlaySoundEvent>(OnPlaySound);

    private void OnPlaySound(PlaySoundEvent e)
    {
        if (e.Clip == null) return;
        _sources[e.Track].PlayOneShot(e.Clip, e.Volume);
    }

    private AudioSource CreateSource(AudioMixerGroup group)
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = group;
        src.playOnAwake = false;
        return src;
    }
}
