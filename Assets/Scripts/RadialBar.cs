using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Origin { Bottom, Right, Top, Left }

public class RadialBar : MonoBehaviour
{
    public Image barImage;
    public RectTransform mask;
    public Image startTip;
    public Image endTip;

    [NaughtyAttributes.OnValueChanged("OnColorChanged")]
    public Color color;
    
    [Range(0.01f, 0.99f)]
    [NaughtyAttributes.OnValueChanged("OnBarWidthChanged")]
    public float barWidthPercent;
    
    [NaughtyAttributes.OnValueChanged("OnOriginChanged")]
    public Origin origin;
    
    [Range(0, 1)]
    [NaughtyAttributes.OnValueChanged("OnFillAmountChanged")]
    public float fillAmount;
    
    [NaughtyAttributes.OnValueChanged("OnClockwiseChanged")]
    public bool clockwise;

    void OnColorChanged()
    {
        barImage.color = color;
        startTip.color = color;
        endTip.color = color;
    }
    
    void OnBarWidthChanged()
    {
        float halfBarWidth = barWidthPercent * 0.5f;
        Vector2 minAnchor = new Vector2(halfBarWidth, halfBarWidth);
        Vector2 maxAnchor = new Vector2(1 - halfBarWidth, 1 - halfBarWidth);
        mask.anchorMin = minAnchor;
        mask.anchorMax = maxAnchor;

        UpdateTips();
    }
    
    void OnOriginChanged()
    {
        barImage.fillOrigin = (int) origin;
        UpdateTips();
    }
    
    void OnFillAmountChanged()
    {
        barImage.fillAmount = fillAmount;
        UpdateTips();
    }
    
    void OnClockwiseChanged()
    {
        barImage.fillClockwise = clockwise;
        UpdateTips();
    }

    [NaughtyAttributes.Button()]
    void UpdateTips()
    {
        Vector2 originDir;
        float angleOffset;
        switch (origin)
        {
            case Origin.Bottom:
                originDir = Vector2.down;
                angleOffset = clockwise ? 90 : 270;
                break;
            case Origin.Left:
                originDir = Vector2.left;
                angleOffset = 180;
                break;
            case Origin.Top:
                originDir = Vector2.up;
                angleOffset = clockwise ? 270 : 90;
                break;
            case Origin.Right:
                originDir = Vector2.right;
                angleOffset = 0;
                break;
            default:
                originDir = Vector2.down;
                angleOffset = 270;
                break;
        }

        // cálculos para actualizar posición de la punta inicial
        float barCenterFactor = (1 - barWidthPercent) * 0.5f + barWidthPercent * 0.25f;
        Vector2 startAnchorPos = Vector2.one * 0.5f + originDir * barCenterFactor;
        
        // cálculos para actualizar posición de la punta final
        float alt = clockwise ? -1 : 1;
        float angle = (360 * fillAmount + angleOffset) % 360 * alt;
        float angleRadians = angle * Mathf.Deg2Rad;
        Vector2 endDir = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
        Vector2 endAnchorPos = Vector2.one * 0.5f + endDir * barCenterFactor;
        
        // cálculos para actualizar grosor
        float width = barWidthPercent * 0.25f;
        Vector2 anchorsOffset = Vector2.one * width;
        
        // actualización de anchors
        var startTransform = startTip.rectTransform;
        var endTransform = endTip.rectTransform;
        startTransform.anchorMin = startAnchorPos - anchorsOffset;
        startTransform.anchorMax = startAnchorPos + anchorsOffset;
        endTransform.anchorMin = endAnchorPos - anchorsOffset;
        endTransform.anchorMax = endAnchorPos + anchorsOffset;
    }

    void Start()
    {
        barImage.fillMethod = Image.FillMethod.Radial360;
        
        UpdateTips();
    }
    
}
