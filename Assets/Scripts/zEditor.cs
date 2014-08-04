using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class zEditor
{
    public zSidebar sidebar;
    public zGridLayout pieceLayout, colorLayout;
    private Stack<UndoAction> undoActions = new Stack<UndoAction>();
    public float sidebarScreenRation { get { return sidebarScreenRation; } }
    public zEditor(float sidebarScreenRatio, float buttonWidth)
    {
        sidebar = new zSidebar(this, sidebarScreenRatio, buttonWidth);
        SetupPieceLayout(sidebarScreenRatio, buttonWidth);
        SetupColorLayout(sidebarScreenRatio, buttonWidth);
        sidebar.undoAction.OnClick += (zb) => { OnUndo(); };
    }
    public void Draw()
    {
        pieceLayout.Draw();
        colorLayout.Draw();
        sidebar.Draw();
    }
    void SetupPieceLayout(float ratio, float buttonWidth)
    {
        List<zButton> zeButtons = new List<zButton>();
        foreach (Type t in RoomManager.PieceTypeList)
        {
            zeButtons.Add(new zButton(t, buttonWidth, buttonWidth));
        }
        float layoutwidth = Screen.width * .5f;
        float layoutheight = Screen.height * .6f;
        float layoutx = Screen.width * (1f - ratio) - layoutwidth;
        pieceLayout = new zGridLayout(new Rect(layoutx, 0, layoutwidth, layoutheight), zeButtons, true, new Vector2(Screen.width, 0), 1f);
        //
        Action<zButton> hideLayout = (zb) => { pieceLayout.Hide(); sidebar.piecePicker.type = zb.type; };
        foreach (var zb in pieceLayout.contents) { zb.OnClick += hideLayout; }
    }
    void SetupColorLayout(float ratio, float buttonWidth)
    {
        List<zButton> colorButtons = new List<zButton>();
        foreach (ColorSlot color in Enum.GetValues(typeof(ColorSlot)))
        {
            zButton zb = new zButton(typeof(Tile), buttonWidth, buttonWidth);
            zb.color = MetaData.colors[color];
            colorButtons.Add(zb);

        }
        float layoutwidth = Screen.width * .5f;
        float layoutheight = Screen.height * .4f;
        float layoutx = Screen.width * (1f - ratio) - layoutwidth;
        float layouty = colorButtons[0].Height + 12;
        colorLayout = new zGridLayout(new Rect(layoutx, layouty, layoutwidth, layoutheight), colorButtons, true, new Vector2(Screen.width, layouty), 1f);

        Action<zButton> hideLayout = (zb) => { colorLayout.Hide(); sidebar.colorPicker.color = zb.color; };
        foreach (var zb in colorLayout.contents) { zb.OnClick += hideLayout; }
        sidebar.colorPicker.color = colorButtons[0].color;
    }

    public void UpdateEditor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        Vector2 mouse = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        if (!mouse.isWithinGrid()) return;
        if (sidebar.activeButton == sidebar.piecePicker)
        {
            OnAdd(mouse);
        }
        else if (sidebar.activeButton == sidebar.eraserTool)
        {
            OnErase(mouse);
        }
    }
    Queue<Vector2> prevMousePositions = new Queue<Vector2>();
    void OnAdd(Vector2 mouse)
    {
        if (Input.GetMouseButton(0))
        {
            if (pieceLayout.IsVisible || colorLayout.IsVisible) return;
            Type type = sidebar.piecePicker.type;
            Cell target = Cell.GetFromWorldPos(mouse);
            ColorSlot colorslot = MetaData.GetColorFromSlot(sidebar.colorPicker.color);
            if (type == typeof(Wall))
            {
                TryWallSpawn(mouse, target, colorslot);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (type == typeof(Player))
                {
                    RoomManager.roomManager.SpawnPlayer(target);
                    AddUndoAction(RoomManager.roomManager.player, UndoAction.ActionType.Add);
                }
                else
                {
                    GamePiece gamePiece = RoomManager.roomManager.SpawnPiece(type, target, colorslot);
                    AddUndoAction(gamePiece, UndoAction.ActionType.Add);
                }
            }
        }
        else
        {
            if (prevMousePositions.Count != 0) prevMousePositions = new Queue<Vector2>();
        }
    }

    void TryWallSpawn(Vector2 mouse, Cell target, ColorSlot colorslot)
    {
        prevMousePositions.Enqueue(mouse);
        Side side; Orientation or;
        Vector2 worldpos = Utils.WorldToWallPos(mouse, out side, out or);
        if (prevMousePositions.Count == 1)
        {
            Wall wall = RoomManager.roomManager.SpawnWall(target, side, colorslot);
            AddUndoAction(wall, UndoAction.ActionType.Add);
        }
        else if (prevMousePositions.Count >= 15)
        {
            Vector2 oldPos = prevMousePositions.Dequeue();
            Vector2 dir = mouse - oldPos;
            float angle = Mathf.Atan2(dir.y, dir.x);
            angle = Mathf.Abs(angle);
            //Debug.Log(angle);

            float dead = Mathf.PI / 12;
            Wall existing = target.getWall(side);
            if (existing == null)
            {
                if (side == Side.left || side == Side.right)
                {
                    if (Mathf.Abs(angle - Mathf.PI / 2f) < dead)
                    {
                        Wall wall = RoomManager.roomManager.SpawnWall(target, side, colorslot);
                        AddUndoAction(wall, UndoAction.ActionType.Add);
                        Debug.Log(side);
                    }
                }
                else
                {
                    if (angle > Mathf.PI - dead || angle < dead)
                    {
                        Wall wall = RoomManager.roomManager.SpawnWall(target, side, colorslot);
                        AddUndoAction(wall, UndoAction.ActionType.Add);
                        Debug.Log(side);
                    }
                }
            }
            
        }
    }

    void OnErase(Vector2 mouse)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (pieceLayout.IsVisible || colorLayout.IsVisible) return;
            Type type = sidebar.piecePicker.type;
            if (type == typeof(Wall))
            {
                Wall wall = RoomManager.roomManager.RemoveWall(mouse);
                AddUndoAction(wall, UndoAction.ActionType.Remove);
            }
            else
            {
                Cell target = Cell.GetFromWorldPos(mouse);
                GamePiece gamePiece = RoomManager.roomManager.RemoveTopPiece(target);
                AddUndoAction(gamePiece, UndoAction.ActionType.Remove);
            }
        }
    }
    void OnUndo()
    {
        if (undoActions.Count != 0)
        {
            UndoAction undoAction = undoActions.Pop();
            undoAction.PerformUndoAction();
        }
    }
    void AddUndoAction(Wall wall, UndoAction.ActionType actionType)
    {
        if (wall == null) return;
        UndoAction action = new UndoAction(wall, actionType);
        undoActions.Push(action);
    }
    void AddUndoAction(GamePiece piece, UndoAction.ActionType actionType)
    {
        if (piece == null) return;
        UndoAction action = new UndoAction(piece, actionType);
        undoActions.Push(action);
    }
}

public class UndoAction
{
    public enum ActionType
    {
        Add,
        Remove,
    }
    Cell cell;
    Wall wall;
    GamePiece piece;
    ActionType actionType;
    Vector2 position;
    public UndoAction(Wall wall, ActionType actionType)
    {
        this.wall = wall;
        this.actionType = actionType;
        this.position = wall.transform.position;
        this.cell = Cell.GetFromWorldPos(position);
    }
    public UndoAction(GamePiece piece, ActionType actionType)
    {
        this.piece = piece;
        this.actionType = actionType;
        this.position = piece.transform.position;
        this.cell = piece.cell;
    }
    public void PerformUndoAction()
    {
        if (actionType == ActionType.Add)
        {
            if (wall != null)
            {
                RoomManager.roomManager.RemoveWall(position);
            }
            else if (piece != null)
            {
                RoomManager.roomManager.RemoveTopPiece(cell);
            }
        }
        else if (actionType == ActionType.Remove)
        {
            if (wall != null)
            {
                RoomManager.roomManager.AddWall(wall);
            }
            else if (piece != null)
            {
                if (piece is Player)
                {
                    RoomManager.roomManager.SpawnPlayer(cell);
                }
                else
                {
                    RoomManager.roomManager.AddPiece(piece.gameObject, piece.GetType(), piece.colorslot);
                }
            }
        }
    }
}

public class zSidebar
{
    public zEditor editor;
    List<zButton> contents;
    Rect properRect;
    Vector2 origin;
    float screenWidthPercent, width;

    public zButton piecePicker;
    public zButton colorPicker;
    public zButton eraserTool;
    public zButton selectTool;
    public zButton undoAction;
    public zButton menuButton;

    public zButton activeButton;

    public zSidebar(zEditor editor, float screenWidthPercent, float buttonWidth)
    {
        this.editor = editor;
        this.screenWidthPercent = screenWidthPercent;
        this.origin = new Vector2(Screen.width * (1f - screenWidthPercent), 0);
        this.width = Screen.width * screenWidthPercent;
        this.properRect = new Rect(origin.x, origin.y, width, Screen.height);
        this.contents = new List<zButton>();

        piecePicker = new zButton(typeof(Wall), buttonWidth, buttonWidth); contents.Add(piecePicker);
        colorPicker = new zButton(typeof(Tile), buttonWidth, buttonWidth); contents.Add(colorPicker);
        eraserTool = new zButton("Erase", buttonWidth, buttonWidth); contents.Add(eraserTool);
        selectTool = new zButton("Select", buttonWidth, buttonWidth); contents.Add(selectTool);
        undoAction = new zButton("Undo", buttonWidth, buttonWidth); contents.Add(undoAction);
        menuButton = new zButton("Menu", buttonWidth, buttonWidth); contents.Add(menuButton);

        piecePicker.OnClick += delegate(zButton zb)
        {
            if (zb.activated && !editor.pieceLayout.IsVisible)
            {
                editor.pieceLayout.Show();
            }
            else if (editor.pieceLayout.IsVisible)
            {
                editor.pieceLayout.Hide();
            }
        };
        piecePicker.LongPress += (zb) =>
        {
            if (editor.pieceLayout.IsVisible)
            {
                editor.pieceLayout.Hide();
            }
            else
            {
                if (editor.colorLayout.IsVisible)
                {
                    editor.colorLayout.Hide();
                }
                editor.pieceLayout.Show();
            }
            SwitchActiveButton(zb);
        };

        Action<zButton> toggleColors = (zb) =>
        {
            if (editor.colorLayout.IsVisible)
            {
                editor.colorLayout.Hide();
            }
            else
            {
                editor.colorLayout.Show();
            }
        };
        colorPicker.OnClick += toggleColors;
        colorPicker.LongPress += toggleColors;
        menuButton.OnClick += Serialize;

        float vertPadding = (Screen.height - (piecePicker.Height * contents.Count)) / (contents.Count + 1);
        float heightCounter = vertPadding;
        foreach(zButton btn in contents)
        {
            btn.x = Screen.width - (width / 2) - (btn.Width / 2);
            btn.y = heightCounter;
            heightCounter += btn.Height + vertPadding;
            btn.OnClick += (zb) => 
            { 
                SwitchActiveButton(zb); 
                if (zb != piecePicker && editor.pieceLayout.IsVisible) 
                    editor.pieceLayout.Hide();
                if (zb != colorPicker && editor.colorLayout.IsVisible)
                    editor.colorLayout.Hide(); 
            };
        }
    }

    public void SwitchActiveButton(zButton button)
    {
        if (activeButton == button || button == colorPicker || button == undoAction || button == menuButton) return;
        if (activeButton != null)
        {
            activeButton.activated = false;
        }
        activeButton = button;
        activeButton.activated = true;
    }
    TouchScreenKeyboard keyboard; 
    public void ShowKeyboard(zButton button)
    {
        //keyboard = TouchScreenKeyboard.Open("Enter name");

    }
    public void Serialize(zButton button)
    {
        string filepath = "";
        try
        {
            filepath = FileWrite.InitSerialization();

        }
        catch(Exception e)
        {
            filepath = e.Message;
        }
        keyboard = TouchScreenKeyboard.Open(filepath);
    }
    public void Draw()
    {
        GUI.Box(properRect, "");
        foreach(var btn in contents)
        {
            btn.Draw();
        }
    }
}