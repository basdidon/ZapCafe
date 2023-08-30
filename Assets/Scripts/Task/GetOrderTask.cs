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

        TaskManager.Instance.AddTask(this);
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        workStation = Bar;
        return true;
    }

    public override bool TryCheckCondition(Worker worker, IWorkStation workStation) => worker.HoldingItem == null;
}
