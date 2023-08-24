using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicImpact : MonoBehaviour
{
    [SerializeField] bool armed;
    [SerializeField] bool impactHasForce;
    [SerializeField] bool isExplosive;
    [SerializeField] bool ignorePlayer;
    [SerializeField] float forceStrength;

    void OnHit(Collision collision)
    {
        if (armed)
        {
            if (impactHasForce)
            {

                ContactPoint contact = collision.contacts[0];
                Vector3 position = contact.point;
                float explosiveDistance = Mathf.Sqrt(forceStrength);

                if (isExplosive)
                {
                    Collider[] colliders = Physics.OverlapSphere(position, explosiveDistance);
                    Debug.Log("Hit");

                    foreach (Collider collider in colliders)
                    {
                        if (ignorePlayer && collider.gameObject.tag == "Player")
                            continue;

                        if (collider.gameObject.tag == "Projectile")
                            continue;

                        collider.TryGetComponent(out Rigidbody rb);

                        if (rb)
                        {
                            rb.AddExplosionForce(forceStrength, position, explosiveDistance, 0.0f, ForceMode.Impulse);
                        }
                    }
                }
                else
                {
                    contact.otherCollider.TryGetComponent(out Rigidbody rb);
                    if (rb)
                    {
                        Vector3 explosiveForce = -contact.normal * forceStrength;
                        rb.AddForceAtPosition(explosiveForce, position, ForceMode.Impulse);
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit(collision);
    }
}
