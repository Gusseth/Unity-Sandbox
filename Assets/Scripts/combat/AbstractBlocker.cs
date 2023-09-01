using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public abstract class AbstractBlocker : MonoBehaviour, IBlocker
{
    [SerializeField] protected bool blocking;
    [SerializeField] protected bool parrying;
    [SerializeField] protected int parryTime;
    [SerializeField] protected HitBoxLayer layer;
    [SerializeField] protected AbstractActorBase user;
    protected CancellationTokenSource token;

    public bool Blocking { get => blocking; set => SetBlocking(value); }
    public bool Parry { get => parrying; set => SetParrying(value); }

    public HitBoxLayer HitBoxLayer => layer;
    public GameObject Owner => user.gameObject ?? null;

    public virtual bool CheckHit(HitData data)
    {
        return true;
    }

    public abstract void OnBlock(Block data);

    public abstract void OnParry(Block data);

    public virtual void PreBlock(float3 direction, AbstractActorBase actor)
    {
        user = actor;
        blocking = parrying = true;
        DisposeToken();
        token = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
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

    public virtual void Response(HitData data)
    {
        if (data is Block block)
        {
            if (block.parry)
                OnParry(block);
            else
                OnBlock(block);
        }
    }

    protected virtual void OnDifficultyChange(in Difficulty difficulty)
    {
        parryTime = difficulty.ParryTime;
    }

    protected virtual void OnEnable()
    {
        parryTime = Root.Instance.Difficulty.ParryTime;
        Root.DifficultyChangeEvent += OnDifficultyChange;
    }

    protected virtual void OnDisable()
    {
        Root.DifficultyChangeEvent -= OnDifficultyChange;
    }

    protected virtual void ParryTimeout()
    {
        Debug.Log("Stopped parrying");
        parrying = false;
        DisposeToken();
    }

    protected virtual void Awake() { }

    private async UniTask ParryTimeoutRoutine(bool useUnscaledTime = false)
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

    private void SetBlocking(bool value)
    {
        if (value)
            PreBlock(MathHelpers.NaN3, user);
        else
        {
            PostBlock();
        }
    }

    private void SetParrying(bool value)
    {
        if (value)
            PreBlock(MathHelpers.NaN3, user);
        else
        {
            parrying = false;
        }
    }
}