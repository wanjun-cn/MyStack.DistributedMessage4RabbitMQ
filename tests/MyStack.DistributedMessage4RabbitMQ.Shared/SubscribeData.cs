using Microsoft.Extensions.DistributedMessage4RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyStack.DistributedMessage4RabbitMQ.Shared
{
    [Subscribe("ABC")]
    public class SubscribeData
    {
    }
}
