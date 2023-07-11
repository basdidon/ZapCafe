using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class DonutBox : BoardObject,IWorkStation<Donut>
{
    // Worker
    public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkingPoint { get; set; }
    public Vector3Int WorkingCell { get => BoardManager.GetCellPos(WorkingPoint.position); }

    [SerializeField] Sprite sprite;
    public Sprite Sprite => sprite;

    private void Start()
    {
        WorkStationRegistry.Instance.AddWorkStation(this);
    }

    public Donut GetItem()
    {
        throw new System.NotImplementedException();
    }
}

public class GetItem<T> : Task<T> where T : Item
{
    public override float Duration => 3f;

    public GetItem(Customer customer) : base(customer)
    {
        Performed += delegate {
            Worker.ItemSpriteRenderer.sprite = WorkStation.Sprite;
            var serveTask = new Bar.ServeOrderTask(customer);
            serveTask.Performed += delegate {
                Worker.ItemSpriteRenderer.sprite = null;
                Worker.Tasks.Remove(serveTask);
            };
            Worker.Tasks.Add(serveTask);

            TaskManager.Instance.WorkStationFree<T>();
        };
    }
}

















    /*
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
    /*
    public override float Duration => throw new System.NotImplementedException();

    public override bool TryGetworkStation(Worker worker, out WorkStation workStation)
    {
        throw new System.NotImplementedException();
    }*/
