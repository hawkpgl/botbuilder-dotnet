﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples.Services
{
    public interface IFlightBookingService
    {
        Task<bool> BookFlight(BookingDetails booking, CancellationToken cancellationToken = default(CancellationToken));
    }
}
