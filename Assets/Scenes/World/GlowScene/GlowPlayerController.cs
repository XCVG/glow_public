using CommonCore;
using CommonCore.Input;
using CommonCore.LockPause;
using CommonCore.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowPlayerController : MonoBehaviour
{
    const float DEADZONE = 0.01f;

    public GameObject ShipObject = null;

    [Header("movement")]
    public float TranslateSpeed = 5f;
    public float MaxDisplacement = 6f;

    [Header("shooting")]
    public GameObject GlowStickPrefab;
    public Transform ShootPoint;
    public float BaseVelocity = 12f;
    public float Cooldown = 0.25f;
    public float XAngle = 60f;
    public float YBase = 0.1f;
    public float YFactor = 0.1f;
    public float YVelocity = 5f;

    private GlowSceneController GlowSceneController;
    private float TimeToNextShot = 0;

    private void Start()
    {
        GlowSceneController = CoreUtils.GetWorldRoot().GetComponent<GlowSceneController>();

    }

    private void Update()
    {
        if (LockPauseModule.IsPaused() || GlowSceneController.State != GlowSceneController.GlowSceneState.Running)
            return;

        if (TimeToNextShot > 0)
            TimeToNextShot -= Time.deltaTime;

        float moveVec = MappedInput.GetAxis(DefaultControls.MoveX);
        if(Mathf.Abs(moveVec) > DEADZONE)
        {
            //Debug.Log(moveVec);

            if(!(moveVec < 0 && ShipObject.transform.position.x <= -MaxDisplacement) && !(moveVec > 0 && ShipObject.transform.position.x >= MaxDisplacement))
                ShipObject.transform.Translate(Vector3.right * Time.deltaTime * TranslateSpeed * moveVec, Space.World);

        }

        if(MappedInput.GetButton(DefaultControls.Fire) && TimeToNextShot <= 0)
        {
            //var ray = WorldUtils.GetActiveCamera().ScreenPointToRay(Input.mousePosition);
            //var collisions = Physics.RaycastAll(ray, 100f, LayerMask.GetMask("Default", "ActorHitbox"), QueryTriggerInteraction.Collide);

            var rawMP = Input.mousePosition;
            float halfW = Screen.width / 2f;
            float halfH = Screen.height / 2f;
            Vector2 relativeMP = new Vector2(Mathf.Clamp((rawMP.x - halfW) / halfW, -1, 1), Mathf.Clamp((rawMP.y - halfH) / halfH, -1, 1));
            //Debug.Log(relativeMP.ToString("F2"));

            float xAngle = relativeMP.x * XAngle;
            float yFactorHacky = (relativeMP.y + 1) * YFactor;
            float yVelocityHacky = (relativeMP.y + 1) * YVelocity;

            var bullet = GameObject.Instantiate(GlowStickPrefab, CoreUtils.GetWorldRoot());
            bullet.SetActive(true);
            bullet.transform.position = ShootPoint.position;
            var rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = (Quaternion.AngleAxis(xAngle, Vector3.up) * (new Vector3(0, YBase + yFactorHacky, 1f)).normalized).normalized * (BaseVelocity + yVelocityHacky);

            WorldUtils.SpawnEffect("GlowFireEffect", ShootPoint.position, Quaternion.identity, null, false);

            TimeToNextShot = Cooldown;
        }
    }
}
