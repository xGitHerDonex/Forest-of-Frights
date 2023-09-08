using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] int explosionAmount;

    void Start()
    {


    }
    //Lecture Code 9-8-23 if Iphysics is attached to the other collider then multiply the explosion amount to the transm=form normalized between 0-1
    private void OnTriggerEnter(Collider other)
    {
        #region suggestion on trigger vs trigger trip on enter

        if (other.isTrigger)
        {
            return;
        } 
        #endregion
        IPhysics physicable = other.GetComponent<IPhysics>();

        if (physicable != null)
        {
            physicable.physics((other.transform.position - transform.position).normalized * explosionAmount);
        }
    }
}
