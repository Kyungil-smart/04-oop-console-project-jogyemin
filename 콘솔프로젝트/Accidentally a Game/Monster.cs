using System;

namespace TurnBattleGame
{

    enum MonsterType
    {
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Boss
    }

    class Monster
    {

        public int HP;

        public int MaxHP;

        public bool IsBoss;

        public MonsterType Type;

        int atk;

        Random rand = new Random();

        public Monster(int stage)
        {

            if (stage == 5)
            {
                IsBoss = true;
                Type = MonsterType.Boss;
            }
            else
            {
                IsBoss = false;

                if (stage == 1) Type = MonsterType.Stage1;
                else if (stage == 2) Type = MonsterType.Stage2;
                else if (stage == 3) Type = MonsterType.Stage3;
                else Type = MonsterType.Stage4;
            }

            switch (Type)
            {
                case MonsterType.Stage1:
                    MaxHP = 50;
                    atk = 6;
                    break;

                case MonsterType.Stage2:
                    MaxHP = 80;
                    atk = 9;
                    break;

                case MonsterType.Stage3:
                    MaxHP = 120;
                    atk = 12;
                    break;

                case MonsterType.Stage4:
                    MaxHP = 180;
                    atk = 15;
                    break;

                case MonsterType.Boss:
                    MaxHP = 250;
                    atk = 18;
                    break;
            }

            HP = MaxHP;
        }

        public int GetDamage()
        {

            if (IsBoss && rand.Next(0, 3) == 0)
                return atk * 2;

            return atk;
        }
    }
}
