using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMeat : Task
{
    public override float Duration => throw new System.NotImplementedException();

    //public GetMeat(Order order):base(order){}

    public override IWorkStation GetworkStation(Worker worker)
    {
        throw new System.NotImplementedException();
    }
}
