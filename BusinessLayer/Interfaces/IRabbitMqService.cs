using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IRabbitMqService
    {
        void SendMessage(string message);
        void StartConsuming(string queueName,CancellationToken cancellationToken);
    }
}
