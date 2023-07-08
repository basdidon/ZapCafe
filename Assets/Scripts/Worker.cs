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
    // Hold Item
    public SpriteRenderer ItemSpriteRenderer;

    // task progress
    public GameObject TaskProgress;
    public Image ProgressImg;

    // Task
    [OdinSerialize] public Task CurrentTask { get; set; }
    public List<Task> Tasks { get; set; }
    
    public bool TrySetTask(Task newTask)
    {
        if (newTask == null)
            return false;

        if (newTask.TryGetTaskObject(this, out TaskObject taskObject))
        {
            // move worker to taskObject
            if (PathFinder.TryFindWaypoint(this, CellPosition, taskObject.WorkingCell, dirs, out List<Vector3Int> waypoints))
            {
                CurrentTask = newTask;
                //CurrentTask.Worker = this;
                taskObject.Worker = this;
                CurrentState = new WorkerMove(this, waypoints, new ExecutingTask(this, taskObject));
                return true;
            }
            else
            {
                Debug.LogError("<color=red> Can't Move To TaskObject</color>");
            }
        }

        return false;
    }

    // Events
    public System.Action OnTaskDone;

    // State
    protected void Awake()
    {
        IdleState = new WorkerIdle(this);
        Tasks = new();
    }

    protected override void Start()
    {
        base.Start();
        TaskProgress.SetActive(false);
    }

    public override bool CanMoveTo(Vector3Int cellPos) => PathTilemap.HasTile(cellPos);
}

public class WorkerIdle : IdleState<Worker>
{
    public WorkerIdle(Worker worker) : base(worker) {}

    public override void EnterState()
    {
        
        var task = Charecter.Tasks.Find(task => Charecter.TrySetTask(task));
        
        if(task != null)
        {
            Charecter.Tasks.Remove(task);
        }
        else
        {
            TaskManager.Instance.AddAvaliableWorker(Charecter);
        }
    }

    public override void ExitState(){}
}

public class WorkerMove : MoveState<Worker>
{
    public IState NextState { get; set; }
    public WorkerMove(Worker worker, List<Vector3Int> waypoints, IState nextState = null) : base(worker, waypoints)
    {
        NextState = nextState;
    }

    public override void SetNextState()
    {
        if(WayPoints.Count == 0)
        {
            Charecter.CurrentState = NextState;
        }
        else
        {
            // self transition
            Charecter.CurrentState = new WorkerMove(Charecter,WayPoints,NextState);
        }
    }
}

public class ExecutingTask : ISelfExitState
{
    Worker Worker { get; }
    Task NextTask { get; set; }
    TaskObject TaskObject { get; }
    float timeElapsed;
    float duration;

    public ExecutingTask(Worker worker,TaskObject taskObject)
    {
        Worker = worker;
        TaskObject = taskObject;
    }

    public void EnterState()
    {
        Worker.TaskProgress.SetActive(true);
        Worker.ProgressImg.fillAmount = 0f;
        duration = Worker.CurrentTask.Duration;
        timeElapsed = 0f;
        Worker.StartCoroutine(StartTask());
    }

    IEnumerator StartTask()
    {
        while (timeElapsed < Worker.CurrentTask.Duration)
        {
            Worker.ProgressImg.fillAmount = timeElapsed / duration;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log(Worker.CurrentTask.ToString());
        NextTask = Worker.CurrentTask.Execute();

        SetNextState();
    }

    public void ExitState(){
        TaskObject.Worker = null;
        Worker.TaskProgress.SetActive(false);
    }

    public void SetNextState()
    {
        if (!Worker.TrySetTask(NextTask))
            Worker.CurrentState = Worker.IdleState;
        
    }
}
