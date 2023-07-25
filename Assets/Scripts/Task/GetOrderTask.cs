using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOrderTask : Task
{
    public Bar Bar { get; set; }
    public override float Duration => 5f;

    public GetOrderTask(Customer customer, Bar bar) : base(customer)
    {
        Bar = bar;
    }

    public override IWorkStation GetworkStation(Worker worker) => (IWorkStation)Bar;
}
