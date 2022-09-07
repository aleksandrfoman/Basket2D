using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private CircleCollider2D collider2D;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip collisionClip, shootClip;
    public Vector3 pos { get { return transform.position; } }

    public void Push(Vector2 force)
    {
        audioSource.clip = shootClip;
        audioSource.Play();
        rigidbody2D.AddTorque(1f, ForceMode2D.Impulse);
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
    }

    public void ActivateRb()
    {
        rigidbody2D.isKinematic = false;
    }

    public void DeactivateRb()
    {
        rigidbody2D.velocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        rigidbody2D.angularVelocity = 0f;
        rigidbody2D.isKinematic = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.clip = collisionClip;
        audioSource.Play();
    }

}
