using Fountain.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.UI
{
    public class ProgressBar : MonoBehaviour
    {
        private Image fill;
        private void Awake()
        {
            fill = this.transform.FindChildByName(nameof(fill)).GetComponent<Image>();
        }
        public void SetFillAmount(float amount)
        {
            fill.fillAmount = amount;
        }
        
    }
}
