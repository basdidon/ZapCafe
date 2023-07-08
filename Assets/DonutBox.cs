using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutBox : TaskObject
{
    TaskManager TaskManager { get => TaskManager.Instance; }

    // Worker
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public override Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    // Item Sprite
    [SerializeField] Sprite donutSprite;

    private void Start()
    {
        TaskManager.AddTaskObject(this);
    }

    public void GetItem(Worker worker)
    {
        worker.ItemSpriteRenderer.sprite = donutSprite;
    }

    public class GetDonut : Task
    {
        public GetDonut(Customer customer) : base(customer) {}

        public override bool TryGetTaskObject(Charecter charecter,out TaskObject taskObject)
        {
            taskObject = null;
            if(TaskManager.Instance.TryGetTaskObject(charecter,out DonutBox donutBox))
            {
                taskObject = donutBox;
                return true;
            }

            return false;
        }

        public override float Duration => 1f;

        public override Task Execute()
        {
            return new Bar.ServeOrderTask(Customer);
        }
    }
}
