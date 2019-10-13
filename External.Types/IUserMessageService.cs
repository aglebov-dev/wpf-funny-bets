namespace Company.Client.Common.Interfaces
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Тип сообщения
    /// </summary>
    public enum UserMessageType
    {
        Information,
        Warring,
        Error,
        Confirmation
    }

    /// <summary>
    /// Сообщение
    /// </summary>
    public class UserMessage
    {
        public UserMessageType Type { get; }
        public string Message { get; }
        public UserMessage(UserMessageType type, string message)
        {
            Type = type;
            Message = message;
        }
    }

    /// <summary>
    /// Интерфейс для простых уведомлений пользователю
    /// </summary>
    public interface IUserMessageService
    {
        ReadOnlyObservableCollection<UserMessage> Messages { get; }

        void ShowError(string message);
        void ShowWarring(string message);
        void ShowConfirmation(string message);
        void ShowInformation(string message);
    }

    /// <summary>
    /// Реализация IUserMessageService
    /// </summary>
    public abstract class UserMessageService : IUserMessageService
    {
        ObservableCollection<UserMessage> MessagesSource;
        public ReadOnlyObservableCollection<UserMessage> Messages { get; }

        public UserMessageService()
        {
            MessagesSource = new ObservableCollection<UserMessage>();
            Messages = new ReadOnlyObservableCollection<UserMessage>(MessagesSource);
        }

        public virtual void ShowError(string message)
        {
            var messageModel = new UserMessage(UserMessageType.Error, message);
            AddMessage(messageModel);
        }
        public virtual void ShowWarring(string message)
        {
            var messageModel = new UserMessage(UserMessageType.Warring, message);
            AddMessage(messageModel);
        }

        public virtual void ShowConfirmation(string message)
        {
            var messageModel = new UserMessage(UserMessageType.Confirmation, message);
            AddMessage(messageModel);
        }
        public virtual void ShowInformation(string message)
        {
            var messageModel = new UserMessage(UserMessageType.Information, message);
            AddMessage(messageModel);
        }

        void AddMessage(UserMessage message)
        {
            MessagesSource.Add(message);
            if (Messages.Count > 100)
            {
                var item = Messages[Messages.Count - 1];
                MessagesSource.Remove(item);
            }
        }
    }
}
