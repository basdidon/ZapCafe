using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    public bool CanExecute { get; }
    public void Execute();
}
