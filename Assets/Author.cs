using UnityEngine;
using System.Collections;

public class Author : MonoBehaviour {

    public enum difficulty_setting { Easy, Normal, Hard, Nightmare, Battletoads }

    public string author;
    public string levelName;
    public difficulty_setting difficulty;

}
