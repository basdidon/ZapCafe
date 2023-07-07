using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutBox : TaskObject
{
    TaskManager TaskManager { get => TaskManager.Instance; }

    // Worker
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public override Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    private void Start()
    {
        TaskManager.AddTaskObject(this);
    }

    public class GetDonut : Task
    {
        public GetDonut(Customer customer) : base(customer) { }

        public override bool TryGetTaskObject(Charecter charecter)
        {
            if(TaskManager.Instance.TryGetTaskObject(charecter,out DonutBox donutBox))
            {
                TaskObject = donutBox;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override float Duration => 1f;

        public override void Execute()
        {
            TaskManager.Instance.AddTask(new Bar.ServeOrderTask(Customer));
        }
    }
}
