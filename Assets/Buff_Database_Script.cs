using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Database_Script : MonoBehaviour
{
    [System.Serializable]
    public class Buff
    {
        [Header("ID")]
        public BuffID buffID;
        [Header("Duration")]
        public float duration; //-1 for permanenet
        public float durationLeft;

        [Header("Stats")]
        public int meleeWeaponDamage;
        public BuffEffect meleeWeaponDamageCalculation;
        public float weaponAttackCooldown;
        public BuffEffect weaponAttackCooldownCalculation;

        public Buff(BuffID buffID, float durationIn, int meleeWeaponDamageIn, BuffEffect meleeWeaponDamageCalIn, float weaponAttackCooldownIn, BuffEffect weaponAttackCooldownCalIn)
        {
            this.buffID = buffID;
            this.duration = durationIn;
            this.meleeWeaponDamage = meleeWeaponDamageIn;
            this.meleeWeaponDamageCalculation = meleeWeaponDamageCalIn;
            this.weaponAttackCooldown = weaponAttackCooldownIn;
            this.weaponAttackCooldownCalculation = weaponAttackCooldownCalIn;
        }
    }

    public enum BuffID
    {
        sprayAndPrayBuff
    }

    public enum BuffEffect
    {
        add,
        subtract,
        multipleBy,
        divideBy,
        set
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Buff sprayAndPrayBuff = new Buff(BuffID.sprayAndPrayBuff, 3.0f, 0, BuffEffect.add, 0.1f, BuffEffect.set);
}
