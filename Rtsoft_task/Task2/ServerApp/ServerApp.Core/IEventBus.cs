using System;

namespace ServerApp.Core
{
    public interface IEventBus
    {
        void Print(string message);

        void Error(string message);
    }


    // Производный класс от EventArgs
    public class MessageEventArgs : EventArgs
    {
        public string Message;
    }

    public sealed class EventBusImpl : IEventBus
    {
        public event EventHandler ErrorHappened;
        public event EventHandler PrintHappened;

        public void Error(string message) => ErrorHappened?.Invoke(this, new MessageEventArgs() { Message = message });
        public void Print(string message) => PrintHappened?.Invoke(this, new MessageEventArgs() { Message = message });
    }
}