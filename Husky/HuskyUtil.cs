using System;
using System.Collections.Generic;

namespace Husky
{
    using GameDefinition = Tuple<uint, uint, string, Action<uint, uint, string, Action<object>>>;

    public class HuskyUtil
    {
        private static readonly Dictionary<string, GameDefinition> Games = new Dictionary<string, GameDefinition>()
        {
            {"iw4mp", new GameDefinition(0x823D24B8, 0x823D2100, "mp", ModernWarfare2.ExportBSPData)},
        };


        /// <summary>
        /// Looks for matching game and loads BSP from it
        /// </summary>
        public static void LoadGame(Action<object> printCallback = null)
        {
            try
            {
                // Get all processes
                //var processes = Process.GetProcesses();
                //XRPC console = new XRPC();
                //console.Connect();
                //// Loop through them, find match
                //foreach (var process in processes)
                //{
                //    // Check for it in dictionary
                //    if (Games.TryGetValue(process.ProcessName, out var game))
                //    {
                //        // Export it
                //        game.Item4(game.Item1, game.Item2, game.Item3, printCallback);

                //        // Done
                //        return;
                //    }
                //}

                if (Games.TryGetValue("iw4mp", out var game))
                    game.Item4(game.Item1, game.Item2, game.Item3, printCallback);
                // Failed
                printCallback?.Invoke("Failed to find a supported game, please ensure one of them is running.");
            }
            catch (Exception e)
            {
                printCallback?.Invoke("An unhandled exception has occured:");
                printCallback?.Invoke(e);
            }
        }
    }
}
