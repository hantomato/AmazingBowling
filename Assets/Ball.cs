using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public LayerMask whatIsProp;

    public ParticleSystem explosionParticle;
    
    public AudioSource explosionAudio;

    public float maxDamage = 100f;          // 최대 데미지 크기

    public float explosionForce = 1000f;    // 1000 force 의 힘을 뿌리다

    public float lifeTime = 10f;      // 10초 뒤에는 자동으로 파괴

    public float explosionRadius = 20f;     // 폭발 반경 20미터

    public void Start()
    {
        Destroy(gameObject, lifeTime);      // 적어도 일정시간 뒤에는 파괴되도록. (버그 방지)
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, whatIsProp);

        for (int i = 0; i < colliders.Length; ++i)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // AddExplosionForce : 내가 얼마나 팅겨져야하는지 계산해서 적용해준다.
            // void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius);
            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Prop targetProp = colliders[i].GetComponent<Prop>();
            float damage = CalculateDamage(colliders[i].transform.position);

            targetProp.TakeDamage(damage);
        }


        explosionParticle.transform.parent = null;
        explosionParticle.Play();
        explosionAudio.Play();

        Destroy(explosionParticle.gameObject, explosionParticle.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float distance = explosionToTarget.magnitude;

        float edgeToCenterDistance = explosionRadius - distance;
        float percentage = edgeToCenterDistance / explosionRadius;
        float damage = maxDamage * percentage;
        damage = Mathf.Max(0, damage);
        return damage;
    }
}
