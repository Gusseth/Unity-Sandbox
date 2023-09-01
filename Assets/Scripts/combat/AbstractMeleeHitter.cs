using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public abstract class AbstractMeleeHitter : AbstractHitter, IBlocker
{
    [SerializeField] protected bool blocking;
    [SerializeField] protected bool parrying;
    [SerializeField] protected int parryTime;
    protected CancellationTokenSource token;
    protected CancellationToken destroyToken;

    public bool Blocking { get => blocking; set => blocking = value; }
    public bool Parry { get => parrying; set => parrying = value; }

    public override void OnBlocked(Block data)
    {
        Debug.Log("You got blocked");
    }

    public override void OnParried(Block data)
    {
        Debug.Log("Massive L");
        OnBlocked(data);
    }

    public virtual void OnBlock(Block data)
    {
        Debug.Log("Blocked something");
    }

    public virtual void OnParry(Block data)
    {
        Debug.Log("Parried something");
        OnBlock(data);
    }

    public virtual void PreBlock(float3 direction, AbstractActorBase actor)
    {
        Debug.Log("Started blocking");
        user = actor;
        blocking = parrying = true;
        DisposeToken();
        token = CancellationTokenSource.CreateLinkedTokenSource(destroyToken);
        _ = ParryTimeoutRoutine();
    }

    public virtual void PostBlock()
    {
        Debug.Log("Stopped blocking.");
        if (token != null)
        {
            token.Cancel();
        }
        blocking = parrying = false;
    }

    public override void OnHit(Hit data)
    {

    }

    protected override void ChooseBlockFunction(Block data)
    {
        if (ReferenceEquals(data.blocker, this))
        {
            // If we are the one who blocked
            if (data.parry)
                OnParry(data);
            else
                OnBlock(data);
        }
        else
        {
            if (data.parry)
                OnParried(data);
            else
                OnBlocked(data);
        }
    }

    protected virtual void ParryTimeout()
    {
        Debug.Log("Stopped parrying");
        parrying = false;
        DisposeToken();
    }

    protected async UniTask ParryTimeoutRoutine(bool useUnscaledTime = false)
    {
        try
        {
            await UniTask.Delay(parryTime, useUnscaledTime, cancellationToken: token.Token);
        }
        catch (OperationCanceledException)
        {
            DisposeToken();
            return;
        }

        ParryTimeout();
    }

    private void DisposeToken()
    {
        if (token != null)
        {
            token.Dispose();
            token = null;
        }
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        destroyToken = this.GetCancellationTokenOnDestroy();
    }
}
