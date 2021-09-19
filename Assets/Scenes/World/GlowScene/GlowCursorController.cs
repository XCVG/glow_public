using CommonCore.LockPause;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlowCursorController : MonoBehaviour
{
    [SerializeField]
    private Texture2D CursorTexture;

    private CanvasScaler CanvasScaler;
    private RectTransform RectTransform;

    private void Start()
    {
        CanvasScaler = GetComponentInParent<CanvasScaler>();
        RectTransform = (RectTransform)transform;
    }

    private void OnEnable()
    {
        Cursor.SetCursor(CursorTexture, new Vector2(CursorTexture.width / 2, CursorTexture.height / 2), CursorMode.ForceSoftware);
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void LateUpdate()
    {
        var rawMP = Input.mousePosition;

        var normalizedMP = new Vector2(Mathf.Clamp01(rawMP.x / Screen.width), Mathf.Clamp01(rawMP.y / Screen.height));

        var scaledMP = new Vector2(normalizedMP.x * CanvasScaler.referenceResolution.x, normalizedMP.y * CanvasScaler.referenceResolution.y);

        RectTransform.anchoredPosition = scaledMP;
    }
}
