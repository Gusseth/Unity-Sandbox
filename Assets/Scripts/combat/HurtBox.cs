using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour, IHurtBox
{
    [SerializeField] bool isActive;

    [SerializeField] GameObject hurtboxOwner;

    public bool active { get => isActive; }
    public GameObject owner { get => hurtboxOwner; }

    public IGotHit hurtResponder { get; set; }

    public bool CheckHit(Hit data)
    {
        if (hurtResponder == null)
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
