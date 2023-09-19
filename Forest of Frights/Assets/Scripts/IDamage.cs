using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{

    void takeDamage(int amount);

    public void delayDamage(int amount, float seconds);


}
