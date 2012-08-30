#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionExtensions
    {
        public static PropertyInfo GetProperty<T>(this Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            MemberExpression memberExpression = null;

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpression = ((UnaryExpression) expression.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpression = expression.Body as MemberExpression;
                    break;
            }

            if (memberExpression == null)
                throw new ArgumentException("Property could not be identified", "expression");

            PropertyInfo propertyInfo = typeof (T).GetProperty(memberExpression.Member.Name,
                                                               BindingFlags.Instance | BindingFlags.Static |
                                                               BindingFlags.Public | BindingFlags.NonPublic |
                                                               BindingFlags.FlattenHierarchy);

            if (propertyInfo == null)
                throw new ArgumentException("Property could not be identified", "expression");

            return propertyInfo;
        }
    }
}