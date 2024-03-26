using UnityEngine;

[CreateAssetMenu(fileName = "SpriteSheetConfig", menuName = "Configuration/SpriteSheetConfig")]
public class SpriteSheetConfig : ScriptableObject
{
    [System.Serializable]
    public struct SpriteSheetEntry
    {
        public string key;
        public Sprite[] sprites;
    }

    public SpriteSheetEntry[] entries;
}
