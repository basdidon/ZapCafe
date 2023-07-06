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

    public class GetDonut : ITask
    {
        public TaskObject TaskObject { get; }

        public GetDonut(DonutBox donutBox)
        {
            TaskObject = donutBox;
        }
        /*
        public bool TryExecute()
        {
            var donutBoxes = FindObjectsOfType<DonutBox>();
            for (int i = 0; i < donutBoxes.Length; i++)
            {

            }
        }
        */
        public bool CanExecute {
            get
            {
                return true;  

            }
        }
        //TaskObject.Worker == null;

        public float Duration => 1f;

        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
