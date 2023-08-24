using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class MagicalShieldHurtboxResponder : MonoBehaviour, IBlocker
{
    [SerializeField] bool blocking;
    [SerializeField] bool parrying;
    [SerializeField] HitBoxLayer hitBoxLayer;
    [SerializeField] int parryTime = 1000;
    CancellationToken token;
    public bool Blocking { get => blocking; set => SetBlock(value); }
    public bool Parry { get => parrying; set => parrying = value; }

    public GameObject Owner => transform.parent.gameObject;

    public HitBoxLayer HitBoxLayer => hitBoxLayer;

    void SetBlock(bool value)
    {
        if (value)
            PreBlock(MathHelpers.NaN3);
        else
        {
            PostBlock();
        }
    }

    void ParryingTimeout()
    {
        if (Blocking)
            parrying = false;
    }

    public void PostBlock()
    {
        blocking = false;
    }

    public void PreBlock(float3 direction)
    {
        blocking = true;
        parrying = true;
        TimeHelpers.InvokeAsync(ParryingTimeout, parryTime, token);
    }

    public void SetSize(float3 size)
    {

    }

    // Start is called before the first frame update
    void Awake()
    {
        token = this.GetCancellationTokenOnDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
