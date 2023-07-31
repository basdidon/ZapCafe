using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOrderTask : BaseTask
{
    public Bar Bar { get; set; }
    public override float Duration => 5f;

    public GetOrderTask(Customer customer, Bar bar) : base()
    {
        Bar = bar;
    }

    public override bool TryGetWorkStation(Worker worker, out IWorkStation workStation)
    {
        workStation = Bar;
        return true;
    }
}
