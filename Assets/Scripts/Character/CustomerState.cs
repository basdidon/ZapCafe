using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomerState
{
    public class IdleState : IState
    {
        public void EnterState() { }
        public void ExitState() { }
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
                    /*Debug.Log("a");
                    TaskManager.Instance.AddTask(newTask);*/
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