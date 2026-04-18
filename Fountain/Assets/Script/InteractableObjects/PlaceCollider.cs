using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCollider : MonoBehaviour
{
    public GameObject _collider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _collider.SetActive(true);
        } 
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
