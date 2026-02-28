using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Foutain.UI
{
    public class DropDownTest : MonoBehaviour
    {
        public TMP_Dropdown dropdown;
        public List<string> options;
        private void Start()
        {
            dropdown = this.GetComponent<TMP_Dropdown>();
        }
        private void OnGUI()
        {
            if (GUILayout.Button("clear"))
            {
                dropdown.ClearOptions();
            }
            if (GUILayout.Button("ADD"))
            {
                dropdown.AddOptions(options);
            }
        }
    }
}
