using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button, IPointerClickHandler
{
    public UnityEvent onLeftClick = new UnityEvent();
    public UnityEvent onRightClick = new UnityEvent();
    public UnityEvent onMiddleClick = new UnityEvent();
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onLeftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            onMiddleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            onRightClick.Invoke();
        
    }

    protected override void OnDestroy()
    {
        onLeftClick.RemoveAllListeners();
        onRightClick.RemoveAllListeners();
        onMiddleClick.RemoveAllListeners();
        base.OnDestroy();
    }
}
