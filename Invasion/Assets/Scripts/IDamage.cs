using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{

    void hurtBaddies(int amount);

    public void delayDamage(int amount, float seconds);


}
