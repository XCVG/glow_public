using CommonCore;
using CommonCore.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowTargetScript : MonoBehaviour
{
    //TODO max hits?

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if(other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            CoreUtils.GetWorldRoot().GetComponent<GlowSceneController>().HandleTargetHit();

            var effect = WorldUtils.SpawnEffect("GlowstickHitEffect", other.transform.position, Quaternion.identity, null, false);
            if(effect != null)
            {
                var es = effect.GetComponent<GlowstickHitEffectScript>();
                var gr = other.gameObject.GetComponentInChildren<GlowstickRandomizer>();
                if(es != null && gr != null)
                {
                    es.SetHue(gr.Hue);
                }
            }

            Destroy(other.gameObject);
        }
    }
}
