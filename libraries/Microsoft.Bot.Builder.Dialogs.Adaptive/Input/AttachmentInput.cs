﻿// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Input
{
    /// <summary>
    /// Format specifier for outputs.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter), /*camelCase*/ true)]
    public enum AttachmentOutputFormat
    {
        /// <summary>
        /// Pass inputs in a List.
        /// </summary>
        All,

        /// <summary>
        /// Pass input as a single element.
        /// </summary>
        First
    }

    /// <summary>
    /// Input dialog which prompts the user to send a file.
    /// </summary>
    public class AttachmentInput : InputDialog
    {
        [JsonProperty("$kind")]
        public const string Kind = "Microsoft.AttachmentInput";

        public AttachmentInput([CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
        {
            this.RegisterSourceLocation(callerPath, callerLine);
        }

        /// <summary>
        /// Gets or sets the AttachmentOutputFormat for the attachments. 
        /// </summary>
        /// <value>
        /// The AttachmentOutputFormat or an expression which evaluates to an AttachmentOutputFormat.
        /// </value>
        [JsonProperty("outputFormat")]
        public EnumExpression<AttachmentOutputFormat> OutputFormat { get; set; } = AttachmentOutputFormat.First;

        protected override Task<InputState> OnRecognizeInputAsync(DialogContext dc, CancellationToken cancellationToken = default)
        {
            var input = dc.State.GetValue<List<Attachment>>(VALUE_PROPERTY);
            var first = input.Count > 0 ? input[0] : null;

            if (first == null || (string.IsNullOrEmpty(first.ContentUrl) && first.Content == null))
            {
                return Task.FromResult(InputState.Unrecognized);
            }

            switch (this.OutputFormat.GetValue(dc.State))
            {
                case AttachmentOutputFormat.All:
                    dc.State.SetValue(VALUE_PROPERTY, input);
                    break;
                case AttachmentOutputFormat.First:
                    dc.State.SetValue(VALUE_PROPERTY, first);
                    break;
            }

            return Task.FromResult(InputState.Valid);
        }
    }
}
