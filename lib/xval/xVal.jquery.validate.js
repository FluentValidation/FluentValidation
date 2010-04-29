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

// xVal.jquery.validate.js
// An xVal plugin to enable support for jQuery Validate
// http://xval.codeplex.com/
// (c) 2009 Steven Sanderson
// License: Microsoft Public License (Ms-PL) (http://www.opensource.org/licenses/ms-pl.html)

(function($) {
    xVal.Plugins["jquery.validate"] = {
        AttachValidator: function(elementPrefix, rulesConfig, options) {
            var self = this;
            self._ensureCustomFunctionsRegistered();
            $(function() {
                self._ensureValidationSummaryContainerExistsIfRequired(options);

                for (var i = 0; i < rulesConfig.Fields.length; i++) {
                    var fieldName = rulesConfig.Fields[i].FieldName;
                    var fieldRules = rulesConfig.Fields[i].FieldRules;

                    // Is there a matching DOM element?
                    var elemId = self._makeAspNetMvcHtmlHelperID((elementPrefix ? elementPrefix + "." : "") + fieldName);
                    var elem = document.getElementById(elemId);
                    if (elem) {
                        for (var j = 0; j < fieldRules.length; j++) {
                            var rule = fieldRules[j];
                            if (rule != null) {
                                var ruleName = rule.RuleName;
                                var ruleParams = rule.RuleParameters;
                                var errorText = (typeof (rule.Message) == 'undefined' ? null : rule.Message);
                                self._attachRuleToDOMElement(ruleName, ruleParams, errorText, $(elem), elementPrefix, options);
                            }
                        }
                    }
                }
            });
        },

        _makeAspNetMvcHtmlHelperID: function(fullyQualifiedModelName) {
            // If you have changed HtmlHelper.IdAttributeDotReplacement from "_" to something else, then you must also change the following line to match
            return fullyQualifiedModelName.replace(/\./g, "_");
        },

        _attachRuleToDOMElement: function(ruleName, ruleParams, errorText, element, elementPrefix, options) {
            var parentForm = element.parents("form");
            if (parentForm.length != 1)
                alert("Error: Element " + element.attr("id") + " is not in a form");
            this._ensureFormIsMarkedForValidation($(parentForm[0]), options);
            this._associateNearbyValidationMessageSpanWithElement(element);

            var options = {};

            switch (ruleName) {
                case "Required":
                    options.required = true;
                    options.messages = { required: errorText || xVal.Messages.Required };
                    break;

                case "Range":
                    if (ruleParams.Type == "string") {
                        options.xVal_stringRange = [ruleParams.Min, ruleParams.Max];
                        if (errorText != null) options.messages = { xVal_stringRange: $.format(errorText) };
                    }
                    else if (ruleParams.Type == "datetime") {
                        var minDate, maxDate;
                        if (typeof (ruleParams.MinYear) != 'undefined')
                            minDate = new Date(ruleParams.MinYear, ruleParams.MinMonth - 1, ruleParams.MinDay, ruleParams.MinHour, ruleParams.MinMinute, ruleParams.MinSecond);
                        if (typeof (ruleParams.MaxYear) != 'undefined')
                            maxDate = new Date(ruleParams.MaxYear, ruleParams.MaxMonth - 1, ruleParams.MaxDay, ruleParams.MaxHour, ruleParams.MaxMinute, ruleParams.MaxSecond);
                        options.xVal_dateRange = [minDate, maxDate];
                        if (errorText != null) options.messages = { xVal_dateRange: $.format(errorText) };
                    }
                    else if (typeof (ruleParams.Min) == 'undefined') {
                        options.max = ruleParams.Max;
                        errorText = errorText || xVal.Messages.Range_Numeric_Max;
                        if (errorText != null) options.messages = { max: $.format(errorText) };
                    }
                    else if (typeof (ruleParams.Max) == 'undefined') {
                        options.min = ruleParams.Min;
                        errorText = errorText || xVal.Messages.Range_Numeric_Min;
                        if (errorText != null) options.messages = { min: $.format(errorText) };
                    }
                    else {
                        options.range = [ruleParams.Min, ruleParams.Max];
                        errorText = errorText || xVal.Messages.Range_Numeric_MinMax;
                        if (errorText != null) options.messages = { range: $.format(errorText) };
                    }

                    break;

                case "StringLength":
                    if (typeof (ruleParams.MinLength) == 'undefined') {
                        options.maxlength = ruleParams.MaxLength;
                        errorText = errorText || xVal.Messages.StringLength_Max;
                        if (errorText != null) options.messages = { maxlength: $.format(errorText) };
                    }
                    else if (typeof (ruleParams.MaxLength) == 'undefined') {
                        options.minlength = ruleParams.MinLength;
                        errorText = errorText || xVal.Messages.StringLength_Min;
                        if (errorText != null) options.messages = { minlength: $.format(errorText) };
                    }
                    else {
                        options.rangelength = [ruleParams.MinLength, ruleParams.MaxLength];
                        errorText = errorText || xVal.Messages.StringLength_MinMax;
                        if (errorText != null) options.messages = { rangelength: $.format(errorText) };
                    }
                    break;

                case "DataType":
                    switch (ruleParams.Type) {
                        case "EmailAddress":
                            options.email = true;
                            options.messages = { email: errorText || xVal.Messages.DataType_EmailAddress };
                            break;
                        case "Integer":
                            options.xVal_regex = ["^\\-?\\d+$", ""];
                            options.messages = { xVal_regex: errorText || xVal.Messages.DataType_Integer || "Please enter a whole number." };
                            break;
                        case "Decimal":
                            options.number = true;
                            options.messages = { number: errorText || xVal.Messages.DataType_Decimal };
                            break;
                        case "Date":
                            options.date = true;
                            options.messages = { date: errorText || xVal.Messages.DataType_Date };
                            break;
                        case "DateTime":
                            options.xVal_regex = ["^\\d{1,2}/\\d{1,2}/(\\d{2}|\\d{4})\\s+\\d{1,2}\\:\\d{2}(\\:\\d{2})?$", ""];
                            options.messages = { xVal_regex: errorText || xVal.Messages.DataType_DateTime || "Please enter a valid date and time." };
                            break;
                        case "Currency":
                            options.xVal_regex = ["^\\D?\\s?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$", ""];
                            options.messages = { xVal_regex: errorText || xVal.Messages.DataType_Currency || "Please enter a currency value." };
                            break;
                        case "CreditCardLuhn":
                            options.xVal_creditCardLuhn = true;
                            if (errorText != null) options.messages = { xVal_creditCardLuhn: errorText };
                            break;
                    }
                    break;

                case "RegEx":
                    options.xVal_regex = [ruleParams.Pattern, ruleParams.Options];
                    if (errorText != null) options.messages = { xVal_regex: errorText };
                    break;

                case "Comparison":
                    var elemToCompareId = this._makeAspNetMvcHtmlHelperID((elementPrefix ? elementPrefix + "." : "") + ruleParams.PropertyToCompare);
                    var elemToCompare = document.getElementById(elemToCompareId);
                    if (elemToCompare != null) {
                        options.xVal_comparison = [ruleParams.PropertyToCompare, elemToCompare, ruleParams.ComparisonOperator];
                        if (errorText != null) options.messages = { xVal_comparison: errorText };
                    }
                    break;

                case "Remote":
                    var dataAccessor = {};
                    parentForm.find("input[name], textarea[name], select[name]").each(function() {
                        var input = this;
                        dataAccessor[input.name] = function() { return $(input).val(); };
                    });
                    options.remote = {
                        url: ruleParams.url,
                        data: dataAccessor,
                        type: 'post'
                    };
                    break;

                case "Custom":
                    var ruleFunction = this._parseAsFunctionWithWarnings(ruleParams.Function);
                    if (ruleFunction != null) {
                        var customFunctionName = this._registerCustomValidationFunction(ruleFunction);
                        var evaluatedParams = ruleParams.Parameters == "null" ? null : eval("(" + ruleParams.Parameters + ")");
                        options[customFunctionName] = evaluatedParams || true;
                        options.messages = [];
                        options.messages[customFunctionName] = errorText;
                    }
                    break;
            }

            element.rules("add", options);
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

        _associateNearbyValidationMessageSpanWithElement: function(element) {
            // If there's a <span class='field-validation-error'> soon after, it's probably supposed to display the error message
            // jquery.validation goes looking for an attribute called "htmlfor" as follows
            var nearbyMessages = element.nextAll("span.field-validation-error");
            if (nearbyMessages.length > 0) {
                $(nearbyMessages[0]).attr("generated", "true")
                                    .attr("htmlfor", element.attr("id"));
            }
        },

        _ensureFormIsMarkedForValidation: function(formElement, options) {
            if (!formElement.data("isMarkedForValidation")) {
                formElement.data("isMarkedForValidation", true);
                var validationOptions = {
                    errorClass: "field-validation-error",
                    errorElement: "span",
                    highlight: function(element) { $(element).addClass("input-validation-error"); },
                    unhighlight: function(element) { $(element).removeClass("input-validation-error"); }
                };
                if (options.ValidationSummary) {
                    validationOptions.wrapper = "li";
                    validationOptions.errorLabelContainer = "#" + options.ValidationSummary.ElementID + " ul:first";
                }
                var validator = formElement.validate(validationOptions);
                if (options.ValidationSummary)
                    this._modifyJQueryValidationElementHidingBehaviourToSupportValidationSummary(validator, options);
            }
        },

        _registerCustomValidationFunction: function(evalFn) {
            jQuery.validator.xValCustomFunctionCount = (jQuery.validator.xValCustomFunctionCount || 0) + 1;
            var functionName = "xVal_customFunction_" + jQuery.validator.xValCustomFunctionCount;
            jQuery.validator.addMethod(functionName, function(value, element, params) {
                if (this.optional(element))
                    return true;
                return evalFn(value, element, params);
            });
            return functionName;
        },

        _ensureCustomFunctionsRegistered: function() {
            if (!jQuery.validator.xValFunctionsRegistered) {
                jQuery.validator.xValFunctionsRegistered = true;

                jQuery.validator.addMethod("xVal_stringRange", function(value, element, params) {
                    if (this.optional(element)) return true;
                    if (params[0] != null)
                        if (value < params[0]) return false;
                    if (params[1] != null)
                        if (value > params[1]) return false;
                    return true;
                }, function(params) {
                    if ((params[0] != null) && (params[1] != null))
                        return $.format(xVal.Messages.Range_String_MinMax || "Please enter a value alphabetically between '{0}' and '{1}'.", params[0], params[1]);
                    else if (params[0] != null)
                        return $.format(xVal.Messages.Range_String_Min || "Please enter a value not alphabetically before '{0}'.", params[0]);
                    else
                        return $.format(xVal.Messages.Range_String_Max || "Please enter a value not alphabetically after '{0}'.", params[1]);
                });

                jQuery.validator.addMethod("xVal_dateRange", function(value, element, params) {
                    if (this.optional(element)) return true;

                    var parsedValue = Date.parse(value);
                    if (isNaN(parsedValue))
                        return false;
                    else
                        parsedValue = new Date(parsedValue);
                    if (params[0] != null)
                        if (parsedValue < params[0]) return false;
                    if (params[1] != null)
                        if (parsedValue > params[1]) return false;
                    return true;
                }, function(params, elem) {
                    if (isNaN(Date.parse(elem.value)))
                        return xVal.Messages.DataType_Date || "Please enter a valid date in yyyy/mm/dd format.";
                    var formatDate = function(date) {
                        var result = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
                        if (date.getHours() + date.getMinutes() + date.getSeconds() != 0)
                            result += " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
                        return result.replace(/\b(\d)\b/g, '0$1');
                    };
                    if ((params[0] != null) && (params[1] != null))
                        return $.format(xVal.Messages.Range_DateTime_MinMax || "Please enter a date between {0} and {1}.", formatDate(params[0]), formatDate(params[1]));
                    else if (params[0] != null)
                        return $.format(xVal.Messages.Range_DateTime_Min || "Please enter a date no earlier than {0}.", formatDate(params[0]));
                    else
                        return $.format(xVal.Messages.Range_DateTime_Max || "Please enter a date no later than {0}.", formatDate(params[1]));
                });

                jQuery.validator.addMethod("xVal_regex", function(value, element, params) {
                    if (this.optional(element)) return true;
                    var pattern = params[0];
                    var options = params[1];
                    var regex = new RegExp(pattern, options);
                    return regex.test(value);
                }, function(params) {
                    return xVal.Messages.Regex || "This value is invalid."; // Pity we can't be more descriptive
                });

                jQuery.validator.addMethod("xVal_creditCardLuhn", function(value, element, params) {
                    if (this.optional(element)) return true;
                    value = value.replace(/\D/g, "");
                    if (value == "") return false;
                    var sum = 0;
                    for (var i = value.length - 2; i >= 0; i -= 2)
                        sum += Array(0, 2, 4, 6, 8, 1, 3, 5, 7, 9)[parseInt(value.charAt(i), 10)];
                    for (var i = value.length - 1; i >= 0; i -= 2)
                        sum += parseInt(value.charAt(i), 10);
                    return (sum % 10) == 0;
                }, function(params) {
                    return xVal.Messages.DataType_CreditCardLuhn || "Please enter a valid credit card number.";
                });

                jQuery.validator.addMethod("xVal_comparison", function(value, element, params) {
                    if (this.optional(element)) return true;
                    var elemToCompare = params[1];
                    var comparisonOperator = params[2];
                    switch (comparisonOperator) {
                        case "Equals": return value == elemToCompare.value;
                        case "DoesNotEqual": return value != elemToCompare.value;
                    }
                    return true; // Ignore unrecognized operator
                }, function(params) {
                    var propertyToCompareName = params[0];
                    var comparisonOperator = params[2];
                    switch (comparisonOperator) {
                        case "Equals": return $.format(xVal.Messages.Comparison_Equals || "This value must be the same as {0}.", propertyToCompareName);
                        case "DoesNotEqual": return $.format(xVal.Messages.Comparison_DoesNotEqual || "This value must be different from {0}.", propertyToCompareName);
                    }
                });

                $.expr[":"].displayableValidationSummaryMessage = function(object) {
                    var span = $(object).find("span:first");
                    if (span.length == 0)
                        return true;
                    return !(span.css("display") === "none") && !span.is(":empty");
                };
            }
        },

        _ensureValidationSummaryContainerExistsIfRequired: function(options) {
            // If we're using a validation summary, make sure the container contains an UL
            // (If there were no server-generated errors, there won't be until we create one)
            if (options.ValidationSummary) {
                var validationSummaryContainer = $("#" + options.ValidationSummary.ElementID);
                if (validationSummaryContainer.length == 0)
                    alert("Cannot find validation summary element \"" + options.ValidationSummary.ElementID + "\". Make sure you've put an element with this ID into your HTML document.");
                if (!validationSummaryContainer.is(":has(ul)")) {
                    validationSummaryContainer.append($("<span class='validation-summary-errors' />").text(options.ValidationSummary.HeaderMessage))
                                              .append($("<ul />"))
                                              .hide();
                }
            }
        },

        _modifyJQueryValidationElementHidingBehaviourToSupportValidationSummary: function(validator, options) {
            // Intercept the hideErrors() and showErrors() methods. Pity there isn't a proper API for this.
            var originalHideErrorsMethod = validator.hideErrors;
            var originalShowErrorsMethod = validator.showErrors;
            validator.hideErrors = function() {
                this.toHide = this.toHide.not("ul"); // Don't ever hide ULs, because these might still contain server-generated error messages
                originalHideErrorsMethod.apply(this, arguments);
                // If the summary container contains no displayable messages, hide the whole thing
                $("#" + options.ValidationSummary.ElementID + ":not(:has(li:displayableValidationSummaryMessage))").hide();
            };
            validator.showErrors = function() {
                originalShowErrorsMethod.apply(this, arguments);
                // If the summary container contains any displayable messages, show it
                $("#" + options.ValidationSummary.ElementID + ":has(li:displayableValidationSummaryMessage)").show();
            };
        }
    };
})(jQuery);