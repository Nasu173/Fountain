using UnityEngine;

namespace Fountain.Fountain
{
    public class FountainWaterEffect : MonoBehaviour
    {
        [SerializeField] ParticleSystem _waterParticle;

        public void Play() => _waterParticle.Play();
        public void Stop() => _waterParticle.Stop();
    }
}
