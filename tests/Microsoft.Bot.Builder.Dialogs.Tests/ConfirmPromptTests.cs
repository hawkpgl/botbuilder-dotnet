﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.Dialogs.Tests
{
    [TestClass]
    public class ConfirmPromptTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfirmPromptWithEmptyIdShouldFail()
        {
            var emptyId = string.Empty;
            var confirmPrompt = new ConfirmPrompt(emptyId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfirmPromptWithNullIdShouldFail()
        {
            var nullId = string.Empty;
            nullId = null;
            var confirmPrompt = new ConfirmPrompt(nullId);
        }

        [TestMethod]
        public async Task ConfirmPrompt()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter(TestAdapter.CreateConversation(TestContext.TestName))
                .Use(new AutoSaveStateMiddleware(convoState))
                .Use(new TranscriptLoggerMiddleware(new TraceTranscriptLogger(traceActivity: false)));

            // Create new DialogSet.
            var dialogs = new DialogSet(dialogState);
            dialogs.Add(new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English));

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                var results = await dc.ContinueDialogAsync(cancellationToken);
                if (results.Status == DialogTurnStatus.Empty)
                {
                    await dc.PromptAsync("ConfirmPrompt", new PromptOptions { Prompt = new Activity { Type = ActivityTypes.Message, Text = "Please confirm." } }, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    if ((bool)results.Result)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Confirmed."), cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Not confirmed."), cancellationToken);
                    }
                }
            })
            .Send("hello")
            .AssertReply("Please confirm. (1) Yes or (2) No")
            .Send("yes")
            .AssertReply("Confirmed.")
            .StartTestAsync();
        }

        [TestMethod]
        public async Task ConfirmPromptRetry()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter(TestAdapter.CreateConversation(TestContext.TestName))
                .Use(new AutoSaveStateMiddleware(convoState))
                .Use(new TranscriptLoggerMiddleware(new TraceTranscriptLogger(traceActivity: false)));

            var dialogs = new DialogSet(dialogState);
            dialogs.Add(new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English));

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                var results = await dc.ContinueDialogAsync(cancellationToken);
                if (results.Status == DialogTurnStatus.Empty)
                {
                    var options = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm.",
                        },
                        RetryPrompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm, say 'yes' or 'no' or something like that.",
                        },
                    };
                    await dc.PromptAsync("ConfirmPrompt", options, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    if ((bool)results.Result)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Confirmed."), cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Not confirmed."), cancellationToken);
                    }
                }
            })
            .Send("hello")
            .AssertReply("Please confirm. (1) Yes or (2) No")
            .Send("lala")
            .AssertReply("Please confirm, say 'yes' or 'no' or something like that. (1) Yes or (2) No")
            .Send("no")
            .AssertReply("Not confirmed.")
            .StartTestAsync();
        }

        [TestMethod]
        public async Task ConfirmPromptNoOptions()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter(TestAdapter.CreateConversation(TestContext.TestName))
                .Use(new AutoSaveStateMiddleware(convoState))
                .Use(new TranscriptLoggerMiddleware(new TraceTranscriptLogger(traceActivity: false)));

            var dialogs = new DialogSet(dialogState);
            dialogs.Add(new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English));

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                var results = await dc.ContinueDialogAsync(cancellationToken);
                if (results.Status == DialogTurnStatus.Empty)
                {
                    var options = new PromptOptions();
                    await dc.PromptAsync("ConfirmPrompt", options, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    if ((bool)results.Result)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Confirmed."), cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Not confirmed."), cancellationToken);
                    }
                }
            })
            .Send("hello")
            .AssertReply(" (1) Yes or (2) No")
            .Send("lala")
            .AssertReply(" (1) Yes or (2) No")
            .Send("no")
            .AssertReply("Not confirmed.")
            .StartTestAsync();
        }

        [TestMethod]
        public async Task ConfirmPromptChoiceOptionsNumbers()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter(TestAdapter.CreateConversation(TestContext.TestName))
                .Use(new AutoSaveStateMiddleware(convoState))
                .Use(new TranscriptLoggerMiddleware(new TraceTranscriptLogger(traceActivity: false)));

            var dialogs = new DialogSet(dialogState);
            var prompt = new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English);

            // Set options
            prompt.ChoiceOptions = new Choices.ChoiceFactoryOptions { IncludeNumbers = true };
            prompt.Style = Choices.ListStyle.Inline;
            dialogs.Add(prompt);

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                var results = await dc.ContinueDialogAsync(cancellationToken);
                if (results.Status == DialogTurnStatus.Empty)
                {
                    var options = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm.",
                        },
                        RetryPrompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm, say 'yes' or 'no' or something like that.",
                        },
                    };
                    await dc.PromptAsync("ConfirmPrompt", options, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    if ((bool)results.Result)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Confirmed."), cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Not confirmed."), cancellationToken);
                    }
                }
            })
            .Send("hello")
            .AssertReply("Please confirm. (1) Yes or (2) No")
            .Send("lala")
            .AssertReply("Please confirm, say 'yes' or 'no' or something like that. (1) Yes or (2) No")
            .Send("2")
            .AssertReply("Not confirmed.")
            .StartTestAsync();
        }

        [TestMethod]
        public async Task ConfirmPromptChoiceOptionsMultipleAttempts()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter(TestAdapter.CreateConversation(TestContext.TestName))
                .Use(new AutoSaveStateMiddleware(convoState))
                .Use(new TranscriptLoggerMiddleware(new TraceTranscriptLogger(traceActivity: false)));

            var dialogs = new DialogSet(dialogState);
            var prompt = new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English);

            // Set options
            prompt.ChoiceOptions = new Choices.ChoiceFactoryOptions { IncludeNumbers = true };
            prompt.Style = Choices.ListStyle.Inline;
            dialogs.Add(prompt);

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                var results = await dc.ContinueDialogAsync(cancellationToken);
                if (results.Status == DialogTurnStatus.Empty)
                {
                    var options = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm.",
                        },
                        RetryPrompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm, say 'yes' or 'no' or something like that.",
                        },
                    };
                    await dc.PromptAsync("ConfirmPrompt", options, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    if ((bool)results.Result)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Confirmed."), cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Not confirmed."), cancellationToken);
                    }
                }
            })
            .Send("hello")
            .AssertReply("Please confirm. (1) Yes or (2) No")
            .Send("lala")
            .AssertReply("Please confirm, say 'yes' or 'no' or something like that. (1) Yes or (2) No")
            .Send("what")
            .AssertReply("Please confirm, say 'yes' or 'no' or something like that. (1) Yes or (2) No")
            .Send("2")
            .AssertReply("Not confirmed.")
            .StartTestAsync();
        }

        [TestMethod]
        public async Task ConfirmPromptChoiceOptionsNoNumbers()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter(TestAdapter.CreateConversation(TestContext.TestName))
                .Use(new AutoSaveStateMiddleware(convoState))
                .Use(new TranscriptLoggerMiddleware(new TraceTranscriptLogger(traceActivity: false)));

            var dialogs = new DialogSet(dialogState);
            var prompt = new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English);

            // Set options
            prompt.ChoiceOptions = new Choices.ChoiceFactoryOptions { IncludeNumbers = false, InlineSeparator = "~" };
            dialogs.Add(prompt);

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                var results = await dc.ContinueDialogAsync(cancellationToken);
                if (results.Status == DialogTurnStatus.Empty)
                {
                    var options = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm.",
                        },
                        RetryPrompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Please confirm, say 'yes' or 'no' or something like that.",
                        },
                    };
                    await dc.PromptAsync("ConfirmPrompt", options, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    if ((bool)results.Result)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Confirmed."), cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Not confirmed."), cancellationToken);
                    }
                }
            })
            .Send("hello")
            .AssertReply("Please confirm. Yes or No")
            .Send("2")
            .AssertReply("Please confirm, say 'yes' or 'no' or something like that. Yes or No")
            .Send("no")
            .AssertReply("Not confirmed.")
            .StartTestAsync();
        }

        [TestMethod]
        public async Task ShouldUsePromptClassStyleProperty()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(convoState));

            var dialogs = new DialogSet(dialogState);
            var prompt = new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English)
            {
                Style = ListStyle.Inline
            };
            dialogs.Add(prompt);

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
                {
                    var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                    var results = await dc.ContinueDialogAsync(cancellationToken);
                    if (results.Status == DialogTurnStatus.Empty)
                    {
                        await dc.PromptAsync(
                            "ConfirmPrompt",
                            new PromptOptions
                            {
                                Prompt = new Activity { Type = ActivityTypes.Message, Text = "is it true?" },
                            },
                            cancellationToken);
                    }
                })
                .Send("hello")
                .AssertReply("is it true? (1) Yes or (2) No")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task PromptOptionsStyleShouldOverridePromptClassStyleProperty()
        {
            var convoState = new ConversationState(new MemoryStorage());
            var dialogState = convoState.CreateProperty<DialogState>("dialogState");

            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(convoState));

            var dialogs = new DialogSet(dialogState);
            var prompt = new ConfirmPrompt("ConfirmPrompt", defaultLocale: Culture.English)
            {
                Style = ListStyle.Inline
            };
            dialogs.Add(prompt);

            await new TestFlow(adapter, async (turnContext, cancellationToken) =>
                {
                    var dc = await dialogs.CreateContextAsync(turnContext, cancellationToken);

                    var results = await dc.ContinueDialogAsync(cancellationToken);
                    if (results.Status == DialogTurnStatus.Empty)
                    {
                        await dc.PromptAsync(
                            "ConfirmPrompt",
                            new PromptOptions
                            {
                                Prompt = new Activity { Type = ActivityTypes.Message, Text = "is it true?" },
                                Style = ListStyle.None
                            },
                            cancellationToken);
                    }
                })
                .Send("hello")
                .AssertReply("is it true?")
                .StartTestAsync();
        }

        private Action<IActivity> SuggestedActionsValidator(string expectedText, SuggestedActions expectedSuggestedActions)
        {
            return activity =>
            {
                Assert.IsInstanceOfType(activity, typeof(IMessageActivity));
                var msg = (IMessageActivity)activity;
                Assert.AreEqual(expectedText, msg.Text);
                Assert.AreEqual(expectedSuggestedActions.Actions.Count, msg.SuggestedActions.Actions.Count);
                for (var i = 0; i < expectedSuggestedActions.Actions.Count; i++)
                {
                    Assert.AreEqual(expectedSuggestedActions.Actions[i].Type, msg.SuggestedActions.Actions[i].Type);
                    Assert.AreEqual(expectedSuggestedActions.Actions[i].Value, msg.SuggestedActions.Actions[i].Value);
                    Assert.AreEqual(expectedSuggestedActions.Actions[i].Title, msg.SuggestedActions.Actions[i].Title);
                }
            };
        }
    }
}