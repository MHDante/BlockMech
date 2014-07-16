using UnityEngine;
using System.Collections;
using System;
public class Door : Wall
{
    public ColorSlot colorslot = ColorSlot.A;
    public Color colorPreview;
    public bool isOpen { get { return isTraversible; } }

    void OnValidate()
    {
        SetColorSlot(colorslot);
    }
    public void SetColorSlot(ColorSlot colorSlot)
    {
        this.colorslot = colorSlot;
        colorPreview = Author.GetColorSlot(colorSlot);
        gameObject.GetComponent<SpriteRenderer>().color = colorPreview;
    }
    public void Open()
    {
        if (isOpen) return;
        isTraversible = true;

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void Close()
    {
        if (!isOpen) return;
        isTraversible = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

}
