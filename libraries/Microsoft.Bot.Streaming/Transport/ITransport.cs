﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Bot.Streaming.Transport
{
    public interface ITransport : IDisposable
    {
        bool IsConnected { get; }

        void Close();
    }
}
