using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public Sprite play, pause, stop, restart;
    public Sprite radialDeselected, radialSelected;
    public Sprite showPass, hidePass;
    public List<SpriteSwap> spriteSwaps = new List<SpriteSwap>();

    protected internal Sprite SpriteSwap(string spriteName)
    {
        int spriteCount = spriteSwaps.Count;
        if (spriteCount > 0)
        {
            var spriteSwap = spriteSwaps.GetEnumerator();
            while (spriteSwap.MoveNext())
            {
                if (spriteSwap.Current.name.ToLower().Equals(spriteName.ToLower()))
                {
                    Sprite sprite = spriteSwap.Current.sprite;
                    return sprite;
                }
            }
        }
        Debug.Log($"Check sprite name!");
        return null;
    }
}