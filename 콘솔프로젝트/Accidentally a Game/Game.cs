using System;
using System.Threading;

namespace TurnBattleGame
{

    internal class Game
    {

        Player player = null!;

        Monster monster = null!;

        bool isDefending;

        bool monsterHit;

        bool playerHit;

        public void Run()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.CursorVisible = false;

            Console.SetWindowSize(120, 35);
            Console.SetBufferSize(120, 35);

            ShowTitle();

            player = new Player();

            for (int stage = 1; stage <= 5 && player.HP > 0; stage++)
            {

                player.Potion = 5;

                monster = new Monster(stage);

                Console.Clear();
                Console.WriteLine(monster.IsBoss ? "최종 보스 등장!" : $"스테이지 {stage}");
                Thread.Sleep(1200);

                BattleLoop();

                if (player.HP > 0) StageReward();
                StageReward();
            }

            Console.Clear();
            Console.WriteLine(player.HP > 0 ? "게임 클리어!" : "게임 오버");
            Console.ReadKey();
        }

        void ShowTitle()
        {
            string[] title =
            {

             " ███████╗ ██████╗ ███╗   ███╗███████╗██╗  ██╗ ██████╗ ██╗    ██╗",
             " ██╔════╝██╔═══██╗████╗ ████║██╔════╝██║  ██║██╔═══██╗██║    ██║",
             " ███████╗██║   ██║██╔████╔██║█████╗  ███████║██║   ██║██║ █╗ ██║",
             " ╚════██║██║   ██║██║╚██╔╝██║██╔══╝  ██╔══██║██║   ██║██║███╗██║",
             " ███████║╚██████╔╝██║ ╚═╝ ██║███████╗██║  ██║╚██████╔╝╚███╔███╔╝",
             " ╚══════╝ ╚═════╝ ╚═╝     ╚═╝╚══════╝╚═╝  ╚═╝ ╚═════╝  ╚══╝╚══╝",
             "                                                               ",
             "Accidentally a Game"
             };

            string[] menu =
             {
             "1. 게임 시작",
             "2. 게임 종료"
             };

            while (true)
            {
                Console.Clear();

                int consoleWidth = Console.WindowWidth;
                int consoleHeight = Console.WindowHeight;

                int titleWidth = title.Max(line => line.Length);
                int menuWidth = menu.Max(line => line.Length);

                int blockWidth = Math.Max(titleWidth, menuWidth);

                int titleHeight = title.Length;
                int menuHeight = menu.Length;

                int spacing = 2; 

                int blockHeight = titleHeight + spacing + menuHeight;

                int startX = (consoleWidth - blockWidth) / 2;
                int startY = (consoleHeight - blockHeight) / 2;

                for (int i = 0; i < title.Length; i++)
                {
                    Console.SetCursorPosition(
                         (consoleWidth - title[i].Length) / 2,
                          startY + i
                    );
                    Console.Write(title[i]);
                }

                for (int i = 0; i < menu.Length; i++)
                {
                    Console.SetCursorPosition(
                        (consoleWidth - menu[i].Length) / 2,
                        startY + titleHeight + spacing + i
                    );
                    Console.Write(menu[i]);
                }

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.D1) return;
                if (key == ConsoleKey.D2) Environment.Exit(0);
            }
        }

        void BattleLoop()
        {

            while (player.HP > 0 && monster.HP > 0)
            {

                PlayerTurn();

                if (monster.HP <= 0) break;

                MonsterTurn();
            }
        }

        void PlayerTurn()
        {

            isDefending = false;

            bool done = false;

            while (!done)
            {
                Console.Clear();
                DrawScene();
                DrawMenu();

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.D1)
                {

                    AnimateSlash();
                    Thread.Sleep(120);

                    monsterHit = true;
                    Console.Clear();
                    DrawScene();
                    Thread.Sleep(200);
                    monsterHit = false;

                    DealDamage(monster, player.NormalAtk);

                    done = true;
                }

                else if (key == ConsoleKey.D2 && player.SkillCooldown == 0)
                {

                    AnimateSkill();
                    Thread.Sleep(150);

                    Console.Clear();
                    DrawScene();
                    Thread.Sleep(150);

                    DealDamage(monster, player.SkillAtk);

                    player.UseSkill();
                    done = true;
                }

                else if (key == ConsoleKey.D3)
                {
                    isDefending = true;
                    done = true;
                }

                else if (key == ConsoleKey.D4 && player.Potion > 0)
                {
                    player.Potion--;
                    HealPlayer(40);
                    done = true;
                }

                player.ReduceCooldown();
            }
        }

        void MonsterTurn()
        {
            playerHit = false;
            monsterHit = false;

            Console.Clear();
            DrawScene();

            Console.SetCursorPosition(Define.MONSTER_X - 5, Define.CHAR_Y - 4);
            Console.Write("(((");
            Thread.Sleep(500);

            if (monster.IsBoss)
                AnimateBossAttack();
            else
                AnimateMonsterAttack();

            int dmg = monster.GetDamage();

            if (isDefending)
                dmg /= 2;

            DealDamage(player, dmg);
        }

        void AnimateSlash()
        {

            for (int x = Define.PLAYER_X + 4; x < Define.MONSTER_X - 4; x += 2)
            {
                Console.Clear();
                DrawScene();
                Console.SetCursorPosition(x, Define.CHAR_Y - 2);
                Console.Write(">>>>>");
                Thread.Sleep(20);
            }
        }

        void AnimateSkill()
        {
            for (int y = Define.CHAR_Y - 6; y <= Define.CHAR_Y - 2; y++)
            {
                Console.Clear();
                DrawScene();
                Console.SetCursorPosition(Define.MONSTER_X - 3, y);
                Console.Write("&&&&");
                Thread.Sleep(60);
            }
        }

        void AnimateMonsterAttack()
        {

            for (int x = Define.MONSTER_X - 2; x > Define.PLAYER_X + 2; x -= 2)
            {
                Console.Clear();
                DrawScene();

                Console.SetCursorPosition(x, Define.CHAR_Y - 2);
                Console.Write("ooooo");

                Thread.Sleep(40);
            }
        }

        void AnimateBossAttack()
        {
            for (int x = Define.MONSTER_X - 6; x > Define.PLAYER_X + 2; x -= 2)
            {
                Console.Clear();
                DrawScene();

                int centerY = Define.CHAR_Y - 2;

                Console.SetCursorPosition(x + 4, centerY - 2);
                Console.Write("🔥🔥");

                Console.SetCursorPosition(x + 2, centerY - 1);
                Console.Write("🔥🔥🔥");

                Console.SetCursorPosition(x, centerY);
                Console.Write("🔥🔥🔥🔥");

                Console.SetCursorPosition(x + 2, centerY + 1);
                Console.Write("🔥🔥🔥");

                Console.SetCursorPosition(x + 4, centerY + 2);
                Console.Write("🔥🔥");

                Thread.Sleep(50);
            }
        }

        void DealDamage(dynamic target, int dmg)
        {
            for (int i = 0; i < dmg; i++)
            {

                if (target is Player && player.HP <= 0) break;
                if (target is Monster && monster.HP <= 0) break;

                playerHit = false;
                monsterHit = false;

                if (target is Player)
                {
                    player.HP--;
                    playerHit = true;

                    if (player.HP < 0)
                        player.HP = 0;
                }
                else if (target is Monster)
                {
                    monster.HP--;
                    monsterHit = true;

                    if (monster.HP < 0)
                        monster.HP = 0;
                }

                Console.Clear();
                DrawScene();
                Thread.Sleep(15);
            }

            playerHit = false;
            monsterHit = false;
        }

        void HealPlayer(int amount)
        {
            for (int i = 0; i < amount && player.HP < player.MaxHP; i++)
            {
                player.HP++;
                Console.Clear();
                DrawScene();
                Thread.Sleep(15);
            }
        }

        void DrawScene()
        {
            DrawHP();
            DrawCharacters();
        }

        void DrawHP()
        {
            int hpY = Define.CHAR_Y + 2;

            Console.SetCursorPosition(Define.PLAYER_X, hpY);
            Console.Write("HP ");
            DrawBar(player.HP, player.MaxHP);
            Console.Write($" ({player.HP} / {player.MaxHP})");

            Console.SetCursorPosition(Define.MONSTER_X, hpY);
            Console.Write("HP ");
            DrawBar(monster.HP, monster.MaxHP);
            Console.Write($" ({monster.HP} / {monster.MaxHP})");

            if (monster.IsBoss)
            {
                Console.SetCursorPosition(Define.MONSTER_X, hpY + 1);
                DrawBar(monster.HP, monster.MaxHP);
            }
        }

        void DrawCharacters()
        {

            Console.SetCursorPosition(Define.PLAYER_X, Define.CHAR_Y - 3);
            Console.Write(playerHit ? "X " : " O "); 
            Console.SetCursorPosition(Define.PLAYER_X, Define.CHAR_Y - 2);
            Console.Write(playerHit ? "(X) " : "L|( "); 
            Console.SetCursorPosition(Define.PLAYER_X, Define.CHAR_Y - 1);
            Console.Write(" | "); 
            Console.SetCursorPosition(Define.PLAYER_X, Define.CHAR_Y);
            Console.Write("| |"); 

            if (monster.IsBoss)
            {

                string[] dragonArt =
        {
            "      ,===:'.,            `-._",
            "           `:.`---.__         `-._",
            "             `:.     `--.         `.",
            "               \\.        `.         `.",
            "       (,,(,    \\.         `.   ____,-`.,",
            "    (,'     `/   \\.   ,--.___`.'",
            ",  ,'  ,--.  `,   \\.;'         `",
            " `{D, {    \\  :    \\;",
            "   V,,'    /  /    
            "   j;;    /  ,' ,-
            "   \\;'   /  ,' /  _  \\  /  _  \\   ,'/",
            "         \\   `'  / \\  `'  / \\  `.' /",
            "          `.___,'   `.__,'   `.__,'"
        };

                int startY = Define.CHAR_Y - dragonArt.Length + 1;

                for (int i = 0; i < dragonArt.Length; i++)
                {

                    Console.SetCursorPosition(Define.MONSTER_X, startY + i);
                    Console.Write(dragonArt[i]);
                }
            }

            else
            {

                if (monster.Type == MonsterType.Stage1)
                {
                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 3);
                    Console.Write(monsterHit ? "X!!X" : "^..^");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 2);
                    Console.Write(monsterHit ? "(XX) )~" : "(oo) )~");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 1);
                    Console.Write(",, ,,");
                }

                else if (monster.Type == MonsterType.Stage2)
                {
                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 3);
                    Console.Write(monsterHit ? "####" : "<..>");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 2);
                    Console.Write(monsterHit ? "####" : "/||\\");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 1);
                    Console.Write(" || ");
                }

                else if (monster.Type == MonsterType.Stage3)
                {
                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 3);
                    Console.Write(monsterHit ? "!!!!" : "[@@]");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 2);
                    Console.Write(monsterHit ? "!!!!" : "{==}");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 1);
                    Console.Write(" || ");
                }

                else if (monster.Type == MonsterType.Stage4)
                {
                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 3);
                    Console.Write(monsterHit ? "%%%%" : "{##}");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 2);
                    Console.Write(monsterHit ? "%%%%" : "||||");

                    Console.SetCursorPosition(Define.MONSTER_X, Define.CHAR_Y - 1);
                    Console.Write(" || ");
                }
            }
        }

        void DrawBar(int hp, int max)
        {

            int bars = (hp * 10) / max;
            Console.Write("[");

            for (int i = 0; i < 10; i++)
                Console.Write(i < bars ? "■" : " ");
            Console.Write("]");
        }

        void DrawMenu()
        {
            Console.SetCursorPosition(0, Define.UI_Y);
            Console.WriteLine("1. 공격");
            Console.WriteLine($"2. 스킬 (쿨 {player.SkillCooldown})");
            Console.WriteLine("3. 방어");
            Console.WriteLine($"4. 회복 ({player.Potion})");
        }

        void StageReward()
        {
            Console.Clear();
            Console.WriteLine("보상을 선택하세요");
            Console.WriteLine("1. 최대 HP +10");
            Console.WriteLine("2. 공격력 +2");
            Console.WriteLine("3. 스킬 쿨타임 감소");

            while (true)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.D1)
                {
                    player.MaxHP += 10;
                    player.HP += 10;
                    break;
                }

                if (key == ConsoleKey.D2)
                {
                    player.NormalAtk += 2;
                    player.SkillAtk += 2;
                    break;
                }

                if (key == ConsoleKey.D3 && player.MaxSkillCooldown > 1)
                {
                    player.ReduceMaxCooldown();
                    break;
                }
            }
        }
    }
}
