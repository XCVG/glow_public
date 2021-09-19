using CommonCore;
using CommonCore.LockPause;
using CommonCore.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowObstacleScript : MonoBehaviour
{
    public string DestroyEffect;
    public string BumpEffect;
    public float FlingVelocity = 30f;
    public float FlingSpinMultiplier = 30f;
    public float AfterHitRetainTime = 10f;
    public float AfterHitEffectDelayTime = 0.2f;

    private bool WasHit = false;
    private float Elapsed = 0;

    private void Start()
    {
        

    }

    private void Update()
    {
        if (LockPauseModule.IsPaused())
            return;

        if (WasHit)
        {
            Elapsed += Time.deltaTime;

            if(Elapsed >= AfterHitRetainTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.transform.gameObject.name);

        if (collision.transform.gameObject.name.Equals("PlayerShip", StringComparison.OrdinalIgnoreCase))
            HandlePlayerCollision(collision.GetContact(0).point, collision.gameObject);
        else
            HandleOtherCollision(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if(other.gameObject.name.Equals("PlayerShip", StringComparison.OrdinalIgnoreCase))
            HandlePlayerCollision(other.ClosestPoint(transform.position), other.gameObject);
    }

    private void HandlePlayerCollision(Vector3 hitPoint, GameObject other)
    {
        if (WasHit)
            return;

        transform.SetParent(CoreUtils.GetWorldRoot()); //reparent!

        float dir = Mathf.Sign(other.transform.position.x - transform.position.x);
        Vector3 flingDirection = new Vector3(UnityEngine.Random.Range(0.1f, 0.3f) * -dir, UnityEngine.Random.Range(0.5f, 1f), 1f).normalized;

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; //doesn't do anything because collider is kinematic, oh well
        rb.AddForce(flingDirection * FlingVelocity, ForceMode.VelocityChange);

        Vector3 torque = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * FlingSpinMultiplier;
        rb.AddTorque(torque, ForceMode.VelocityChange);

        var collider = GetComponent<Collider>();
        collider.isTrigger = false;

        WorldUtils.SpawnEffect(DestroyEffect, hitPoint, Quaternion.identity, null, false);

        CoreUtils.GetWorldRoot().GetComponent<GlowSceneController>().HandleObstacleHit();

        WasHit = true;
    }

    private void HandleOtherCollision(Collision collision)
    {
        if (!WasHit || Elapsed < AfterHitEffectDelayTime) //only meant for post-hit collisions
            return;

        WorldUtils.SpawnEffect(BumpEffect, collision.GetContact(0).point, Quaternion.identity, null, false);
    }
}
