using System;
using ClientApp.Base;
using ClientApp.Client;

namespace ClientApp.Models
{
    public enum State
    {
        Stopped,
        Started
    }

    public abstract class AbstractItem : ViewModelBase
    {
        public abstract RemoteProcCommand ActivateCmd { get; }

        public abstract RemoteProcCommand DeactivateCmd { get; }

        public Guid Guid { get; } = Guid.NewGuid();

        public State Status { get; private set; } = State.Stopped;

        public string Name { get; set; } = "";

        public string Args { get; set; }= "";

        public ICommandsGroup Parent { get; set; }

        public void Start()
        {
            Status = State.Started;
        }

        public void Stop()
        {
            Status = State.Stopped;
        }
    }
}