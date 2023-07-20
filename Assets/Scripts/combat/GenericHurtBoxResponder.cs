using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHurtBoxResponder : MonoBehaviour, IGotHit
{
    List<IHurtBox> hurtBoxes;

    public bool CheckHit(Hit data)
    {
        return true;
    }

    public void Response(Hit data)
    {
        Debug.Log($"You hit me with {data.damage} damage.");
    }

    // Start is called before the first frame update
    void Start()
    {
        hurtBoxes = new List<IHurtBox>(GetComponentsInChildren<IHurtBox>());
        foreach (IHurtBox box in hurtBoxes)
        {
            box.hurtResponder = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
