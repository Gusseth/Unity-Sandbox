using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BasicImpact : MonoBehaviour
{
    [SerializeField] bool active;
    [SerializeField] bool impactHasForce;
    [SerializeField] bool ignorePlayer;
    [SerializeField] float forceStrength;
    // Start is called before the first frame update

    void OnHit(Collision collision)
    {
        if (active)
        {
            if (!ignorePlayer && collision.gameObject.tag == "Player")
                return;

            if (impactHasForce)
            {
                ContactPoint contact = collision.contacts[0];
                float explosiveDistance = Mathf.Sqrt(forceStrength);

                contact.otherCollider.TryGetComponent(out Rigidbody rb);

                if (rb)
                {
                    Vector3 position = contact.point;
                    Vector3 explosiveForce = -contact.normal * forceStrength;

                    rb.AddForceAtPosition(explosiveForce * forceStrength, position, ForceMode.Impulse);
                }
                //Physics.OverlapSphere(position, )
            }

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit(collision);
    }
}
