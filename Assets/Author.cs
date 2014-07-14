using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class Author : MonoBehaviour
{
    private static Author _instance;
    private static Author instance { get { if (_instance == null) _instance = (Author)Object.FindObjectOfType(typeof(Author)); return _instance; } }


    public enum difficulty_setting { Easy, Normal, Hard, Nightmare, Battletoads }

    public string author;
    public string levelName;
    public difficulty_setting difficulty;
    public bool usesLock;
    public bool usesBlock;
    public bool usesSwitch;
    public bool usesButton;
    public bool usesTrap;
    public bool usesTeleporter;

    public Color[] colors = { Color.white, Color.blue, Color.green, Color.red, Color.yellow, Color.magenta };
    
    public static Color GetColorSlot(ColorSlot colorslot)
    {
        int slotnum = (int)colorslot;
        if (instance != null && instance.colors != null && slotnum < instance.colors.Length)
        {
            return instance.colors[(int)colorslot];
        }
        return Color.grey;
    }


    void Awake()
    {
        _instance = this; //lol privaton
    }
   
}
