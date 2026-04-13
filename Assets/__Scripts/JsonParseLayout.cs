using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores layout information
/// </summary>
[System.Serializable]
public class JsonLayout
{
    public Vector2                 multiplier;
    public List<JsonLayoutSlot>     slots;
    public JsonLayoutPile           drawPile, discardPile;
}

/// <summary>
/// implements unity's iserializationcallbackreceiver interface
/// </summary>
[System.Serializable]
public class JsonLayoutSlot: ISerializationCallbackReceiver
{
    public int      id;
    public float      x;
    public float      y;
    public bool         faceUp;
    public string       layer;
    public string       hiddenByString;

    [System.NonSerialized]
    public List<int>   hiddenBy;

/// <summary>
/// pulls data from hiddenbystring and places into hidden by list
/// </summary>
    public void OnAfterDeserialize()
    {
        hiddenBy = new List<int>();
        if(hiddenByString.Length == 0) return;

        string[] bits = hiddenByString.Split(',');
        for (int i=0; i<bits.Length; i++)
        {
            hiddenBy.Add(int.Parse(bits[i]));
        }
    }

    public void OnBeforeSerialize(){}
}

[System.Serializable]
public class JsonLayoutPile
{
    public float      x, y;
    public string       layer;
    public float        xStagger; //stagger fan cards to the side for the draw pile
}

public class JsonParseLayout: MonoBehaviour
{
    public static JsonParseLayout S {get; private set;}

    [Header("Inscribed")]
    public TextAsset jsonLayoutFile;

    [Header("Dynamic")]
    public JsonLayout layout;

    void Awake()
    {
        layout = JsonUtility.FromJson<JsonLayout>(jsonLayoutFile.text);
        S = this;
    }
}

