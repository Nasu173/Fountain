using System.Collections;
using UnityEngine;

namespace Foutain.Fountain
{
    public class FountainController : MonoBehaviour
    {
        [SerializeField] FountainSteamEffect _steam;
        [SerializeField] FountainWaterEffect _water;
        [SerializeField] float _steamDuration = 2f;
        [SerializeField] float _waterDelay = 0.25f;

        bool _isOn;
        Coroutine _sequence;

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
