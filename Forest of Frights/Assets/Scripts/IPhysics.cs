using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPhysics
{
    //interface method for physics interaction currently only implements pushback 9-8-23
    public void physics(Vector3 dir, bool explosion);

}
