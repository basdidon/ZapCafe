using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : BoardObject
{
    TaskManager TaskManager { get => TaskManager.Instance; }
    public GameObject customerPrefab; //**** move to objectPool later
    public Transform spawnAt;

    // Customer
    [field: SerializeField] public Customer Customer { get; set; }
    [field: SerializeField] public Transform ServicePosition { get; set; }

    // Worker
    [field: SerializeField] public Worker Worker { get; set; }
    [field: SerializeField] public Transform WorkPosition { get; set; }

    // Events

    // Mono
    private void Start()
    {
        SpawnNewCustomer();
    }

    // Method
    public void SpawnNewCustomer()
    {
        var spawnPoint = BoardManager.GetCellCenterWorld(BoardManager.GetCellPos(spawnAt.position));
        Instantiate(customerPrefab,spawnPoint,Quaternion.identity);
    }

    public void AddNewCustomer(Customer customer)
    {
        Customer = customer;
        TaskManager.AddTasks(new GetOrderTask(this));
    }

    public class GetOrderTask : ITask
    {
        Bar Bar { get; } 
        public GetOrderTask(Bar bar)
        {
            Bar = bar;
        }

        public void Execute()
        {
            //Bar.StartCoroutine(GetOrder());
        }
        /*
        IEnumerator GetOrder()
        {

        }*/
    }

}
