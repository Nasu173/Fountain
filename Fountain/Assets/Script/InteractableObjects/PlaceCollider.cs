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

            GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
            {
                Track = AudioTrack.Fountain1
            });

            GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
            {
                Track = AudioTrack.MonsterFootstep
            });
        } 
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
