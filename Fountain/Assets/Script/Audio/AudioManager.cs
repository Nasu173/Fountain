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
        GameEventBus.Subscribe<PauseSoundEvent>(OnPauseSound);
    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<PlaySoundEvent>(OnPlaySound);
        GameEventBus.Unsubscribe<PauseSoundEvent>(OnPauseSound);
    }

    private void OnPlaySound(PlaySoundEvent e)
    {
        if (e.Clip == null) return;
        var src = _sources[e.Track];
        if (e.IsLoop)
        {
            src.clip = e.Clip;
            src.volume = e.Volume;
            src.pitch = e.Pitch;
            src.loop = true;
            src.Play();
        }
        else
        {
            src.pitch = e.Pitch;
            src.PlayOneShot(e.Clip, e.Volume);
        }
    }

    private void OnPauseSound(PauseSoundEvent e) => _sources[e.Track].Pause();

    private AudioSource CreateSource(AudioMixerGroup group)
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = group;
        src.playOnAwake = false;
        return src;
    }
}
