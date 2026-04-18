using System.Collections;
using UnityEngine;

namespace Fountain.Fountain
{
    public class LightCycleController : MonoBehaviour
    {
        [SerializeField] Light[] lights;
        [SerializeField] float _idleMin = 3f;
        [SerializeField] float _idleMax = 8f;
        [SerializeField] float _eruptMin = 5f;
        [SerializeField] float _eruptMax = 12f;

        Coroutine _cycle;

        private void OnEnable()
        {
            _cycle = StartCoroutine(Cycle());
        }

        private void OnDisable()
        {
            if (_cycle != null) StopCoroutine(_cycle);
            foreach (Light l in lights)
            {
                l.enabled = false;
            }
        }

        private IEnumerator Cycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_idleMin, _idleMax));
                foreach (Light l in lights)
                {
                    l.enabled = true;
                }
                yield return new WaitForSeconds(Random.Range(_eruptMin, _eruptMax));
                foreach (Light l in lights)
                {
                    l.enabled = false;
                }
            }
        }
    }
}