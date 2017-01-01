namespace Bibliotheca.Server.Gateway.Core.Model
{
    public class ActionConfirmation
    {
        public bool WasSuccess { get; set; }
        public string Message { get; set; }

        protected ActionConfirmation()
        {
        }

        public static ActionConfirmation CreateSuccessfull()
        {
            return new ActionConfirmation
            {
                WasSuccess = true
            };
        }

        public static ActionConfirmation CreateError(string message)
        {
            return new ActionConfirmation
            {
                WasSuccess = false,
                Message = message
            };
        }
    }

    public class ActionConfirmation<T> : ActionConfirmation
    {
        public T ObjectData { get; set; }

        public static ActionConfirmation<T> CreateSuccessfull(T objectData)
        {
            return new ActionConfirmation<T>
            {
                WasSuccess = true,
                ObjectData = objectData
            };
        }

        public static new ActionConfirmation<T> CreateError(string message)
        {
            return new ActionConfirmation<T>
            {
                WasSuccess = false,
                Message = message
            };
        }
    }
}
