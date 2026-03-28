using System.Collections;
using UnityEngine;

namespace Fountain.Fountain
{
    public class FountainCycleController : MonoBehaviour
    {
        [SerializeField] FountainController _controller;
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
            _controller.TurnOff();
        }

        private IEnumerator Cycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_idleMin, _idleMax));
                _controller.TurnOn();
                yield return new WaitForSeconds(Random.Range(_eruptMin, _eruptMax));
                _controller.TurnOff();
            }
        }
    }
}
