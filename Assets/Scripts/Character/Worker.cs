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
    [SerializeReference] ITask currentTask;
    public ITask CurrentTask
    {
        get => currentTask;
        set
        {
            currentTask = value;
            if(CurrentTask != null)
            {
                Debug.Log(currentTask.Waypoints == null);
                CurrentState = new WorkerMove(this, currentTask.Waypoints, new ExecutingTask(this));
            }
        }
    }

    public bool TryGetWaypoint(Vector3Int targetCellPos,out List<Vector3Int> waypoints)
    {
        return PathFinder.TryFindWaypoint(this, CellPosition, targetCellPos, dirs, out waypoints);
    }
    /*
    // Can Worker go to workStation
    public bool TrySetWorkStaion(IWorkStation workStation)
    {
        // move worker to workStation
        if (PathFinder.TryFindWaypoint(this, CellPosition, workStation.WorkingCell, dirs, out List<Vector3Int> waypoints))
        {
            
            return true;
        }

        Debug.LogError("<color=red> Can't Move To workStation</color>");
        return false;
    }*/

    // State
    protected void Awake()
    {
        IdleState = new WorkerIdle(this);
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
        Worker.CurrentTask = null;
    }

    public void SetNextState()
    {
            Worker.CurrentState = Worker.IdleState;   
    }
}
