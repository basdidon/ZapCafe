using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Directions
{
    LeftDown,
    LeftUp,
    RightUp,
    RightDown,
}

[Serializable]
public class SpriteDirection
{
    [field: SerializeField] public Sprite LeftDownSprite { get; set; }
    [field: SerializeField] public Sprite LeftUpSprite { get; set; }
    [field: SerializeField] public Sprite RightUpSprite { get; set; }
    [field: SerializeField] public Sprite RightDownSprite { get; set; }

    //public Action<Sprite> OnDirectionChanged;

    public Sprite GetSprite(Directions direction)
    {
        return direction switch
        {
            Directions.LeftDown => LeftDownSprite,
            Directions.LeftUp => LeftUpSprite,
            Directions.RightUp => RightUpSprite,
            Directions.RightDown => RightDownSprite,
            _ => null,
        };
    }
}  
