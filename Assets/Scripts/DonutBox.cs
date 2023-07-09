using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class DonutBox : TaskObject
{
    TaskManager TaskManager { get => TaskManager.Instance; }

    // Worker
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public override Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    // Donut Sprite
    [OdinSerialize] public Sprite DonutSprite { get; set; }

    private void Start()
    {
        TaskManager.AddTaskObject(this);
    }
}

public class GetDonut : Task
{
    public GetDonut(Customer customer) : base(customer)
    {
        performed += delegate { 
            Worker.ItemSpriteRenderer.sprite = (TaskObject as DonutBox).DonutSprite;
            //Worker.Tasks.Add(new Bar.ServeOrderTask(customer));
        };
    }

    public override bool TryGetTaskObject(Charecter charecter, out TaskObject taskObject)
    {
        taskObject = null;
        if (TaskManager.Instance.TryGetTaskObject(charecter, out DonutBox donutBox))
        {
            taskObject = donutBox;
            return true;
        }

        return false;
    }

    public override float Duration => 1f;


    /*
    public override Task Execute()
    {
        // give donut to player
        return new Bar.ServeOrderTask(Customer);
    }*/
}