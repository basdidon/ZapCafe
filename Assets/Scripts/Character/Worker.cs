using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Worker : Charecter
{
    [field: SerializeField] public Tilemap PathTilemap { get; set; }

    // task progress
    public GameObject TaskProgress;
    public Image ProgressImg;

    // Task
    [field:SerializeField] public ITask CurrentTask { get; set; }
    public List<ITask> Tasks { get; set; }

    /// <summary>
    /// set <c>Task</c> to <c>Worker</c>
    /// </summary>
    /// <param name="newTask"></param>
    /// <returns></returns>
    public bool TrySetTask(ITask newTask)
    {
        if (newTask == null)
            return false;
        if (newTask.Worker != null && newTask.Worker != this)
            return false;

        var workStation = newTask.GetworkStation(this);

        if (workStation != null)
        {
            // move worker to workStation
            if (PathFinder.TryFindWaypoint(this, CellPosition, workStation.WorkingCell, dirs, out List<Vector3Int> waypoints))
            {
                CurrentTask = newTask;
                CurrentTask.Worker = this;
                CurrentTask.WorkStation = workStation;
                CurrentState = new WorkerMove(this, waypoints, new ExecutingTask(this, workStation));
                return true;
            }
            else
            {
                Debug.LogError("<color=red> Can't Move To workStation</color>");
            }
        }

        return false;
    }

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

    public override bool CanMoveTo(Vector3Int cellPos){
        RaycastHit2D[] hits = new RaycastHit2D[100];
        int hits_n = Physics2D.RaycastNonAlloc(BoardManager.GetCellCenterWorld(cellPos), Vector2.down, hits,0.1f);
        bool isCollided = false;
        for (int i = 0; i < hits_n; i++){
            if (hits[i].transform.CompareTag("Collider"))
            {
                isCollided = true;
                break;
            }
            //Debug.Log($"{hits[i].transform.tag} : {hits[i].transform.name} contacctPoint at {hits[i].point} & at {cellPos}");
        }

        // Debug.DrawRay(BoardManager.GetCellCenterWorld(cellPos), Vector2.down * 0.1f, Color.black, 5f);

        return PathTilemap.HasTile(cellPos) && !isCollided;
    }
}

public class WorkerIdle : IdleState<Worker>
{
    public WorkerIdle(Worker worker) : base(worker) {}

    public override void EnterState()
    {
        //var task = Charecter.Tasks.Find(task => Charecter.TrySetTask(task));
        /*
        var task = TaskManager.Instance.Tasks.Find(task => task.Worker = Charecter);
        if(task == null)
        {
        }*/
        Debug.Log("Player Idle");
        TaskManager.Instance.AddAvaliableWorker(Charecter);
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
    IWorkStation WorkStation { get; }
    float timeElapsed;
    float duration;

    public ExecutingTask(Worker worker,IWorkStation workStation)
    {
        Worker = worker;
        WorkStation = workStation;
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
        Worker.CurrentTask.Started?.Invoke();
        while (timeElapsed < duration)
        {
            Worker.ProgressImg.fillAmount = timeElapsed / duration;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log(Worker.CurrentTask.ToString());
        Worker.CurrentTask.Performed?.Invoke();

        SetNextState();
    }

    public void ExitState(){
        Worker.TaskProgress.SetActive(false);
    }

    public void SetNextState()
    {
            Worker.CurrentState = Worker.IdleState;   
    }
}
