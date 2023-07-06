using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    TaskObject TaskObject { get; }
    public bool CanExecute { get; }
    public float Duration { get; }
    public void Execute();
}
