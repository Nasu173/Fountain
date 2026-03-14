using System.Collections;
using UnityEngine;

namespace Fountain.Fountain
{
    public class FountainSteamEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _steamParticle;
        [SerializeField] float _initialEmissionRate = 20f;

        public void Play()
        {
            var emission = _steamParticle.emission;
            emission.rateOverTime = _initialEmissionRate;
            _steamParticle.Play();
        }

        public void FadeOut(float duration)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }

        private IEnumerator FadeOutCoroutine(float duration)
        {
            var emission = _steamParticle.emission;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                emission.rateOverTime = Mathf.Lerp(_initialEmissionRate, 0f, elapsed / duration);
                yield return null;
            }
            emission.rateOverTime = 0f;
            _steamParticle.Stop(false);
        }

        public void StopImmediate()
        {
            _steamParticle.Stop(true);
        }
    }
}
