using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

public class MagicalShieldController : AbstractHoldableMagicController
{
    [SerializeField] VisualEffect vfx;

    public override IMagicController Instantiate(CastingData data)
    {
        IMagicController instance = Instantiate(gameObject, data.owner.transform).GetComponent<IMagicController>();
        PostInstantiate(instance, data);
        return instance;
    }

    public override bool OnCast(CastingData data)
    {
        bool x = base.OnCast(data);
        owner.AddExcludable(singleton);
        return x;
    }

    public override bool OnCastEnd()
    {
        if (active)
        {
            vfx.Stop();
            owner.DeleteExcludable(singleton);
            transform.parent = null;
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
        if (active && !owner.KamiMode)
        {
            owner.AddKe(-KeCost * Time.deltaTime);
            if (owner.Ke == 0)
            {
                OnCastEnd();
            }
        }
    }
}
