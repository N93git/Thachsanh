using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSC : HeroesSC
{
    // Start is called before the first frame update
    void Start()
    {
        base.manamax = 120;
        healthmax = 80;
        manacurrent = manamax;
        base.healthcurrent = healthmax;
        manaregen = 5;
        healthregen = 2;
        Basestart();
    }
    private float manaSkill = 20;
    // Update is called once per frame
    void Update()
    {
        BaseUpdate();
    }
    public override void BaseAttack()
    {
        if (manacurrent > manaSkill)
        {
            GetMana(manaSkill);
            StartCoroutine(TransitionToAttack());
        }
        else
        {
            Debug.Log("Khong đủ mana");
        }

    }
    void FixedUpdate()
    {
        BaseFixUpdate();
    }
}
