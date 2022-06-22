﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Builder.LanguageGeneration
{
    /// <summary>
    /// Centralized Template errors.
    /// </summary>
    public class TemplateErrors
    {
        public const string NoTemplate = "LG file must have at least one template definition.";

        public const string InvalidTemplateBody = "Invalid template body. Expecting '-' prefix. ";

        public const string MissingStrucEnd = "Invalid structure body. Expecting ']' at the end of the body.";

        public const string EmptyStrucContent = "Invalid structure body. Body cannot be empty. ";

        public const string InvalidWhitespaceInCondition = "Invalid condition: At most 1 whitespace allowed between 'IF/ELSEIF/ELSE' and ':'.";

        public const string NotStartWithIfInCondition = "Invalid condition: Conditions must start with 'IF/ELSEIF/ELSE' prefix.";

        public const string MultipleIfInCondition = "Invalid template body. There cannot be more than one 'IF' condition. Expecting 'IFELSE' or 'ELSE' statement.";

        public const string NotEndWithElseInCondition = "Conditional response template does not end with 'ELSE' condition.";

        public const string InvalidMiddleInCondition = "Invalid template body. Expecting 'ELSEIF'.";

        public const string InvalidExpressionInCondition = "Invalid condition. 'IF', 'ELSEIF' definitions must include a valid expression.";

        public const string ExtraExpressionInCondition = "Invalid condition. 'ELSE' definition cannot include an expression.";

        public const string MissingTemplateBodyInCondition = "Invalid condition body. Conditions must include a valid body.";

        public const string InvalidWhitespaceInSwitchCase = "Invalid condition: At most 1 whitespace allowed between 'SWITCH/CASE/DEFAULT' and ':'.";

        public const string NotStartWithSwitchInSwitchCase = "Invalid conditional response template. Expecting a 'SWITCH' statement?";

        public const string MultipleSwithStatementInSwitchCase = "Invalid template body. There cannot be more than one 'SWITCH' statement. Expecting 'CASE' or 'DEFAULT' statement.";

        public const string InvalidStatementInMiddlerOfSwitchCase = "Invalid template body. Expecting a 'CASE' statement.";

        public const string NotEndWithDefaultInSwitchCase = "Conditional response template does not end with 'DEFAULT' condition.";

        public const string MissingCaseInSwitchCase = "Invalid template body. Expecting at least one 'CASE' statement.";

        public const string InvalidExpressionInSwiathCase = "Invalid condition. 'SWITCH' and 'CASE' statements must include a valid expression.";

        public const string ExtraExpressionInSwitchCase = "Invalid condition. 'DEFAULT' statement cannot include an expression.";

        public const string MissingTemplateBodyInSwitchCase = "Invalid condition body. Expecing valid body inside a 'CASE' or 'DEFAULT' block.";

        public const string NoEndingInMultiline = "Expecting '```' to close the multi-line block.";

        public const string NoCloseBracket = "Close } is missing in Expression.";

        public const string LoopDetected = "Loop detected:";

        public const string InvalidMemory = "Scope is not a LG customized memory.";

        public const string StaticFailure = "Static failure with the following error.";

        public const string InvalidTemplateNameType = "Expected string type for the parameter of template function.";

        public static string InvalidStrucBody(string invalidBody) => $"Invalid structure body: '{invalidBody}'. Body can include <PropertyName> = <Value> pairs or ${{reference()}} template reference.";

        public static string InvalidStrucName(string invalidName) => $"Invalid structure name: '{invalidName}'. name should start with letter/number/_ and can only contains letter/number/./_.";

        public static string SyntaxError(string unexpectedContent) => $"{unexpectedContent}. Expecting a comment, template definition, import statement or option definition.";

        public static string InvalidTemplateName(string invalidTemplateName) => $"Invalid template name: '{invalidTemplateName}'. Template names can only contain letter, underscore '_' or number. Any part of a template name (split by '.') cannot start with a number.";

        public static string InvalidParameter(string invalidParameter) => $"Invalid parameter name: '{invalidParameter}'. Parameter names can only contain letter, underscore '_' or number.";

        public static string DuplicatedTemplateInSameTemplate(string templateName) => $"Duplicated definitions found for template: '{templateName}'.";

        public static string DuplicatedTemplateInDiffTemplate(string templateName, string source) => $"Duplicated definitions found for template: '{templateName}' in '{source}'.";

        public static string NoTemplateBody(string templateName) => $"Missing template body in template '{templateName}'.";

        public static string TemplateNotExist(string templateName) => $"No such template '{templateName}'.";

        public static string ErrorExpression(string refFullText, string templateName, string prefixText) => $"[{templateName}] {prefixText} Error occurred when evaluating '{refFullText}'.";

        public static string NullExpression(string expression) => $"'{expression}' evaluated to null.";

        public static string ArgumentMismatch(string templateName, int expectedCount, int actualCount) => $"arguments mismatch for template '{templateName}'. Expecting '{expectedCount}' arguments, actual '{actualCount}'.";

        public static string TemplateExist(string templateName) => $"template '{templateName}' already exists.";

        public static string ExpressionParseError(string exp) => $"Error occurred when parsing expression '{exp}'.";
    }
}