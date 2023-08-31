using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHurtBoxResponder : MonoBehaviour, IHitResponder
{
    [SerializeField] ActorBase actor;
    List<IHurtBox> hurtBoxes;

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void Response(HitData data)
    {
        if (actor != null) {
            actor.AddDamage(data);
        }
        Debug.Log($"You hit me, {gameObject.name}! My health is now {actor.Health}/{actor.MaxHealth}!");
    }

    // Start is called before the first frame update
    void Start()
    {
        hurtBoxes = new List<IHurtBox>(GetComponentsInChildren<IHurtBox>());
        foreach (IHurtBox box in hurtBoxes)
        {
            box.HurtResponder = this;
        }
        actor ??= GetComponent<ActorBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
