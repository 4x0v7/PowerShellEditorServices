﻿using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.EditorServices.Protocol.MessageProtocol
{
    public class EventContext
    {
        private MessageWriter messageWriter;

        public EventContext(MessageWriter messageWriter)
        {
            this.messageWriter = messageWriter;
        }

        public async Task SendEvent<TParams>(EventType<TParams> eventType, TParams eventParams)
        {
            await this.messageWriter.WriteEvent(
                eventType,
                eventParams);
        }
    }
}
