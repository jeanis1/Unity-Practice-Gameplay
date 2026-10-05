using System;
using System.Collections.Specialized;
using Code.Component.AI;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

namespace Code.Weapon
{
    public interface IBaseArrow
    {
        void SetDamage(float damage);
    }
    [RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public class BaseArrow : MonoBehaviour, IBaseArrow
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private AudioClip arrowSound;
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private float damageAmt = 0f;
        private Rigidbody rb;
        private AudioSource audioSource;
        private bool hasHit = false; //prevent infinity knockback + onCollisionEnter loop
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = arrowSound;
            audioSource.loop = true; //may need to adjust to only once
            audioSource.playOnAwake = false;
        }   
        private void Start()
        {
            //Temporary Disabled - Re-enable to add Velocitys
            rb.linearVelocity = transform.forward * speed;

            if (arrowSound != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        public void SetDamage(float damage)
        {
            damageAmt = damage;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (hasHit) return;
            
            if(collision.gameObject.CompareTag(enemyTag) && collision.gameObject.GetComponent<AI_ControllerComponent>() != null)
            {
                hasHit = true; 
                
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true; // freeze
                StopFlySound();

                var controller = collision.gameObject.GetComponent<AI_ControllerComponent>();
                Vector3 dir = (collision.transform.position - transform.position).normalized;
                controller.TakeDamage(dir, damageAmt);
                Destroy(gameObject);
            }

        }

        private void StopFlySound()
        {
            audioSource.Stop();
        }


    }
}
