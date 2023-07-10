using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class DonutBox : WorkStation
{
    TaskManager TaskManager { get => TaskManager.Instance; }

    // Worker
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public override Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    // Donut Sprite
    [OdinSerialize] public Sprite DonutSprite { get; set; }

    private void Start()
    {
        TaskManager.AddworkStation(this);
    }
}

public class GetDonut : Task<DonutBox>
{
    public override float Duration => 3f;

    public GetDonut(Customer customer) : base(customer)
    {
        performed += delegate {
            Worker.ItemSpriteRenderer.sprite = (WorkStation as DonutBox).DonutSprite;
            var serveTask = new Bar.ServeOrderTask(customer);
            serveTask.performed += delegate {
                Worker.ItemSpriteRenderer.sprite = null;
                Worker.Tasks.Remove(serveTask);
            };
            Worker.Tasks.Add(serveTask);

            TaskManager.Instance.WorkStationFree<DonutBox>();
        };
    }

    public override bool TryGetworkStation(Worker worker, out WorkStation workStation)
    {
        workStation = null;
        if (TaskManager.Instance.TryGetworkStation(worker, out DonutBox donutBox))
        {
            workStation = donutBox;
            return true;
        }

        return false;
    }

}