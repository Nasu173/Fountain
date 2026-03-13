using UnityEngine;

namespace Foutain.Fountain
{
    public class FountainTestInput : MonoBehaviour
    {
        [SerializeField] FountainController _fountain;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O)) _fountain.TurnOn();
            if (Input.GetKeyDown(KeyCode.P)) _fountain.TurnOff();
        }
    }
}
