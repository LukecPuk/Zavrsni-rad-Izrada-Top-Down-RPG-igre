using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject destroyVFX;
    [SerializeField] private AudioClip SoundClip;
    [SerializeField] private float SoundVolume = 0.25f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<DamageSource>() || other.gameObject.GetComponent<Projectile>())
        {
            PickUpSpawner pickUpSpawner = GetComponent<PickUpSpawner>();
            pickUpSpawner?.DropItems();
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
            SFXManager.instance.PlaySFXClip(SoundClip, transform, SoundVolume);
            Destroy(gameObject);
        }
    }
}