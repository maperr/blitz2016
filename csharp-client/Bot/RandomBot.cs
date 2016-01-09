// Copyright (c) 2005-2016, Coveo Solutions Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Coveo.Bot;

namespace CoveoBlitz.RandomBot
{
    /// <summary>
    /// RandomBot
    ///
    /// This bot will randomly chose a direction each turns.
    /// </summary>
    public class RandomBot : ISimpleBot
    {
        private readonly Random random = new Random();

        /// <summary>
        /// This will be run before the game starts
        /// </summary>
        public void Setup()
        {
            Console.WriteLine("Coveo's C# RandomBot");
        }

        /// <summary>
        /// This will be run on each turns. It must return a direction fot the bot to follow
        /// </summary>
        /// <param name="state">The game state</param>
        /// <returns></returns>
        public string Move(GameState state)
        {

        }

        private string closesPath(GameState state, Pos start, Pos goal)
        {
            List<PathCoord> visited = new List<PathCoord>();
            List<PathCoord> availableTiles = new List<PathCoord>();

            var first = createPathCoord(start, null, state, goal);
            visited.Add(first);
            availableTiles.Add(first);

            while (availableTiles.Any())
            {
                // Sort availableTiles
                availableTiles.Sort((f1, f2) => f1.heuristic.CompareTo(f2.heuristic));

                var next = availableTiles.First(); // todo sort by heuristic

                if (next.current == goal)
                {
                    return Restitute(start, next);
                }
                else
                {
                    var spreadFrom = 
                }
            }
        }

        private string Restitute(Pos start, PathCoord foundPath)
        {

        }


        private List<PathCoord> getAvailableCoords(PathCoord current, GameState state, Pos goal)
        {
            PathCoord up = createPathCoord(new Pos() {x = current.current.x, y = current.current.y + 1}, current, state,
                goal);
            PathCoord down = createPathCoord(new Pos() { x = current.current.x, y = current.current.y - 1 }, current, state,
                goal);
            PathCoord left = createPathCoord(new Pos() { x = current.current.x - 1, y = current.current.y }, current, state,
                goal);
            PathCoord right = createPathCoord(new Pos() { x = current.current.x + 1, y = current.current.y }, current, state,
                goal);

            var rc = new List<PathCoord>();
            if (up != null)
            {
                rc.Add(up);
            }
            if (down != null)
            {
                rc.Add(down);
            }
            if (left != null)
            {
                rc.Add(left);
            }
            if (right != null)
            {
                rc.Add(right);
            }
            return rc;
        } 

        private PathCoord createPathCoord(Pos current, PathCoord previous, GameState state, Pos goal)
        {
            PathCoord pc = new PathCoord();

            if (state.board[current.x][current.y] == Tile.IMPASSABLE_WOOD)
            {
                return null;
            }

            if (previous != null)
            {
                pc.weight = previous.weight; // to modify
            }

            if (state.board[current.x][current.y] == Tile.SPIKES)
            {
                pc.weight += 3; // to modify
            }
            else
            {
                pc.weight += 1;
            }

            pc.heuristic = pc.weight + getDistance(current, goal);
            return pc;
        }

        private int getDistance(Pos a, Pos b)
        {
            return Math.Abs(b.x - a.x) + Math.Abs(b.y - a.y);
        }

        //public string Move(GameState state)
        //{
        //    string direction;

        //    switch (random.Next(0, 5))
        //    {
        //        case 0:
        //            direction = Direction.East;
        //            break;

        //        case 1:
        //            direction = Direction.West;
        //            break;

        //        case 2:
        //            direction = Direction.North;
        //            break;

        //        case 3:
        //            direction = Direction.South;
        //            break;

        //        default:
        //            direction = Direction.Stay;
        //            break;
        //    }

        //    Console.WriteLine("Completed turn {0}, going {1}", state.currentTurn, direction);
        //    return direction;
        //}

        /// <summary>
        /// This is run after the game.
        /// </summary>
        public void Shutdown()
        {
            Console.WriteLine("Done");
        }
    }
}