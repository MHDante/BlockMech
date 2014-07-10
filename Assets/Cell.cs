using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Side { top, right, bottom, left };
public class Cell {

    public int x { get; set; }
    public int y { get; set; }

    public Dictionary<Side, Wall> walls = new Dictionary<Side, Wall>();

    public GamePiece gamePiece;

    public Cell()
    {
        foreach (Side s in Enum.GetValues(typeof(Side)))
        {
            walls[s] = null;
        }
	}
}
