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
        public BuffOperator meleeWeaponDamageOperator;

        public float weaponAttackCooldown;
        public BuffOperator weaponAttackCooldownOperator;

        public int projectilePierce;
        public BuffOperator projectilePierceOperator;

        public float projectileSpeed;
        public BuffOperator projectileSpeedOperator;

        public float moveSpeed;
        public BuffOperator moveSpeedOperator;

        public Buff(BuffID buffID, float durationIn)
        {
            this.buffID = buffID;
            this.duration = durationIn;
            //Set all operators to unused.
            //Operators will be set in the inspector alongside all values.
            this.meleeWeaponDamageOperator = BuffOperator.unused; 
            this.weaponAttackCooldownOperator = BuffOperator.unused;
            this.projectilePierceOperator = BuffOperator.unused;
            this.projectileSpeedOperator = BuffOperator.unused;
            this.moveSpeedOperator = BuffOperator.unused;
        }
    }

    public enum BuffID
    {
        sprayAndPray,
        piercingShot,
        fasterBullets,
        fleetOfFoot
    }

    public enum BuffOperator
    {
        add,
        subtract,
        multipleBy,
        divideBy,
        set,
        unused
    }

    /* To Add New Buff:
     *      1. Add buff below.
     *      2. Add BuffID to BuffID enum
     *      3. Define the stats of the Buff in the inspector
     */

    /* To Add new Stats that a buff may effect:
     *      1. Add the value to the Stats section above.
     *      2. Add a buffOperator field for the stat.
     *      3. Set the buffOperator field to unused in the buff constructor
     *      4. Add appliance of this stat to Minion_Movement_Script.applyBuffToStat()
     */

    public Buff sprayAndPrayBuff = new Buff(BuffID.sprayAndPray, 3.0f);
    public Buff fasterBulletsBuff = new Buff(BuffID.fasterBullets, 5.0f);
    public Buff fleetOfFootBuff = new Buff(BuffID.fleetOfFoot, 5.0f);

    //public Buff piercingShotBuff = new Buff(BuffID.sprayAndPrayBuff, 3.0f, 0, BuffEffect.add, 0.1f, BuffEffect.set);
}
