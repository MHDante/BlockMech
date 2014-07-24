using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.IO;
using System;
using System.Linq;

public class FileWrite : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //InitSerialization();
	}

    public void InitSerialization()
    {
        XElement all = SerializeGrid();
        Debug.Log(all);

        string filename = "xmlfile.xml";

        WriteFile(filename, all.ToString());


        //XElement loaded = XElement.Load(Application.persistentDataPath + "/" + filename);
        //foreach(XElement element in loaded.Elements())
        //{
        //    //Debug.Log(element);
        //    Type t = Type.GetType(element.Name.ToString());
        //    if (t == null) throw new WTFException("Type was null while parsing xml");
        //    if (RoomManager.pieceEnums.ContainsKey(t))
        //    {
        //        
        //    }
        //}
    }



    public XElement SerializeGrid()
    {
        XElement eRoot = new XElement("Root");

        XElement eInfo = new XElement("Meta");
        eRoot.Add(eInfo);
        MetaData auth = FindObjectOfType<MetaData>();
        if (auth != null)
        {
            eInfo.Add(new XAttribute("Author", auth.author));
            eInfo.Add(new XAttribute("LevelName", auth.levelName));
            eInfo.Add(new XAttribute("Difficulty", auth.difficulty));
            
        }

        XElement eGrid = new XElement("Grid");
        eRoot.Add(eGrid);

        var grid = RoomManager.roomManager.Grid;
        for(int y = 0; y < grid[0].Length; y++)
        {
            XElement eRow = new XElement("Row", new XAttribute("y",y));
            bool haspiece = false;
            for(int x = 0; x < grid.Length; x++)
            {
                Cell cell = grid[x][y];
                if (cell.HasPiece())
                {
                    haspiece = true;
                    XElement eCell = new XElement("Cell", new XAttribute("x", x), new XAttribute("y", y));
                    foreach(GamePiece piece in cell.pieces)
                    {
                        XElement ePiece = SerializeObject(piece);
                        eCell.Add(ePiece);
                    }
                    eRow.Add(eCell);
                }
            }
            if (haspiece)
            {
                eGrid.Add(eRow);
            }
        }
        XElement eWalls = new XElement("Walls");
        eRoot.Add(eWalls);

        for (int y = 0; y < grid[0].Length; y++)
        {
            for (int x = 0; x < grid.Length; x++)
            {
                Cell cell = grid[x][y];
                foreach(Side s in cell.walls.Keys)
                {
                    if (s == Side.right || s == Side.top || cell.walls[s] == null) continue;
                    Wall wall = cell.walls[s];
                    XElement eWall = new XElement("Wall");
                    eWall.Add(new XAttribute("x", x));
                    eWall.Add(new XAttribute("y", y));
                    eWall.Add(new XAttribute("Side", s));
                    eWall.Add(new XAttribute("colorslot", wall.colorslot));
                    eWall.Add(new XAttribute("isTraversible", wall.IsTraversible));
                    eWalls.Add(eWall);
                }
            }
        }
        

        return eRoot;
    }

    public XElement SerializeObject(object o)
    {
        Type type = o.GetType();
        XElement r = new XElement(type.ToString());
        if (o is MonoBehaviour)
        {
            MonoBehaviour mono = (MonoBehaviour)o;
            if (mono.gameObject != null)
            {
                XAttribute att = new XAttribute("Name", mono.gameObject.name);
                r.Add(att);
                if (mono is GamePiece)
                {
                    GamePiece gp = (GamePiece)mono;
                    XAttribute att2 = new XAttribute("Zpos", gp.getZPosition());
                    r.Add(att2);
                }
            }
        }
        foreach(var prop in type.GetProperties())
        {
            if (prop.GetCustomAttributes(typeof(SerializeBlockIt), true).Length > 0)
            {
                XElement p = new XElement(prop.Name, prop.GetValue(o, null));
                XAttribute att = new XAttribute("PType", prop.PropertyType);
                p.Add(att);
                r.Add(p);
            }
        }
        foreach (var field in type.GetFields())
        {
            if (field.GetCustomAttributes(typeof(SerializeBlockIt), true).Length > 0)
            {
                XElement f = new XElement(field.Name, field.GetValue(o));
                XAttribute att = new XAttribute("FType", field.FieldType);
                f.Add(att);
                r.Add(f);
            }
        }
        return r;
    }


    void WriteFile(string filename, string text)
    {
        Debug.Log(Application.persistentDataPath);
        string fullFileName = Application.persistentDataPath + "/" + filename;
        StreamWriter fileWriter = File.CreateText(fullFileName);
        //fileWriter.WriteLine("Hello world");
        fileWriter.Write(text);
        fileWriter.Close();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
[System.AttributeUsage(System.AttributeTargets.Property |
                       System.AttributeTargets.Field)
]
public class SerializeBlockIt : Attribute { }
