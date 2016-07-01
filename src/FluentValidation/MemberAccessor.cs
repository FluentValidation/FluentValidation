#pragma warning disable 1591
namespace FluentValidation.Internal
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;

	public static class MemberAccessor<TObject> {
		public static MemberAccessor<TObject, TValue> From<TValue>(Expression<Func<TObject, TValue>> getExpression) {
			return new MemberAccessor<TObject, TValue>(getExpression);
		} 
	}

	public class MemberAccessor<TObject, TValue> {
		readonly Expression<Func<TObject, TValue>> getExpression;
		readonly Func<TObject, TValue> getter;
		readonly Action<TObject, TValue> setter;

		public MemberAccessor(Expression<Func<TObject, TValue>> getExpression) {
			this.getExpression = getExpression;
			getter = getExpression.Compile();
			setter = CreateSetExpression(getExpression).Compile();

			Member = getExpression.GetMember();
		}

		static Expression<Action<TObject, TValue>> CreateSetExpression(Expression<Func<TObject, TValue>> getExpression) {
			var valueParameter = Expression.Parameter(getExpression.Body.Type);
			var assignExpression = Expression.Lambda<Action<TObject, TValue>>(
				Expression.Assign(getExpression.Body, valueParameter),
				getExpression.Parameters.First(), valueParameter);
			return assignExpression;
		}

		public MemberInfo Member { get; private set; }

		public TValue Get(TObject target) {
			return getter(target);
		}

		public void Set(TObject target, TValue value) {
			setter(target, value);
		}

		protected bool Equals(MemberAccessor<TObject, TValue> other) {
			return Member.Equals(other.Member);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MemberAccessor<TObject, TValue>)obj);
		}

		public override int GetHashCode() {
			return Member.GetHashCode();
		}

		public static implicit operator Expression<Func<TObject, TValue>>(MemberAccessor<TObject, TValue> @this) {
			return @this.getExpression;
		}

		public static implicit operator MemberAccessor<TObject, TValue>(Expression<Func<TObject, TValue>> @this) {
			return new MemberAccessor<TObject, TValue>(@this);
		}
	}
}
