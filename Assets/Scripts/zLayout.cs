using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class zGridLayout<T> where T : zButton {
    private enum State { Growing, Shrinking, Hidden , Visible}
    State state;
    public GUISkin skin;
    List<T> contents;
    Vector2 scrollViewVector;
    Rect properRect;
    Rect currentRect;
    Rect virtualRect;
    Vector2 origin;
    float duration;
    float t;
    float heightCounter;
    float padding;
    
    public zGridLayout(Rect rect, IEnumerable<T> contents, bool startHidden, Vector2? animOrigin = null, float animDuration = 0.5f, float padding = 10)
    {
        this.properRect = rect;
        this.contents = contents.ToList();
        this.origin = animOrigin?? new Vector2(rect.x, rect.y);
        this.currentRect = new Rect(origin.x, origin.y, 0, 0);
        this.duration = animDuration;
        this.state = startHidden ? State.Hidden : State.Growing;
        this.padding = padding;

        heightCounter += padding;

        //first pass
        List<int> indexArray = new List<int>();
        List<Vector2> rowSize = new List<Vector2>();
        float widthCounter = padding, tempHeight = 0;
        int counter = 0;
        for(int i = 0; i < this.contents.Count;i++)
        {
            counter++;
            T element = this.contents[i];
            if (element.Width + widthCounter > rect.width)
            {
                heightCounter += tempHeight + padding;
                indexArray.Add(counter);
                rowSize.Add(new Vector2(widthCounter, tempHeight));
                counter = 0;
                tempHeight = 0;
                widthCounter = padding;
            }
            else
            {
                widthCounter += element.Width + padding;
            }
            if (element.Height > tempHeight) tempHeight = element.Height;
            if (element.Width > rect.width - padding * 2)
            {
                //throw new SystemException("Button too big. (or, conversly, zLayout too small)");
                Debug.Log("Button too big. (or, conversly, zLayout too small)");
            }
        }
        heightCounter += tempHeight;
        this.virtualRect = new Rect(0, 0, properRect.width, heightCounter + padding);

        counter = 0;
        heightCounter = padding;
        //second pass
        for (int i = 0; i < indexArray.Count; i++)
        {
            int index = indexArray[i];
            int end = counter + index;
            float wCounter = (this.properRect.width - rowSize[i].x) / 2 + padding;

            while(counter<end)
            {
                T element = this.contents[counter];
                element.x = wCounter; wCounter += element.Width + padding;
                element.y = heightCounter;
                counter++;
            }

            heightCounter += rowSize[i].y + padding;
        }
    }

    private void scaleRect()
    {
        currentRect = new Rect(
                    Utils.SmootherStep(origin.x, properRect.x, t),
                    Utils.SmootherStep(origin.y, properRect.y, t),
                    Utils.SmootherStep(0, properRect.width, t),
                    Utils.SmootherStep(0, properRect.height, t));
    }

    public void Show() { state = State.Growing; }
    public void Hide() { state = State.Shrinking; }


    public void Draw()
    {
        switch (state)
        {
            case State.Hidden:
                return;
            case State.Growing:
                t += Time.deltaTime / duration;
                if (t > 1f) { t = 1f; state = State.Visible; }
                scaleRect();
                break;
            case State.Shrinking:
                t -= Time.deltaTime / duration;
                if (t > 1f) { t = 1f; state = State.Hidden; }
                scaleRect();
                break;
        }

        
        scrollViewVector = GUI.BeginScrollView(currentRect, scrollViewVector, virtualRect);

        foreach (T Element in contents)
        {
            Element.Draw();
        }

        GUI.EndScrollView(true);

    }
}