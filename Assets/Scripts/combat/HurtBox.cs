using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour, IHurtBox
{
    [SerializeField] bool isActive;

    [SerializeField] GameObject hurtboxOwner;

    public bool Active { get => isActive; }
    public GameObject Owner { get => hurtboxOwner; }

    public IGotHit HurtResponder { get; set; }

    public bool CheckHit(HitData data)
    {
        if (HurtResponder == null)
            Debug.Log("No responder");
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
