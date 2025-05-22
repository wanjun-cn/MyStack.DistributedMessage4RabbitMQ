using System;

namespace Microsoft.Extensions.DistributedMessage4RabbitMQ.Contracts
{
    /// <summary>
    /// Attribute to mark a class as a dead letter message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DeadLetterAttribute : Attribute
    {
        /// <summary>
        /// The type of the message that is marked as a dead letter.
        /// </summary>
        public Type MessageType { get; }
        /// <summary>
        /// The constructor for the DeadLetterAttribute.
        /// </summary>
        /// <param name="messageType"></param>
        public DeadLetterAttribute(Type messageType)
        {
            MessageType = messageType;
        }
    }
}
