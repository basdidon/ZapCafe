using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BasDidon.Direction;

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
            Directions.Left => LeftDownSprite,
            Directions.Up => LeftUpSprite,
            Directions.Right => RightUpSprite,
            Directions.Down => RightDownSprite,
            _ => null,
        };
    }
}  
