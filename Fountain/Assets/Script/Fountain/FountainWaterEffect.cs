using UnityEngine;

namespace Foutain.Fountain
{
    public class FountainWaterEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _waterParticle;

        public void Play() => _waterParticle.Play();
        public void Stop() => _waterParticle.Stop();
    }
}
