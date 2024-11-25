using System;
using SaturnEngine.Engine.Components;

namespace SaturnEngine.Engine.Core
{
    public static class Log
    {

        public static bool ConsoleOutput { get; set; } = true;
        public static bool FileOutput { get; set; } = false;

        #region Debug

        public static void Debug(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Write(msg);
            Console.ResetColor();
        }

        public static void Debug(Entity entity, string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Write($"[Entity \"{entity.Name}\"({entity.Id})] {msg}");
            Console.ResetColor();
        }

        public static void Debug(ComponentBase component, string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Write($"[Entity \"{component.Entity.Name}\"({component.Entity.Id}), Component \"{component.GetType()}\"] {msg}");
            Console.ResetColor();
        }

        #endregion

        #region Info

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Write(msg);
            Console.ResetColor();
        }

        #endregion

        #region Warning

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Write(msg);
            Console.ResetColor();
        }

        public static void Warning(Entity entity, string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Write($"[Entity \"{entity.Name}\"({entity.Id})] {msg}");
            Console.ResetColor();
        }

        public static void Warning(ComponentBase component, string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Write($"[Entity \"{component.Entity.Name}\"({component.Entity.Id}), Component \"{component.GetType()}\"] {msg}");
            Console.ResetColor();
        }

        #endregion

        #region Error

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Write(msg);
            Console.ResetColor();
        }

        public static void Error(Entity entity, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Write($"[Entity \"{entity.Name}\"({entity.Id})] {msg}");
            Console.ResetColor();
        }

        public static void Error(ComponentBase component, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Write($"[Entity \"{component.Entity.Name}\"({component.Entity.Id}), Component \"{component.GetType()}\"] {msg}");
            Console.ResetColor();
        }

        #endregion

        private static void Write(string msg)
        {
            if (ConsoleOutput)
            {
                Console.WriteLine($"[{DateTime.Now}] {msg}");
            }

            if (FileOutput)
            {

            }
        }

    }
}
