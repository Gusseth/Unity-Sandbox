using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DummyCaster : MonoBehaviour
{
    [SerializeField] INoritoInventoryController inventoryController;
    [SerializeField] int delayInMilliseconds = 1000;
    [SerializeField] float castableSpeed = 5.0f;
    [SerializeField] bool rotateCastablesAfterCast;
    CancellationToken token;

    private void Awake()
    {
        inventoryController = GetComponent<SimpleNoritoInventoryController>();
        inventoryController.SetEquipped(0, transform);
        token = this.GetCancellationTokenOnDestroy();
    }

    private void Start()
    {
        TimeHelpers.InvokeAsync(ShootRoutine, delayInMilliseconds, token);
    }

    private CastingData MakeCastData()
    {
        return new CastingData
        {
            origin = transform,
            owner = gameObject,

            speed = castableSpeed,
            direction = transform.forward
        };
    }

    private void Cast()
    {
        inventoryController.OnCast(MakeCastData());
    }

    void ShootRoutine()
    {
        Cast();
        if (rotateCastablesAfterCast)
        {
            inventoryController.GetNextEquipped(transform);
        }

        TimeHelpers.InvokeAsync(ShootRoutine, delayInMilliseconds, token);
    }
}
