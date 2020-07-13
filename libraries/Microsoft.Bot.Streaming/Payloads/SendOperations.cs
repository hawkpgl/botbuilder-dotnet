﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Streaming.PayloadTransport;

namespace Microsoft.Bot.Streaming.Payloads
{
    public class SendOperations
    {
        private readonly IPayloadSender _payloadSender;

        public SendOperations(IPayloadSender payloadSender)
        {
            _payloadSender = payloadSender;
        }

        public async Task SendRequestAsync(Guid id, StreamingRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var disassembler = new RequestDisassembler(_payloadSender, id, request);

            await disassembler.DisassembleAsync().ConfigureAwait(false);

            if (request.Streams != null)
            {
                var tasks = new List<Task>(request.Streams.Count);
                foreach (var contentStream in request.Streams)
                {
                    var contentDisassembler = new ResponseMessageStreamDisassembler(_payloadSender, contentStream);

                    tasks.Add(contentDisassembler.DisassembleAsync());
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        public async Task SendResponseAsync(Guid id, StreamingResponse response)
        {
            var disassembler = new ResponseDisassembler(_payloadSender, id, response);

            await disassembler.DisassembleAsync().ConfigureAwait(false);

            if (response.Streams != null)
            {
                var tasks = new List<Task>(response.Streams.Count());
                foreach (var contentStream in response.Streams)
                {
                    var contentDisassembler = new ResponseMessageStreamDisassembler(_payloadSender, contentStream);

                    tasks.Add(contentDisassembler.DisassembleAsync());
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        public async Task SendCancelAllAsync(Guid id)
        {
            var disassembler = new CancelDisassembler(_payloadSender, id, PayloadTypes.CancelAll);

            await disassembler.Disassemble().ConfigureAwait(false);
        }

        public async Task SendCancelStreamAsync(Guid id)
        {
            var disassembler = new CancelDisassembler(_payloadSender, id, PayloadTypes.CancelStream);

            await disassembler.Disassemble().ConfigureAwait(false);
        }
    }
}
