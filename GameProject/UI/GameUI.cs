using GameProject.Models;
using GameProject.Services;
using System;
using System.Collections.Generic;

namespace GameProject.UI
{
    // The UI displays information and collects input.
    public class GameUI
    {
        private readonly GameService _service;
        public GameUI(GameService service) { _service = service; }

        public void Start()
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                ShowTitle();
                Console.WriteLine("1. Start game");
                Console.WriteLine("2. View scores");
                Console.WriteLine("3. How to play");
                Console.WriteLine("4. Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();



                if (choice == "1") PlayGame();
                else if (choice == "2") ShowScores();
                else if (choice == "3") ShowInstructions();
                else if (choice == "4") running = false;
                else { Console.WriteLine("Invalid option. Press Enter."); Console.ReadLine(); }
            }
        }

        private void PlayGame()
        {
            Console.Clear();
            Console.Write("Player name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = "Player";

            Player player = _service.CreatePlayer(name);
            List<Monster> monsters = _service.CreateMonsters();

            foreach (Monster monster in monsters)
            {
                Console.WriteLine("\nA " + monster.Name + " appears!");
                RunBattle(player, monster);
                if (!player.IsAlive()) break;
                _service.GiveMonsterReward(player, monster);
                Console.WriteLine(monster.Name + " defeated! Score: " + player.Score);
            }

            bool won = player.IsAlive();
            if (won)
            {
                int bonus = _service.CalculateVictoryBonus(player);
                player.AddScore(bonus);
                Console.WriteLine("\nYou defeated every monster! Bonus: " + bonus);
            }
            else Console.WriteLine("\nYou were defeated.");

            _service.SaveResult(player, won);
            Console.WriteLine("Final score: " + player.Score);
            Pause();
        }

        private void RunBattle(Player player, Monster monster)
        {
            while (player.IsAlive() && monster.IsAlive())
            {
                ShowBattleStatus(player, monster);
                Console.WriteLine("1. Attack");
                Console.WriteLine("2. Use healing potion");
                Console.Write("Action: ");
                string action = Console.ReadLine();

                if (action == "1")
                    Console.WriteLine("You dealt " + _service.PlayerAttack(player, monster) + " damage.");
                else if (action == "2")
                    Console.WriteLine(player.UseHealingPotion() ? "Health restored." : "No potions left.");
                else
                    Console.WriteLine("Invalid action. You lose this turn.");

                if (monster.IsAlive())
                    Console.WriteLine(monster.Name + " dealt " + _service.MonsterAttack(monster, player) + " damage.");
            }
        }

        private void ShowBattleStatus(Player player, Monster monster)
        {
            Console.WriteLine("\n-----------------------------");
            Console.WriteLine(player.Name + " HP: " + player.Health + " | Potions: " + player.HealingPotions);
            Console.WriteLine(monster.Name + " HP: " + monster.Health);
            Console.WriteLine("-----------------------------");
        }

        private void ShowScores()
        {
            Console.Clear();
            Console.WriteLine("=== SAVED SCORES ===");
            // GetTopScores is an advanced student exercise in the service layer.
            List<GameResult> scores = _service.GetTopScores(5);
            if (scores.Count == 0) Console.WriteLine("No saved games yet.");
            foreach (GameResult score in scores)
                Console.WriteLine(score.PlayerName + " - " + score.Score + " - " + score.Result);
            Pause();
        }

        private void ShowTitle()
        {
            Console.WriteLine("=============================");
            Console.WriteLine("     MONSTER BATTLE ARENA");
            Console.WriteLine("=============================");
        }

        private void ShowInstructions()
        {
            // STUDENT TODO (UI task): explain attack, healing, winning, losing,
            // and scoring here. Clear the screen first and call Pause() last.
            Console.Clear();
            Console.WriteLine("\nPlayer had to set is name.");
            Console.WriteLine("\nConsole will show the monster name and player name.");
            Console.WriteLine("\nPlayer can see his hp and the monster hp");
            Console.WriteLine("\nIf the player press 1 it will attack on monster .");
            Console.WriteLine("\nIf player press 3 the monster will attack only ");
            Console.WriteLine("\nBy pressing 2 the plater can get a healing potions max limit is 3 ");
            Console.WriteLine("\nEach potion will award 10 points in the HP to player.");
            Console.WriteLine("\nPlayer each unused healing potions will award you 10 points.");
            Console.WriteLine("\nIf player health becames below 0 you will loose.");
            Console.WriteLine("\nOnce the player defeated by monster, He will get 0 points in reward ");
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("\nPress Enter to return to the menu.");
            Console.ReadLine();
        }
    }
}
