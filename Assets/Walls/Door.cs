using UnityEngine;
using System.Collections;
using System;
public class Door : Wall
{
    public ColorSlot colorslot = ColorSlot.A;
    public Color colorPreview;
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
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
}
