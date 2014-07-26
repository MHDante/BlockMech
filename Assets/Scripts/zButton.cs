using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class zButton {

    public SpriteRenderer border;
    public virtual float Width { get { return 60; } }
    public virtual float Height { get { return 60; } }

    private Func<bool> drawCall;
    public float x, y;

    public event Action<zButton> OnClick;
    public event Action<zButton> LongPress;


    public static Dictionary<Type, List<SpriteTexture>> spriteCache = new Dictionary<Type, List<SpriteTexture>>();
    public zButton(Type type, int border = 10)
    {
        drawCall = delegate
        {
            Rect rect = new Rect(x, y, Width, Height);
            if (!spriteCache.ContainsKey(type))
            {
                string name = type.Name;
                Sprite[] multisprite = Resources.LoadAll<Sprite>("Sprites/" + name);
                List<SpriteTexture> excuses = new List<SpriteTexture>();

                foreach (var sprite in multisprite)
                {
                    excuses.Add(new SpriteTexture(sprite));
                    if (type == (typeof(Wall))) break;
                }

                spriteCache[type] = excuses;
            }
            List<SpriteTexture> sprTex = spriteCache[type];
            bool ret = GUI.RepeatButton(rect, "");
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
                    case 0: c = new Color(1f, 1f, 1f); break;
                }
                GUI.color = c;
                GUI.DrawTextureWithTexCoords(new Rect(rect.x + border, rect.y + border, rect.width - border * 2, rect.height - border * 2), sprTex[i].texture, sprTex[i].rect);
            }
            return ret;
        };
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

    public zButton(Texture t)
    {
        drawCall = delegate
        {
            return GUI.RepeatButton(new Rect(x, y, Width, Height), t);
        };
    }

    public zButton(string text)
    {
        drawCall = delegate
        {
            return GUI.RepeatButton(new Rect(x, y, Width, Height), text);
        };
    }
    bool prevClick;
    float downTime;
    bool longPressHappened;
    int falseCounter = 3;
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
                    Debug.Log("Longpress");

                }
            }
        }
        else
        {
            if (prevClick)
            {
                if (!longPressHappened)
                {
                    if (OnClick!=null)OnClick(this);
                    Debug.Log("onclick");
                }
            }
            longPressHappened = false;
        }

        prevClick = currentClick;
    }



}
