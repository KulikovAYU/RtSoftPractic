using System;
using System.Collections.Generic;
using System.Text;
using ClientApp.Client;

namespace ClientApp.Models
{
    public class Process : AbstractItem
    {
        public override RemoteProcCommand ActivateCmd  => new(CommandType.eRunProc) { Name = Name, Args = Args};
        public override RemoteProcCommand DeactivateCmd => new(CommandType.eStopProc) { Name = Name, Args = Args };
    }
}
