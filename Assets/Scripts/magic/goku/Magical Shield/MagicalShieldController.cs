using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class MagicalShieldController : AbstractHoldableMagicController
{
    [SerializeField] VisualEffect vfx;
    [SerializeField] MagicalShieldHurtboxResponder hurtBox;

    [SerializeField] float3 size;
    float3 Size { get => size; set => SetSize(value); }

    const int vfx_size_id = 2;

    void SetSize(float3 size)
    {
        this.size = size;
        vfx.SetVector3(vfx_size_id, size);
        hurtBox.SetSize(size);
    }

    public override IMagicController Instantiate(CastingData data)
    {
        IMagicController instance = Instantiate(gameObject, data.owner.transform).GetComponent<IMagicController>();
        PostInstantiate(instance, data);
        return instance;
    }

    public override bool OnCast(CastingData data)
    {
        bool x = base.OnCast(data);
        if (owner != null)
            owner.AddExcludable(singleton);
        hurtBox.PreBlock(MathHelpers.NaN3);
        return x;
    }

    public override bool OnCastEnd()
    {
        if (active)
        {
            vfx.Stop();
            if (owner != null)
                owner.DeleteExcludable(singleton);
            transform.parent = null;
            hurtBox.PostBlock();
            active = false;
            _ = TimeHelpers.WaitUntilNoParticles(vfx, this.GetCancellationTokenOnDestroy(), DestroyCastable);
        }
        return true;
    }

    public override void DestroyCastable()
    {
        base.DestroyCastable();
    }

    public override bool CheckRequirements(CastingData data)
    {
        return base.CheckRequirements(data) && !data.ownerActor.ExcludableExists(singleton);
    }

    private void Start()
    {
        //TimeHelpers.InvokeAsync(OCEnd, 5000, this.GetCancellationTokenOnDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        if (active && owner != null && !owner.KamiMode)
        {
            owner.AddKe(-KeCost * Time.deltaTime);
            if (owner.Ke == 0)
            {
                OnCastEnd();
            }
        }
    }
}
