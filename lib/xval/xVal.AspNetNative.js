// Common initialization
var xVal = xVal || {};
xVal.Plugins = xVal.Plugins || {};
xVal.Messages = xVal.Messages || {};
xVal.AttachValidator = function(elementPrefix, rulesConfig, options, pluginName) {
    if (pluginName != null)
        this.Plugins[pluginName].AttachValidator(elementPrefix, rulesConfig, options);
    else
        for (var key in this.Plugins) {
            this.Plugins[key].AttachValidator(elementPrefix, rulesConfig, options);
            return;
        }
};

// xVal.AspNetNative.js
// An xVal plugin to enable support for ASP.NET WebForms native client-side validation
// http://xval.codeplex.com/
// (c) 2009 Steven Sanderson
// License: Microsoft Public License (Ms-PL) (http://www.opensource.org/licenses/ms-pl.html)

var Page_Validators;
var Page_ValidationActive;

xVal.Plugins["AspNetNative"] = {
    AttachValidator: function(elementPrefix, rulesConfig, options) {
        var self = this;
        xVal.domReadyUtils.executeWhenDomIsReady(function() {
            self._attachValidatorDomIsLoaded(elementPrefix, rulesConfig);
        });
    },

    _attachValidatorDomIsLoaded: function(elementPrefix, rulesConfig) {
        Page_Validators = Page_Validators || new Array();

        for (var i = 0; i < rulesConfig.Fields.length; i++) {
            var fieldName = rulesConfig.Fields[i].FieldName;
            var fieldRules = rulesConfig.Fields[i].FieldRules;

            // Is there a matching DOM element?
            var elemId = this._makeAspNetMvcHtmlHelperID((elementPrefix ? elementPrefix + "." : "") + fieldName);
            var elem = document.getElementById(elemId);

            if (elem) {
                for (var j = 0; j < fieldRules.length; j++) {
                    var rule = fieldRules[j];
                    if (rule != null) {
                        var ruleName = rule.RuleName;
                        var ruleParams = rule.RuleParameters;
                        var errorText = (typeof (rule.Message) == 'undefined' ? null : rule.Message);
                        this._attachRuleToDOMElement(ruleName, ruleParams, errorText, elem, elementPrefix);
                    }
                }
            }
        }

        Page_ValidationActive = false;
        if (typeof (ValidatorOnLoad) == "function")
            ValidatorOnLoad();
    },

    _makeAspNetMvcHtmlHelperID: function(fullyQualifiedModelName) {
        // If you have changed HtmlHelper.IdAttributeDotReplacement from "_" to something else, then you must also change the following line to match
        return fullyQualifiedModelName.replace(/\./g, "_");
    },

    _attachRuleToDOMElement: function(ruleName, ruleParams, errorText, element, elementPrefix) {
        var ruleConfig = this._getAspNetRuleConfig(ruleName, ruleParams, errorText, elementPrefix);
        if (ruleConfig == null)
            return;

        // Find parent form and ensure it's enabled for validation
        var parentForm = element;
        while (parentForm.tagName != "FORM") {
            parentForm = parentForm.parentNode;
            if (parentForm == null)
                alert("Error: Element " + element.id + " is not in a form");
        }
        this._ensureValidationEnabledOnForm(parentForm);

        var messageContainer = this._createMessageContainer(element, ruleConfig.errorMessage);

        Page_Validators[Page_Validators.length] = messageContainer;

        messageContainer.controltovalidate = element.id;
        messageContainer.errormessage = ruleConfig.errorMessage;
        messageContainer.display = "Dynamic";
        messageContainer.evaluationfunction = ruleConfig.evaluationFunction;
        for (var i = 0; i < ruleConfig.params.length; i++)
            messageContainer[ruleConfig.params[i].name] = ruleConfig.params[i].value;
    },

    _formatString: function(pattern, params) {
        for (var i = 0; i < params.length; i++)
            pattern = pattern.replace("{" + i + "}", params[i] || "");
        return pattern;
    },

    _formatDate: function(date) {
        // Todo: support variable date formats
        var result = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
        if (date.getHours() + date.getMinutes() + date.getSeconds() != 0)
            result += " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
        return result.replace(/\b(\d)\b/g, '0$1');
    },

    _getAspNetRuleConfig: function(ruleName, ruleParams, fixedErrorText, elementPrefix) {
        switch (ruleName) {
            case "Required":
                return {
                    evaluationFunction: "RequiredFieldValidatorEvaluateIsValid",
                    params: [{ name: "initialvalue", value: ""}],
                    errorMessage: fixedErrorText || xVal.Messages.Required || "Please enter a value."
                };
            case "Range":
                var message;
                var min = null, max = null;
                var messageParams = [];
                if (ruleParams.Type != "datetime") {
                    min = typeof (ruleParams.Min) == 'undefined' ? null : ruleParams.Min;
                    max = typeof (ruleParams.Max) == 'undefined' ? null : ruleParams.Max;
                    if (min != null) messageParams.push(min);
                    if (max != null) messageParams.push(max);
                } else {
                    if (typeof (ruleParams.MinYear) != 'undefined')
                        min = new Date(ruleParams.MinYear, ruleParams.MinMonth - 1, ruleParams.MinDay, ruleParams.MinHour, ruleParams.MinMinute, ruleParams.MinSecond);
                    if (typeof (ruleParams.MaxYear) != 'undefined')
                        max = new Date(ruleParams.MaxYear, ruleParams.MaxMonth - 1, ruleParams.MaxDay, ruleParams.MaxHour, ruleParams.MaxMinute, ruleParams.MaxSecond);
                    if (min != null) messageParams.push(this._formatDate(min));
                    if (max != null) messageParams.push(this._formatDate(max));
                }
                // There are 9 possible default messages depending on data type and which params were specified. 
                // Pick the right one, choosing a suitable fallback message if no defaults are available
                var defaultMessageSet = (ruleParams.Type == "datetime") ? [xVal.Messages.Range_DateTime_Min, xVal.Messages.Range_DateTime_Max, xVal.Messages.Range_DateTime_MinMax]
                                      : (ruleParams.Type == "string") ? [xVal.Messages.Range_String_Min, xVal.Messages.Range_String_Max, xVal.Messages.Range_String_MinMax]
                                      : [xVal.Messages.Range_Numeric_Min, xVal.Messages.Range_Numeric_Max, xVal.Messages.Range_Numeric_MinMax];
                defaultMessageSet[0] = defaultMessageSet[0] || "Please enter a value of at least {0}.";
                defaultMessageSet[1] = defaultMessageSet[1] || "Please enter a value no more than {0}.";
                defaultMessageSet[2] = defaultMessageSet[2] || "Please enter a value between {0} and {1}.";
                message = (min != null) ? ((max != null) ? defaultMessageSet[2]
                                                         : defaultMessageSet[0])
                                        : defaultMessageSet[1];

                var aspNetNativeType = ruleParams.Type == "string" ? "String" :
                                       ruleParams.Type == "integer" ? "Integer" :
                                       ruleParams.Type == "decimal" ? "Double" :
                                       ruleParams.Type == "datetime" ? "Date" : alert("Unknown range type:" + ruleParams.Type);
                if (aspNetNativeType != "Date") {
                    min = "" + (min || Number.MIN_VALUE);
                    max = "" + (max || Number.MAX_VALUE);
                }
                return {
                    evaluationFunction: ruleParams.Type != "datetime" ? "RangeValidatorEvaluateIsValid" : "xVal_AspNetNative_Range_DateTime",
                    params: [{ name: "decimalchar", value: "." },
                             { name: "type", value: aspNetNativeType },
                             { name: "minimumvalue", value: min },
                             { name: "maximumvalue", value: max}],
                    errorMessage: this._formatString(fixedErrorText || message, messageParams)
                };
            case "RegEx":
                return {
                    evaluationFunction: "xVal_AspNetNative_RegEx",
                    params: [{ name: "pattern", value: ruleParams.Pattern },
                             { name: "options", value: typeof (ruleParams.Options) == 'undefined' ? "" : ruleParams.Options}],
                    errorMessage: fixedErrorText || xVal.Messages.Regex || "Please enter a valid value."
                };
            case "StringLength":
                var min = typeof (ruleParams.MinLength) == 'undefined' ? null : ruleParams.MinLength;
                var max = typeof (ruleParams.MaxLength) == 'undefined' ? null : ruleParams.MaxLength;
                var messageParams = [];
                if (min != null) messageParams.push(min);
                if (max != null) messageParams.push(max);
                var pattern = "^.{" + (min || "0") + "," + (max || "") + "}$";
                var message;
                if (min != null) {
                    if (max != null)
                        message = xVal.Messages.StringLength_MinMax || "Please enter a value between {0} and {1} characters long.";
                    else
                        message = xVal.Messages.StringLength_Min || "Please enter a value at least {0} characters long.";
                }
                else
                    message = xVal.Messages.StringLength_Max || "Please enter a value no more than {0} characters long.";

                return {
                    evaluationFunction: "xVal_AspNetNative_RegEx",
                    params: [{ name: "pattern", value: pattern },
                             { name: "options", value: ""}],
                    errorMessage: this._formatString(fixedErrorText || message, messageParams)
                };

            case "DataType":
                if (ruleParams.Type == "CreditCardLuhn") {
                    return {
                        evaluationFunction: "xVal_AspNetNative_CreditCardLuhn",
                        params: [],
                        errorMessage: fixedErrorText || xVal.Messages.DataType_CreditCardLuhn || "Please enter a valid credit card number."
                    };
                }

                var pattern, message;
                switch (ruleParams.Type) {
                    case "EmailAddress":
                        pattern = "^[\\w\\.=-]+@[\\w\\.-]+\\.[\\w]{2,}$";
                        message = xVal.Messages.DataType_EmailAddress || "Please enter a valid email address.";
                        break;
                    case "Integer":
                        pattern = "^\\-?\\d+$";
                        message = xVal.Messages.DataType_Integer || "Please enter a number.";
                        break;
                    case "Decimal":
                        pattern = "^\\-?\\d+(\\.\\d+)?$";
                        message = xVal.Messages.DataType_Decimal || "Please enter a decimal number.";
                        break;
                    case "Date":
                        pattern = "^(\\d{1,2}[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s](\\d{2}|\\d{4}))|((\\d{2}|\\d{4})[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s]\\d{1,2})$";
                        message = xVal.Messages.DataType_Date || "Please enter a valid date.";
                        break;
                    case "DateTime":
                        pattern = "^(\\d{1,2}[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s](\\d{2}|\\d{4}))|((\\d{2}|\\d{4})[/\\-\\.\\s]\\d{1,2}[/\\-\\.\\s]\\d{1,2})\\s+\\d{1,2}\\:\\d{2}(\\:\\d{2})?$";
                        message = xVal.Messages.DataType_DateTime || "Please enter a valid date and time.";
                        break;
                    case "Currency":
                        pattern = "^\\-?\\D?\\s?\\-?\\s?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$";
                        message = xVal.Messages.DataType_Currency || "Please enter a currency value.";
                        break;
                }
                return {
                    evaluationFunction: "xVal_AspNetNative_RegEx",
                    params: [{ name: "pattern", value: pattern },
                             { name: "options", value: "i"}],
                    errorMessage: fixedErrorText || message
                };
            case "Comparison":
                // See if there really is an element to compare to
                var elemToCompareId = this._makeAspNetMvcHtmlHelperID((elementPrefix ? elementPrefix + "." : "") + ruleParams.PropertyToCompare);
                if (document.getElementById(elemToCompareId) == null)
                    return;

                // See if there is an AspNetNative equivalent of the requested operator
                var operator = ruleParams.ComparisonOperator == "Equals" ? "Equal"
                               : ruleParams.ComparisonOperator == "DoesNotEqual" ? "NotEqual"
                               : null;
                if (operator == null)
                    return;

                var message;
                if (ruleParams.ComparisonOperator == "Equals")
                    message = xVal.Messages.Comparison_Equals || "This value must be the same as {0}.";
                if (ruleParams.ComparisonOperator == "DoesNotEqual")
                    message = xVal.Messages.Comparison_DoesNotEqual || "This value must be different from {1}.";

                return {
                    evaluationFunction: "CompareValidatorEvaluateIsValid",
                    params: [{ name: "controltocompare", value: elemToCompareId },
                             { name: "operator", value: operator}],
                    errorMessage: this._formatString(fixedErrorText || message, [ruleParams.PropertyToCompare])
                };

            case "Custom":
                var ruleFunction = this._parseAsFunctionWithWarnings(ruleParams.Function);
                if (ruleFunction != null) {
                    var evaluatedParams = ruleParams.Parameters == "null" ? null : eval("(" + ruleParams.Parameters + ")");
                    return {
                        evaluationFunction: "xVal_AspNetNative_CustomJavaScriptFunction",
                        params: [{ name: "ruleFunction", value: ruleFunction },
                                 { name: "params", value: evaluatedParams}],
                        errorMessage: fixedErrorText || message
                    };
                }
                break;
        }
        return null;
    },

    _parseAsFunctionWithWarnings: function(functionString) {
        var result;
        try { result = eval("(" + functionString + ")") }
        catch (ex) {
            alert("Custom rule error: Could not find or could not parse the function '" + functionString + "'");
            return null;
        }
        if (typeof (result) != 'function') {
            alert("Custom rule error: The JavaScript object '" + functionString + "' is not a function.");
            return null;
        }
        return result;
    },

    _hideElementOnChange: function(elementToWatch, elementToHide) {
        var handler = function() { elementToHide.style.display = "none"; };
        if (elementToWatch.addEventListener)
            elementToWatch.addEventListener('change', handler, false);
        else
            elementToWatch.attachEvent('onchange', handler);
    },

    _createMessageContainer: function(element, initialText) {
        // Is there an existing message container with htmlfor="elementid"?
        // If so, we'll put the messages next to it
        var insertAfterElem = element;
        var spans = document.getElementsByTagName("SPAN");
        for (var i = 0; i < spans.length; i++) {
            if (spans[i].getAttribute("htmlfor") == element.id) {
                insertAfterElem = spans[i];
                this._hideElementOnChange(element, insertAfterElem);
                break;
            }
        }

        var result = document.createElement("span");
        result.id = element.id + "_Msg";
        result.innerHTML = initialText;
        result.style.color = "Red";
        result.style.display = "none";
        if (insertAfterElem.nextSibling)
            insertAfterElem.parentNode.insertBefore(result, insertAfterElem.nextSibling);
        else
            insertAfterElem.parentNode.appendChild(result);
        return result;
    },

    _ensureValidationEnabledOnForm: function(formElement) {
        if (!formElement._xVal_ValidationEnabledOnForm) {
            formElement._xVal_ValidationEnabledOnForm = true;

            formElement.onsubmit = function() {
                return (Page_ValidationActive ? ValidatorCommonOnSubmit() : false);
            };

            var inputControls = formElement.getElementsByTagName("INPUT");
            for (var i = 0; i < inputControls.length; i++) {
                if (inputControls[i].type && (inputControls[i].type.toLowerCase() == 'submit')) {
                    inputControls[i].onclick = function() {
                        WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this.name || this.id || "", "", true, "", "", false, false));
                    };
                }
            }
        }
    }
};

function xVal_AspNetNative_RegEx(val) {
    var value = ValidatorGetValue(val.controltovalidate);
    if (ValidatorTrim(value).length == 0)
        return true;
    var regex = val.options == "" ? new RegExp(val.pattern) : new RegExp(val.pattern, val.options);
    return regex.test(value);
}

function xVal_AspNetNative_Range_DateTime(val) {
    var value = ValidatorGetValue(val.controltovalidate);
    if (ValidatorTrim(value).length == 0)
        return true;
    var min = val.minimumvalue;
    var max = val.maximumvalue;

    var parsedValue = Date.parse(value);
    if (isNaN(parsedValue))
        return false;
    else
        parsedValue = new Date(parsedValue);
    if (min != null)
        if (parsedValue < min) return false;
    if (max != null)
        if (parsedValue > max) return false;
    return true;
}

function xVal_AspNetNative_CreditCardLuhn(val) {
    var value = ValidatorGetValue(val.controltovalidate);
    if (ValidatorTrim(value).length == 0)
        return true;
    value = value.replace(/\D/g, "");
    if (value == "") return false;
    var sum = 0;
    for (var i = value.length - 2; i >= 0; i -= 2)
        sum += Array(0, 2, 4, 6, 8, 1, 3, 5, 7, 9)[parseInt(value.charAt(i), 10)];
    for (var i = value.length - 1; i >= 0; i -= 2)
        sum += parseInt(value.charAt(i), 10);
    return (sum % 10) == 0;
}

function xVal_AspNetNative_CustomJavaScriptFunction(context) {
    var value = ValidatorGetValue(context.controltovalidate);
    if (ValidatorTrim(value).length == 0)
        return true;
    return context.ruleFunction(value, context.controltovalidate, context.params);
}

// --- DOM-ready utils - provides a mechanism for deferring function execution until the DOM is ready
// (just like jQuery's $() function, but without depending on jQuery)
xVal.domReadyUtils = {
    domIsReady: false,
    executeWhenDomIsReady: function(handler) {
        if (this.domIsReady)
            handler();
        else
            this._crossBrowserAttachEventHandler(window, "load", handler);
    },
    _crossBrowserAttachEventHandler: function(target, eventType, handler) {
        if (target.addEventListener)
            target.addEventListener(eventType, handler, false);
        else if (target.attachEvent)
            target.attachEvent('on' + eventType, handler);
        else
            target['on' + eventType] = handler;
    }
};
xVal.domReadyUtils.executeWhenDomIsReady(function() {
    xVal.domReadyUtils.domIsReady = true;
});