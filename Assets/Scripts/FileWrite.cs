using UnityEngine;
using OrbItUtils;
using System.Xml.Linq;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public static class FileWrite
{
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S)) InitSerialization();
    //    if (Input.GetKeyDown(KeyCode.D)) InitDeserialization();
    //    if (Input.GetKeyDown(KeyCode.T)) RoomManager.roomManager.RefreshColorFamilyAll();
    //}
    static string filename = "Battletoads+Tetris.xml";
    public static string InitSerialization()
    {
        XElement all = SerializeGrid();
        Debug.Log(all);

        string fname = MonoBehaviour.FindObjectOfType<MetaData>().levelName;
        if (string.IsNullOrEmpty(fname))
        {
#if UNITY_EDITOR
            fname = Application.isPlaying ? Application.loadedLevelName : Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
#else
            fname = Application.loadedLevelName;
#endif
        }
        fname = WWW.EscapeURL(fname);
        Debug.Log("Writing file: " + fname);
        return WriteFile(fname + ".xml", all.ToString());
    }

    public static void InitDeserialization(string filename)
    {
        //MonoBehaviour.Destroy(RoomManager.roomManager);
        //RoomManager.roomManager = null;
        Application.LoadLevel("Blank");
        AwaitingDSCallback = true;
        FileWrite.filename = filename;
    }
    static bool AwaitingDSCallback = false;
    public static bool DeserializationCallback()
    {
        if (!AwaitingDSCallback)
        {
            return false;
        }
        AwaitingDSCallback = false;

        RoomManager room = MonoBehaviour.FindObjectOfType<RoomManager>();

        XElement loaded = XElement.Load(Application.dataPath + "/SavedLevels/" + filename);
        XElement meta = loaded.Element(XName.Get("Meta"));
        MetaData metaData = (MetaData)MonoBehaviour.FindObjectOfType(typeof(MetaData));
        metaData.author = meta.Attribute("Author").Value;
        metaData.levelName = meta.Attribute("LevelName").Value;
        metaData.welcomeHint = meta.Attribute("WelcomeHint").Value;
        metaData.difficulty = (MetaData.Difficulty)Enum.Parse(typeof(MetaData.Difficulty), meta.Attribute("Difficulty").Value);

        XElement grid = loaded.Element( "Grid");

        foreach (XElement row in grid.Elements( "Row"))
        {
            foreach (XElement eCell in row.Elements( "Cell"))
            {
                int cellx = int.Parse(eCell.Attribute("x").Value);
                int celly = int.Parse(eCell.Attribute("y").Value);
                Cell cell = RoomManager.roomManager.Grid[cellx][celly];
                foreach (XElement gamePiece in eCell.Elements())
                {
                    string typeName = gamePiece.Name.ToString();
                    string gameObjectName = gamePiece.Attribute("Name").Value;
                    Type pieceType = Type.GetType(typeName);
                    GamePiece gp = room.SpawnPiece(pieceType, cell);
                    gp.gameObject.name = gameObjectName;
                    foreach (XElement FPInfo in gamePiece.Elements())
                    {
                        string infotype = FPInfo.Name.ToString();
                        XAttribute attr = FPInfo.Attribute("Name");
                        FPInfo fpinfo = null;
                        if (infotype == "Field" || infotype == "Property")
                        {
                            fpinfo = pieceType.GetFPInfo(attr.Value);
                        }
                        string val = FPInfo.Attribute("Value").Value;
                        fpinfo.SetValue(val, gp);
                    }
                }
            }
        }
        XElement Walls = loaded.Element("Walls");

        foreach (XElement eWall in Walls.Elements())
        {
            int CellX = int.Parse(eWall.Attribute("x").Value);
            int CellY = int.Parse(eWall.Attribute("y").Value);
            Side s = (Side)Enum.Parse(typeof(Side), eWall.Attribute("Side").Value);
            ColorSlot clrSlt = (ColorSlot)Enum.Parse(typeof(ColorSlot),eWall.Attribute("colorslot").Value);
            WallType walltype = (WallType)Enum.Parse(typeof(WallType), eWall.Name.ToString());
            Cell targetCell = Cell.Get(CellX, CellY);
            Wall wall = RoomManager.roomManager.SpawnWall(targetCell, s, clrSlt);
            wall.IsTraversible = bool.Parse(eWall.Attribute("isTraversible").Value);
            wall.wallType = walltype;
        }
        
        return true;
    }
    public static XElement SerializeGrid()
    {
        XElement eRoot = new XElement("Root");

        XElement eInfo = new XElement("Meta");
        eRoot.Add(eInfo);
        MetaData auth = MonoBehaviour.FindObjectOfType<MetaData>();
        if (auth != null)
        {
            eInfo.Add(new XAttribute("Author", auth.author));
            eInfo.Add(new XAttribute("LevelName", auth.levelName));
            eInfo.Add(new XAttribute("WelcomeHint", auth.welcomeHint));
            eInfo.Add(new XAttribute("Difficulty", auth.difficulty));
        }

        XElement eGrid = new XElement("Grid");
        eRoot.Add(eGrid);

        var grid = RoomManager.roomManager.Grid;
        eGrid.Add(new XAttribute("Width", grid[0].Length));
        eGrid.Add(new XAttribute("Height", grid.Length));


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
                        piece.OnSerialize();
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
                    if (s == Side.right || s == Side.top || cell.walls[s] == null || cell.walls[s].gameObject.HasParent("OuterWalls")) continue;
                    Wall wall = cell.walls[s];
                    XElement eWall = new XElement(wall.wallType.ToString());
                    eWall.Add(new XAttribute("x", x));
                    eWall.Add(new XAttribute("y", y));
                    eWall.Add(new XAttribute("Side", s));
                    eWall.Add(new XAttribute("colorslot", wall.colorSlot));
                    eWall.Add(new XAttribute("isTraversible", wall.IsTraversible));
                    eWalls.Add(eWall);
                }
            }
        }
        

        return eRoot;
    }

    public static XElement SerializeObject(object o)
    {
        Type type = o.GetType();
        
        XElement r = new XElement(type.Name);
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
                XElement p = new XElement("Property");
                XAttribute att = new XAttribute("Name", prop.Name);
                XAttribute att2 = new XAttribute("Type", prop.PropertyType);
                XAttribute att3 = new XAttribute("Value", prop.GetValue(o, null));
                
                p.Add(att);
                p.Add(att2);
                p.Add(att3);
                r.Add(p);
            }
        }
        foreach (var field in type.GetFields())
        {
            if (field.GetCustomAttributes(typeof(SerializeBlockIt), true).Length > 0)
            {
                XElement f = new XElement("Field");
                XAttribute att = new XAttribute("Name", field.Name);
                XAttribute att2 = new XAttribute("Type", field.FieldType);
                XAttribute att3 = new XAttribute("Value", field.GetValue(o));

                f.Add(att);
                f.Add(att2);
                f.Add(att3);
                r.Add(f);
            }
        }
        return r;
    }


    static string WriteFile(string filename, string text)
    {
        //Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath;
#if UNITY_EDITOR
        path = Application.dataPath;
#endif
        //string fullFileName = path + "/SavedLevels/" + filename;
        string fullFileName = path + "/" + filename;
        StreamWriter fileWriter = File.CreateText(fullFileName);
        //fileWriter.WriteLine("Hello world");
        fileWriter.Write(text);
        fileWriter.Close();
        return fullFileName;
    }
}
[System.AttributeUsage(System.AttributeTargets.Property |
                       System.AttributeTargets.Field)
]
public class SerializeBlockIt : Attribute { }
