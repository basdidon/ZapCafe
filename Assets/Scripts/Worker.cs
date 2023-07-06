using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Worker : Charecter
{
    [field: SerializeField] public Tilemap PathTilemap { get; set; }
    TaskManager TaskManager { get => TaskManager.Instance; }
    [OdinSerialize] ITask task;
    [OdinSerialize] public ITask Task 
    {
        get => task;
        set
        {
            if (value != null)
            {
                task = value;
                var dirs = new List<Vector3Int>() { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
                if(PathFinder.TryFindWaypoint(this,CellPosition, task.TaskObject.WorkingCell,dirs,out List<Vector3Int> waypoints))
                {
                    WayPoints = waypoints;
                }
            }
        }
    }

    // task progress
    public GameObject TaskProgress;
    public Image ProgressImg;

    // state
    public ExecutingTask ExecutingTask;

    protected override void Awake()
    {
        base.Awake();
        IdleState = new WorkerIdle(this);
        MoveState = new WorkerMove(this);
        ExecutingTask = new ExecutingTask(this);
    }

    protected override void Start()
    {
        base.Start();
        TaskProgress.SetActive(false);
    }

    public override bool CanMoveTo(Vector3Int cellPos)
    {
        return PathTilemap.HasTile(cellPos);
    }

    [Button]
    public void RequestTask()
    {
        ITask task = TaskManager.Tasks.Find(task => task.CanExecute);

        if (task != null)
        {
            Task = task;
        }
        else
        {
            TaskManager.AvailableWorker.Enqueue(this);
        }
    }
}

public class WorkerIdle : IdleState<Worker>
{
    public WorkerIdle(Worker worker) : base(worker) {}

    public override void EnterState()
    {
        TaskManager.Instance.AddAvaliableWorker(Charecter);
    }

    public override void ExitState(){}
}

public class WorkerMove : MoveState<Worker>
{
    public WorkerMove(Worker worker):base(worker){}

    public override void ExitConditionCheck()
    {
        if(Charecter.WayPoints.Count == 0)
        {
            Debug.Log("End");

            if (Charecter.Task != null)
            {
                Charecter.CurrentState = Charecter.ExecutingTask;
            }
        }
        else
        {
            // self transition
            Charecter.CurrentState = Charecter.MoveState;
        }
    }
}

public class ExecutingTask : ISelfExitState
{
    Worker Worker { get; }
    float timeElapsed;
    float duration;
    public ExecutingTask(Worker worker)
    {
        Worker = worker;
    }
    public void EnterState()
    {
        Worker.TaskProgress.SetActive(true);
        Worker.ProgressImg.fillAmount = 0f;
        duration = Worker.Task.Duration;
        timeElapsed = 0f;
        Worker.StartCoroutine(StartTask());
    }

    IEnumerator StartTask()
    {
        while (timeElapsed < Worker.Task.Duration)
        {
            Worker.ProgressImg.fillAmount = timeElapsed / duration;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Worker.Task.Execute();

        ExitConditionCheck();
    }

    public void ExitState(){
        Worker.Task = null;
        Worker.TaskProgress.SetActive(false);
    }

    public void ExitConditionCheck()
    {
        Worker.CurrentState = Worker.IdleState;
    }
}
