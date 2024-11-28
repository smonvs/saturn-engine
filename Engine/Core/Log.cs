using System;
using SaturnEngine.Engine.Components;

namespace SaturnEngine.Engine.Core
{
    public static class Log
    {

        public static bool ConsoleOutput { get; set; } = true;
        public static bool FileOutput { get; set; } = false;

        private static ConsoleColor _debugColor = ConsoleColor.DarkGray;
        private static ConsoleColor _infoColor = ConsoleColor.White;
        private static ConsoleColor _warningColor = ConsoleColor.DarkYellow;
        private static ConsoleColor _errorColor = ConsoleColor.Red;

        #region Debug

        public static void Debug(string msg)
        {
            Console.ForegroundColor = _debugColor;
            Write(msg);
            Console.ResetColor();
        }

        public static void Debug(Entity entity, string msg)
        {
            Console.ForegroundColor = _debugColor;
            Write($"Entity \"{entity.Name}\" ({entity.Id}): {msg}");
            Console.ResetColor();
        }

        public static void Debug(ComponentBase component, string msg)
        {
            Console.ForegroundColor = _debugColor;
            Write($"[Entity \"{component.Entity.Name}\"({component.Entity.Id}), Component \"{component.GetType()}\"] {msg}");
            Console.ResetColor();
        }

        #endregion

        #region Info

        public static void Info(string msg)
        {
            Console.ForegroundColor = _infoColor;
            Write(msg);
            Console.ResetColor();
        }

        public static void Info(Entity entity, string msg)
        {
            Console.ForegroundColor = _infoColor;
            Write($"Entity \"{entity.Name}\" ({entity.Id}): {msg}");
            Console.ResetColor();
        }

        #endregion

        #region Warning

        public static void Warning(string msg)
        {
            Console.ForegroundColor = _warningColor;
            Write(msg);
            Console.ResetColor();
        }

        public static void Warning(Entity entity, string msg)
        {
            Console.ForegroundColor = _warningColor;
            Write($"Entity \"{entity.Name}\" ({entity.Id}): {msg}");
            Console.ResetColor();
        }

        public static void Warning(ComponentBase component, string msg)
        {
            Console.ForegroundColor = _warningColor;
            Write($"[Entity \"{component.Entity.Name}\"({component.Entity.Id}), Component \"{component.GetType()}\"] {msg}");
            Console.ResetColor();
        }

        #endregion

        #region Error

        public static void Error(string msg)
        {
            Console.ForegroundColor = _errorColor;
            Write(msg);
            Console.ResetColor();
        }

        public static void Error(Entity entity, string msg)
        {
            Console.ForegroundColor = _errorColor;
            Write($"Entity \"{entity.Name}\" ({entity.Id}): {msg}");
            Console.ResetColor();
        }

        public static void Error(ComponentBase component, string msg)
        {
            Console.ForegroundColor = _errorColor;
            Write($"[Entity \"{component.Entity.Name}\"({component.Entity.Id}), Component \"{component.GetType()}\"] {msg}");
            Console.ResetColor();
        }

        #endregion

        private static void Write(string msg)
        {
            if (ConsoleOutput)
            {
                #if DEBUG
                Console.WriteLine($"[{DateTime.Now}] {msg}");
                #endif
            }

            if (FileOutput)
            {

            }
        }

    }
}
