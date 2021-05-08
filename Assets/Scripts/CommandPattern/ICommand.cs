using System;
namespace CommandStructure
{
    /// <summary>
    /// The 'ICommand' interface that we will record from
    /// </summary>
public  interface ICommand
    {
         void Execute();
         void UnExecute();
    }
    
}
