using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

public class MagicalShieldController : AbstractMagicController
{
    IDamagableActor owner;
    [SerializeField] VisualEffect vfx;
    [SerializeField] bool active;

    public override void Init()
    {

    }

    public override bool OnCastStart(CastingData data)
    {
        owner = data.ownerActor;
        active = true;
        return true;
    }

    public override bool OnCastEnd()
    {
        vfx.Stop();
        active = false;
        _ = TimeHelpers.WaitUntilNoParticles(vfx, this.GetCancellationTokenOnDestroy(), DestroyCastable);
        return true;
    }

    public void OCEnd()
    {
        OnCastEnd();
    }

    public override bool CheckRequirements(CastingData data)
    {
        return base.CheckRequirements(data);
    }

    private void Start()
    {
        //TimeHelpers.InvokeAsync(OCEnd, 5000, this.GetCancellationTokenOnDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            owner.AddKe(-KeCost * Time.deltaTime);
        }
    }
}
