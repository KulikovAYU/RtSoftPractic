using System;
using System.Collections.ObjectModel;
using ClientApp.Base;

namespace ClientApp.Models
{
    public interface ICommandsGroup
    {
        string Name { get;}
        Action New { get; }
        public ObservableCollection<AbstractItem> Commands { get; }
    }

    public class CommandsGroup<T> : ViewModelBase, ICommandsGroup where T : AbstractItem, new()
    {
        protected CommandsGroup()
        {
            New = () =>
            {
                var itm = new T();
                itm.Parent = this;
                Commands.Add(itm);
            };
        }

        public string Name { get; } = typeof(T).Name;

        public Action New { get; }

        public ObservableCollection<AbstractItem> Commands { get;  } = new();
    }

    public class RunProcessCommandsGroup : CommandsGroup<Process>{}
    
    public class RunDbusCommandsGroup : CommandsGroup<Service>{}
}