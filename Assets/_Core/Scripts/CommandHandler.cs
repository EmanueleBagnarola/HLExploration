using System;
using System.Collections.Generic;
using UnityEngine;

public static class CommandHandler 
{
    private static Queue<Command> commandsQueue = new Queue<Command>();

    private static bool isProcessing;
    
    public static void Execute(Command _command)
    {
        commandsQueue.Enqueue(_command);
        
        _command.Execute();
    }

    public static void Undo()
    {
        if(commandsQueue.Count < 0)
            return;
        
        using Command undoCommand = commandsQueue.Dequeue();
        undoCommand.Undo();
    }
}

public abstract class Command: IDisposable
{
    public abstract void Execute();

    public abstract void Undo();
    
    public abstract void Dispose();
}

public class MoveOnGridCommand : Command
{
    public override void Execute()
    {
    }

    public override void Undo()
    {
    }

    public override void Dispose()
    {
    }
}
