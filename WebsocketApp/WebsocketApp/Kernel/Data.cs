using System;
using System.Collections.Generic;

namespace GamesVonKoch.Data
{
	public class BiDictionary<K, V> : Dictionary<K, V>
	{
		private Dictionary<V, K> inverse = new Dictionary<V, K>();

		public BiDictionary() : base() {}

		public new bool ContainsKey(K key) => base.ContainsKey(key);

		public new void Remove(K key) {
			this.inverse.Remove(base[key]);
			base.Remove(key);
		}

		public new void Add(K key, V val) {
			base.Add(key, val);
			this.inverse.Add(val, key);
		}
	}

	// Either monad
	public interface Either<A, B>
	{
		C Match<C>(Func<A, C> Left, Func<B, C> Right);
		void MatchDo(Action<A> Left, Action<B> Right);
		Either<C, B> MapLeft<C>(Func<A, C> fn);
		Either<A, C> MapRight<C>(Func<B, C> fn);
	}

	public struct Left<A, B> : Either<A, B>
	{
		private A content;
		public Left(A content) { this.content = content; }
		public C Match<C>(Func<A, C> fa, Func<B, C> _) => fa(this.content);
		public void MatchDo(Action<A> fa, Action<B> _) { fa(this.content); }
		public Either<C, B> MapLeft<C>(Func<A, C> fn) => new Left<C, B>(fn(this.content));
		public Either<A, C> MapRight<C>(Func<B, C> fn) => new Left<A, C>(this.content);
	}

	public struct Right<A, B> : Either<A, B>
	{
		private B content;
		public Right(B content) { this.content = content; }
		public C Match<C>(Func<A, C> _, Func<B, C> fb) => fb(this.content);
		public void MatchDo(Action<A> _, Action<B> fb) { fb(this.content); }
		public Either<C, B> MapLeft<C>(Func<A, C> fn) => new Right<C, B>(this.content);
		public Either<A, C> MapRight<C>(Func<B, C> fn) => new Right<A, C>(fn(this.content));
	}

	// Maybe monad
	public interface Maybe<A>
	{
		B Match<B>(Func<B> None, Func<A, B> Some);
		void MatchDo(Action None, Action<A> Some);
		Maybe<B> Map<B>(Func<A, B> fn);
	}

	public struct None<A> : Maybe<A>
	{
		public B Match<B>(Func<B> fn, Func<A, B> _) => fn();
		public void MatchDo(Action def, Action<A> act) { def(); }
		public Maybe<B> Map<B>(Func<A, B> fn) => new None<B>();
	}

	public struct Some<A> : Maybe<A>
	{
		private A content;
		public Some(A content) { this.content = content; }
		public B Match<B>(Func<B> _, Func<A, B> fs) => fs(this.content);
		public void MatchDo(Action def, Action<A> act) { act(this.content); }
		public Maybe<B> Map<B>(Func<A, B> fn) => new Some<B>(fn(this.content));
	}

}
