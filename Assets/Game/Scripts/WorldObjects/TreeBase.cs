using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TreeBase : DestructibleObjectBase
{
    public override async void OnDestroyed()
    {
        Debug.Log($"{gameObject} has been destroyed");

        base.UnsubscribeFromDurability();
        StartFalling();

        await HandleTrunkFall();

        await base.PlayDestructionAnimation();

        base.HarvestAndCleanup();
    }

    private async UniTask HandleTrunkFall()
    {
        GameObject trunk = transform.Find("Trunk").gameObject;
        Rigidbody trunkRigidbody = trunk.AddComponent<Rigidbody>();
        SetupTrunkPhysics(trunkRigidbody);
        trunkRigidbody.AddForce(Vector3.right * 5000f);

        await UniTask.Delay(TimeSpan.FromSeconds(4f));
    }

    protected override void StartFalling()
    {
        _isFalling = true;
        _animator.enabled = false;
    }

    private void SetupTrunkPhysics(Rigidbody rigidbody)
    {
        if (rigidbody == null) return;

        rigidbody.mass = 25f;
        rigidbody.drag = 0.2f;
        rigidbody.angularDrag = 0.1f;
    }
}