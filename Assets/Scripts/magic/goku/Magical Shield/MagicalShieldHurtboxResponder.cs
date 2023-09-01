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
    [SerializeField] int parryTime;
    [SerializeField] AbstractActorBase gokuOwner;
    CancellationToken token;
    
    public bool Blocking { get => blocking; set => SetBlock(value); }
    public bool Parry { get => parrying; set => parrying = value; }

    public GameObject Owner => gokuOwner ? gokuOwner.gameObject : null;

    public HitBoxLayer HitBoxLayer => hitBoxLayer;

    void SetBlock(bool value)
    {
        if (value)
            PreBlock(MathHelpers.NaN3, gokuOwner);
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

    public void PreBlock(float3 direction, AbstractActorBase actor)
    {
        blocking = true;
        parrying = true;
        gokuOwner = actor;
        parryTime = Root.Instance.Difficulty.ParryTime;
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

    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void Response(HitData data)
    {
        if (data is Block block)
        {
            if (block.parry)
                OnParry(block);
            else
                OnBlock(block);
        }
    }

    public void OnBlock(Block data)
    {
        Debug.Log($"Blocked \"{data.attacker.Hitter.Name}\"");
    }

    public void OnParry(Block data)
    {
        Debug.Log($"Parried \"{data.attacker.Hitter.Name}\"");
    }

    void OnDifficultyChange(in Difficulty difficulty)
    {
        if (Blocking)
        {
            parryTime = difficulty.ParryTime;
        }
    }

    void OnEnable()
    {
        parryTime = Root.Instance.Difficulty.ParryTime;
        Root.DifficultyChangeEvent += OnDifficultyChange;
    }

    void OnDisable()
    {
        Root.DifficultyChangeEvent -= OnDifficultyChange;
    }
}
