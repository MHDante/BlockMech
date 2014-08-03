using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using OrbItUtils;


public class zButton {

    public SpriteRenderer border;
    public virtual float Width { get; set; }
    public virtual float Height { get; set; }

    private Func<bool> drawCall;
    public float x, y;
    public bool activated { get; set; }
    public event Action<zButton> OnClick;
    public event Action<zButton> LongPress;
    public Color color = Color.white;
    public Type type;
    public static Dictionary<Type, List<SpriteTexture>> spriteCache = new Dictionary<Type, List<SpriteTexture>>();
    static GUIStyle activeStyle;
    public zButton(Type type, float Width, float Height, int border = 10)
    {
        this.Width = Width;
        this.Height = Height;
        this.type = type;
        drawCall = delegate
        {
            Rect rect = new Rect(x, y, Width, Height);
            UpdateSpriteCache();
            List<SpriteTexture> sprTex = spriteCache[this.type];
            bool ret = false;
            if (activated)
            {
                ret = GUI.RepeatButton(rect, "", activeStyle);
            }
            else
            {
                ret = GUI.RepeatButton(rect, "");
            }
            
            for (int i = sprTex.Count - 1; i >= 0; i--)
            {
                Color c = Color.black;
                switch (i)
                {
                    case 5: c = new Color(0.3f, 0.3f, 0.3f); break;
                    case 4: c = new Color(0.7f, 0.7f, 0.7f); break;
                    case 3: c = new Color(0.4f, 0.4f, 0.4f); break;
                    case 2: c = new Color(0.8f, 0.8f, 0.8f); break;
                    case 1: c = new Color(0.5f, 0.5f, 0.5f); break;
                    case 0: c = new Color(1.0f, 1.0f, 1.0f); break;
                }
                Color temp = GUI.color;
                GUI.color = c * color;
                GUI.DrawTextureWithTexCoords(new Rect(rect.x + border, rect.y + border, rect.width - border * 2, rect.height - border * 2), sprTex[i].texture, sprTex[i].rect);
                GUI.color = temp;
            }
            return ret;
        };
    }
    void UpdateSpriteCache()
    {
        if (!spriteCache.ContainsKey(this.type))
        {
            string name = this.type.Name;
            Sprite[] multisprite = Resources.LoadAll<Sprite>("Sprites/" + name);
            List<SpriteTexture> excuses = new List<SpriteTexture>();

            foreach (var sprite in multisprite)
            {
                excuses.Add(new SpriteTexture(sprite));
                if (this.type == (typeof(Wall))) break;
            }

            spriteCache[this.type] = excuses;
        }
        if (activeStyle == null)
        {
            activeStyle = CreateActiveStyle();
        }
    }
    GUIStyle CreateActiveStyle(string text = "")
    {
        var style = GUI.skin.GetStyle("Button");
        var newStyle = new GUIStyle(style);
        newStyle.normal.background = style.focused.background;
        newStyle.hover.background = style.focused.background;
        if (text != "")
        {
            newStyle.fontSize = (int)(Width / (text.Length + 1));
        }
        return newStyle;
        //activeStyle.normal.background = activeStyle.active.background;
    }
    public zButton(Texture t, float Width, float Height)
    {
        this.Width = Width;
        this.Height = Height;
        drawCall = delegate
        {
            return GUI.RepeatButton(new Rect(x, y, Width, Height), t);
        };
    }
    GUIStyle customButton, activeCustomButton;
    public zButton(string text, float Width, float Height)
    {
        this.Width = Width;
        this.Height = Height;
        drawCall = delegate
        {
            if (customButton == null || activeCustomButton == null)
            {
                customButton = new GUIStyle("button");
                customButton.fontSize = (int)(Width / (text.Length + 1));
                activeCustomButton = CreateActiveStyle(text);
            }
            bool ret = false;
            Rect rect = new Rect(x, y, Width, Height);
            if (activated)
            {
                ret = GUI.RepeatButton(rect, text, activeCustomButton);
            }
            else
            {
                ret = GUI.RepeatButton(rect, text, customButton);
            }
            return ret;
            //return GUI.RepeatButton(rect, text, customButton);
        };
    }
    bool prevClick;
    float downTime;
    bool longPressHappened;
    int falseCounter = 3;
    KeyCode lastRelease = KeyCode.None;
    public void Draw()
    {
        bool currentClick = drawCall();
        //Debug.Log("CurrentClick " + currentClick);
        if (!currentClick)
        {
            falseCounter++;
            if (falseCounter <= 2)
            {
                currentClick = true;
            }
        }
        else
        {
            falseCounter = 0;
        }
        //Debug.Log("CurrentClick " + currentClick);
        if (currentClick)
        {
            if (!prevClick)
            {
                downTime = Time.time;
                //Debug.Log("downtime");
                
            }
            else
            {
                if (Time.time - downTime > 0.5 && !longPressHappened)
                {
                    if (LongPress != null) LongPress(this);
                    longPressHappened = true;
                    //Debug.Log("Longpress");

                }
            }
        }
        else
        {
            if (prevClick)
            {
                if (!longPressHappened)
                {
                    if (lastRelease == KeyCode.Mouse0)
                    {
                        if (OnClick != null) OnClick(this);
                        //Debug.Log("onclick");
                    }
                    else if (lastRelease == KeyCode.Mouse1)
                    {
                        if (LongPress != null) LongPress(this);
                        //longPressHappened = true;
                        //Debug.Log("Longpress");
                    }
                    
                }
            }
            longPressHappened = false;
        }

        prevClick = currentClick;
        if (Input.GetKeyUp(KeyCode.Mouse0)) lastRelease = KeyCode.Mouse0;
        else if (Input.GetKeyUp(KeyCode.Mouse1)) lastRelease = KeyCode.Mouse1;
    }
}
public class SpriteTexture
{
    public Texture texture;
    public Rect rect;
    public SpriteTexture(Sprite sprite)
    {
        texture = sprite.texture;
        Rect tr = sprite.textureRect;
        rect = new Rect(tr.x / texture.width, tr.y / texture.height, tr.width / texture.width, tr.height / texture.height);
    }
}