using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomerState
{
    public class IdleState : IdleState<Customer>
    {
        public IdleState(Customer customer) : base(customer) { }
        public override void EnterState() { }
        public override void ExitState() { }
        public override void UpdateState() { }
    }

    public class MoveState : MoveState<Customer>
    {
        public MoveState(Customer charecter, List<Vector3Int> waypoints) : base(charecter, waypoints) { }

        public override void SetNextState()
        {
            if (WayPoints.Count == 0)
            {
                if (Charecter.CellPosition == Charecter.Bar.ServiceCell)
                {
                    Charecter.Bar.Customer = Charecter;
                    var newTask = new GetOrderTask(Charecter.Bar);
                    newTask.Performed += Charecter.GetOrder;
                }

                Charecter.CurrentState = Charecter.IdleState;
            }
            else
            {
                // self transition
                Charecter.CurrentState = new MoveState(Charecter, WayPoints);
            }
        }
    }

    public class MoveToExitState : MoveState<Customer>
    {
        public MoveToExitState(Customer charecter, List<Vector3Int> waypoints) : base(charecter, waypoints) { }

        public override void SetNextState()
        {
            if (WayPoints.Count == 0)
            {
                Charecter.gameObject.SetActive(false);
            }
            else
            {
                // self transition
                Charecter.CurrentState = new MoveToExitState(Charecter, WayPoints);
            }
        }
    }
}