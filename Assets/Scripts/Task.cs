using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public abstract class Task
{
    public Customer Customer { get;}
    [OdinSerialize] public Worker Worker { get;private set; }
    [OdinSerialize] public TaskObject TaskObject { get;protected set; }

    public Task(Customer customer)
    {
        Customer = customer;
    }

    public abstract float Duration { get; }

    public bool TryAssignTask(Worker worker)
    {
        if (TryGetTaskObject(worker))
        {
            Worker = worker;
            
            // move worker to taskObject
            var dirs = new List<Vector3Int>() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
            if (PathFinder.TryFindWaypoint(worker, worker.CellPosition, TaskObject.WorkingCell, dirs, out List<Vector3Int> waypoints))
            {
                worker.WayPoints = waypoints;
            }
            return true;
        }

        return false;
    }
    public abstract bool TryGetTaskObject(Charecter charecter);
    
    public abstract void Execute();
}