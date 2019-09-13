using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Console
{
    public abstract class ConsoleCommand
    {
        public abstract string Name { get; protected set; }
        public abstract string Command { get; protected set; }
        public abstract string Description { get; protected set; }
        public abstract string Help { get; protected set; }

        public void AddCommandToConsole()
        {
            DevConsole.AddCommandsToConsole(Command, this);
        }

        public abstract void RunCommand();
    }

    public class CommandQuit : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }

        public CommandQuit()
        {
            Name = "Quit";
            Command = "quit";
            Description = "Quits the application";
            Help = "Use this command with no arguments to force Unity to quit!";

            AddCommandToConsole();
        }

        public override void RunCommand()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        public static CommandQuit CreateCommand()
        {
            return new CommandQuit();
        }
    }

    public class RestartCommand : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Help { get; protected set; }

        public RestartCommand()
        {
            Name = "Restart";
            Command = "restart";
            Description = "Restarts the application";
            Help = "Use this command with no arguments to restart the scene";

            AddCommandToConsole();
        }

        public override void RunCommand()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static RestartCommand CreateCommand()
        {
            return new RestartCommand();
        }
    }
}
