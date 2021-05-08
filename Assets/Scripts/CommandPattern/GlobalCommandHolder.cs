

using UnityEngine;
using System.Collections.Generic;
using CommandUndoRedo;

public class GlobalCommandHolder : MonoBehaviour
{
    public static GlobalCommandHolder Instance;

    [SerializeField]
    private List<ICommand> commands = new List<ICommand>();
    private int currentCommandNum = 0;
    

    public static int _propertyCurrentCommantNum
    {
        get { return Instance.currentCommandNum; }
        set
        {
            Instance.currentCommandNum = value;
            if (Instance.currentCommandNum == 0)
            {
                //grey out undo button and redo button based on the required iterations 
            }
        }
    }


    public static List<ICommand> _propertyCommands
    {
        get
        {
            return Instance.commands;
        }
        set
        {

            //if (Instance.commands.Count > 12)//set the command history limit
            //{
            //    Debug.Log("========  command limit reached ============");
            //    return;
            //}
            Instance.commands = value; print(Instance.commands.Count);

        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void Undo()
    {
        if (_propertyCurrentCommantNum > 0)
        {
            _propertyCurrentCommantNum--;
            ICommand Command = _propertyCommands[_propertyCurrentCommantNum-1];
            Command.UnExecute();
        }
    }

    public void Redo()
    {
        if (_propertyCurrentCommantNum < _propertyCommands.Count)
        {
            ICommand Command = _propertyCommands[_propertyCurrentCommantNum];
            _propertyCurrentCommantNum++;
            Command.Execute();
        }
    }


    public static void AddCommands(ICommand command)
    {
        _propertyCommands.Add(command);
        _propertyCurrentCommantNum++;
    }

    //sample code
    //private void Move(MoveDirection direction)
    //{
    //    ICommand moveCommand = new MoveCommand();
    //    moveCommand.Execute();
    //    _propertyCommands.Add(moveCommand);
    //    _propertyCurrentCommantNum++;
    //}



    //Shows what's going on in the command list
    //void OnGUI()
    //{
    //    string label = "   start";
    //    if (currentCommandNum == 0)
    //    {
    //        label = ">" + label;
    //    }
    //    label += "\n";

    //    for (int i = 0; i < commands.Count; i++)
    //    {
    //        if (i == currentCommandNum - 1)
    //            label += "> " + commands[i].ToString() + "\n";
    //        else
    //            label += "   " + commands[i].ToString() + "\n";

    //    }
    //    GUI.Label(new Rect(0, 0, 400, 800), label);
    //}
}
