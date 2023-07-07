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
}

public class WorkerIdle : IdleState<Worker>
{
    public WorkerIdle(Worker worker) : base(worker) {}

    public override void EnterState()
    {
        if (TaskManager.Instance.TryGetTask(Charecter,out Task task))
        {
            Debug.Log("found");
            task.TryAssignTask(Charecter);
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
    public WorkerMove(Worker worker):base(worker){}

    public override void SetNextState()
    {
        if(Charecter.WayPoints.Count == 0)
        {
            if (TaskManager.Instance.Tasks.Find(task => task.Worker == Charecter) != null)
            {
                Charecter.CurrentState = Charecter.ExecutingTask;
            }
            else
            {
                Charecter.CurrentState = Charecter.IdleState;
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
    Task Task { get; set; }
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
        Task = TaskManager.Instance.Tasks.Find(task => task.Worker == Worker);
        Task.TaskObject.Worker = Worker;
        duration = Task.Duration;
        timeElapsed = 0f;
        Worker.StartCoroutine(StartTask());
    }

    IEnumerator StartTask()
    {
        while (timeElapsed < Task.Duration)
        {
            Worker.ProgressImg.fillAmount = timeElapsed / duration;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log(Task.ToString());
        Task.Execute();

        SetNextState();
    }

    public void ExitState(){
        Task.TaskObject.Worker = null;
        TaskManager.Instance.Tasks.Remove(Task);
        Worker.TaskProgress.SetActive(false);
    }

    public void SetNextState()
    {
        Worker.CurrentState = Worker.IdleState;
    }
}
