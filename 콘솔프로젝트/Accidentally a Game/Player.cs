namespace TurnBattleGame
{

    class Player
    {

        public int HP = 100;

        public int MaxHP = 100;

        public int NormalAtk = 15;

        public int SkillAtk = 35;

        public int SkillCooldown = 0;

        public int MaxSkillCooldown = 5;

        public int Potion = 5;

        public void UseSkill()
        {
            SkillCooldown = MaxSkillCooldown;
        }

        public void ReduceCooldown()
        {
            if (SkillCooldown > 0)
                SkillCooldown--;
        }

        public void ReduceMaxCooldown()
        {
            if (MaxSkillCooldown > 1)
                MaxSkillCooldown--;
        }

    }
}
