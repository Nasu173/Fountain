using System;
using System.Collections;
using UnityEngine;

namespace Fountain.Fountain
{
    public class FountainController : MonoBehaviour
    {
        [SerializeField] FountainSteamEffect _steam;
        [SerializeField] FountainWaterEffect _water;
        [SerializeField] private AudioClip _fountainSound;
        [SerializeField] float _steamDuration = 2f;
        [SerializeField] float _waterDelay = 0.25f;
        [SerializeField] bool hasSound = false;

        bool _isOn;
        Coroutine _sequence;

        void OnEnable()
        {
            GameEventBus.Subscribe<TaskStartEvent>(TurnOn);
            GameEventBus.Subscribe<TaskProgressEvent>(StopSFX);
        }

        void OnDisable()
        {
            GameEventBus.Unsubscribe<TaskStartEvent>(TurnOn);
            GameEventBus.Unsubscribe<TaskProgressEvent>(StopSFX);
        }

        private void StopSFX(TaskProgressEvent @event)
        {
            if (@event.TaskId != 12.ToString()) return;
            if (!hasSound) return;
            GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
            {
                Track = AudioTrack.Fountain1,
            });
        }

        private void TurnOn(TaskStartEvent @event)
        {
            if (@event.TaskId != 12.ToString()) return;
            if (_isOn) return;
            _isOn = true;
            _sequence = StartCoroutine(Sequence());

            if (hasSound)
            {
                GameEventBus.Publish<PlaySoundEvent>(new PlaySoundEvent()
                {
                    Clip = _fountainSound,
                    Track = AudioTrack.Fountain1,
                    IsLoop = true,
                });
            }
        }

        public void TurnOn()
        {
            if (_isOn) return;
            _isOn = true;
            _sequence = StartCoroutine(Sequence());
        }

        public void TurnOff()
        {
            if (!_isOn) return;
            _isOn = false;
            if (_sequence != null) StopCoroutine(_sequence);
            _steam.StopImmediate();
            _water.Stop();
        }

        private IEnumerator Sequence()
        {
            _steam.Play();
            _steam.FadeOut(_steamDuration);
            yield return new WaitForSeconds(_steamDuration);
            yield return new WaitForSeconds(_waterDelay);
            _water.Play();
        }
    }
}
