using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetOrderTask : BaseTask
{
    public Bar Bar { get; set; }
    public override float Duration => 1f;

    public GetOrderTask(Bar bar) : base()
    {
        Bar = bar;
        Started += delegate
        {
            Worker.Animator.SetBool("IsTalking", true);
        };
        Performed += delegate
        {
            Worker.Animator.SetBool("IsTalking", false);
        };
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        workStation = Bar;
        return true;
    }

    public override IEnumerable<WorkerWorkStationPair> GetTaskCondition(IEnumerable<WorkerWorkStationPair> pairs)
    {
        return pairs.Where(pair => pair.Worker.HoldingItem == null);
    }

    public override bool TryCheckCondition(ref IEnumerable<WorkerWorkStationPair> pairs)
    {
        pairs = pairs.Where(pair => pair.Worker.HoldingItem == null);
        if (pairs.Count() > 0)
            return true;
        return false;
    }
}
