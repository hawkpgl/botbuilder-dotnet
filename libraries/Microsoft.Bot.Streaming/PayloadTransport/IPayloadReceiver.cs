﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.Bot.Streaming.Payloads;
using Microsoft.Bot.Streaming.Transport;

namespace Microsoft.Bot.Streaming.PayloadTransport
{
    public interface IPayloadReceiver
    {
        event DisconnectedEventHandler Disconnected;

        bool IsConnected { get; }

        void Connect(ITransportReceiver receiver);

        void Subscribe(Func<Header, Stream> getStream, Action<Header, Stream, int> receiveAction);

        void Disconnect(DisconnectedEventArgs e = null);
    }
}
