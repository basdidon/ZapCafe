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

    public Sprite GetSprite(Direction direction)
    {
        return direction switch
        {
            Direction.Left => LeftDownSprite,
            Direction.Up => LeftUpSprite,
            Direction.Right => RightUpSprite,
            Direction.Down => RightDownSprite,
            _ => null,
        };
    }
}  
