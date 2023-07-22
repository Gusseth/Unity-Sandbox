using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHurtBoxResponder : MonoBehaviour, IGotHit
{
    List<IHurtBox> hurtBoxes;

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void Response(HitData data)
    {
        Debug.Log($"You hit me, {gameObject.name} , with {data.damage} damage. Normal:{data.normal}, Position:{data.point}");
    }

    // Start is called before the first frame update
    void Start()
    {
        hurtBoxes = new List<IHurtBox>(GetComponentsInChildren<IHurtBox>());
        foreach (IHurtBox box in hurtBoxes)
        {
            box.HurtResponder = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
