using System;
using System.Collections.Generic;
using System.Net.WebSockets;




// BEGIN ACTORS

namespace GamesVonKoch.Act
{
    using GamesVonKoch.Core;
    using GamesVonKoch.Data;

    public class Broadcaster
    {
        // Broadcasting actor whcih relays any mail sent to all of it's
        // subscribers.
        private PID self;
        private Runtime runtime;

        private Broadcaster(Runtime runtime, bool link)
        {
            ActorMeth meth = (rt, self, state, mail) =>
            {
                switch (mail.mtype)
                {
                    case Symbol.Killed:
                        rt.Die();
                        return state;
                    case Symbol.Broadcast:
                        foreach (PID to in state) { rt.Send(to, mail.content); }
                        return state;
                    case Symbol.Subscribe:
                        state.Add(mail.content);
                        return state;
                    default:
                        return state;
                }
            };
            this.runtime = runtime;
            if (!link)
            {
                this.self = runtime.Spawn(new List<PID>(), meth);
            }
            else
            {
                this.self = runtime.SpawnLink(new List<PID>(), meth);
            }
        }

        public static Broadcaster Start(Runtime runtime)
        {
            return new Broadcaster(runtime, false);
        }

        public static Broadcaster StartLink(Runtime runtime)
        {
            return new Broadcaster(runtime, true);
        }

        public void Broadcast(Symbol type, Mail msg)
        {
            this.runtime.Send(self, new Mail(type, msg));
        }

        public void Subscribe()
        {
            this.runtime.Send(self, new Mail(Symbol.Subscribe, runtime.self));
        }
    }

    public class Future
    {
        private PID self;
        private Runtime runtime;

        private Future(Runtime runtime, Func<dynamic, Either<dynamic, dynamic>> fn, bool link)
        {
            ActorMeth meth = (rt, self, state, mail) =>
            {
                switch (mail.mtype)
                {
                    case Symbol.Continue:
                        ((Either<dynamic, dynamic>)state).MatchDo(
                            Left: r => { },
                            Right: r => { rt.Send(self, new Mail(Symbol.Continue, null)); }
                        );
                        return state;
                    case Symbol.FutureGet:
                        ((Either<dynamic, dynamic>)state).MatchDo(
                            Left: r => { rt.Send(mail.content, new Mail(Symbol.FutureResult, new Some<dynamic>(r))); },
                            Right: _ => { rt.Send(mail.content, new Mail(Symbol.Continue, new None<dynamic>())); }
                        );
                        return state;
                    default:
                        return state;
                }
            };
            if (!link)
            {
                this.self = runtime.Spawn(new None<dynamic>(), meth);
            }
            else
            {
                this.self = runtime.SpawnLink(new None<dynamic>(), meth);
            }
            this.runtime = runtime;
        }

        public static Future Start(Runtime runtime, Func<dynamic, Either<dynamic, dynamic>> fn)
        {
            return new Future(runtime, fn, false);
        }

        public static Future StartLink(Runtime runtime, Func<dynamic, Either<dynamic, dynamic>> fn)
        {
            return new Future(runtime, fn, true);
        }

        public void GetFuture()
        {
            runtime.Send(this.self, new Mail(Symbol.FutureGet, this.self));
        }
    }
}

// END ACTORS

namespace GamesVonKoch.Core
{
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    public enum Symbol
    {
        Text,
        Killed,
        Init,
        Terminate, Continue, Finished,
        Broadcast, Subscribe,
        Future, FutureResult, FutureNotReady, FutureGet,
        Reply,
        NoReply,
        Normal,
        Shutdown,
        Ok, Err,
        Authorize,
        CreateUser,
        AddChild,
        QueueGame,
        GameAction,
        Echo,
        Items, Buy,
        DeleteUser, AdminList


    }

    public struct PID
    {
        private long pid;
        public PID(long pid) { this.pid = pid; }
        public override string ToString() => pid.ToString();
    }

    public struct Mail
    {
        public Symbol mtype;
        public dynamic content;
        public Mail(Symbol mtype, dynamic content)
        {
            this.mtype = mtype;
            this.content = content;
        }
        public override string ToString() => mtype.ToString();
    }

    public struct Runtime
    {
        public PID self;
        private Kernel kernel;

        public Runtime(Kernel kernel, PID self)
        {
            this.self = self;
            this.kernel = kernel;
        }
        public WebSocket GetWebSocket(PID pid)
        {
            return kernel.GetWebSocket(pid);
        }

        public void Send(PID to, Mail msg)
        {
            kernel.Send(to, msg);
        }

        public dynamic Call(PID to, Mail msg)
        {
            return kernel.Call(to, msg);
        }

        public PID Spawn(dynamic state, ActorMeth meth)
        {
            return kernel.Spawn(state, meth);
        }

        public PID SpawnLink(dynamic state, ActorMeth meth)
        {
            return kernel.SpawnLink(self, state, meth);
        }

        public void Die()
        {
            kernel.Kill(self);
        }
    }

    public delegate dynamic ActorMeth(Runtime runtime, PID self, dynamic state, Mail msg);

    public class Kernel
    {

        public class Actor
        {
            public PID pid;
            public Queue<Mail> mqueue;
            public ActorMeth meth;
            public List<PID> children;
            public Runtime runtime;
            public PID? parent;
            public dynamic state;

            public Actor(Runtime runtime, PID pid, dynamic state, ActorMeth meth)
            {
                this.pid = pid;
                this.meth = meth;
                this.children = new List<PID>();
                this.mqueue = new Queue<Mail>();
                this.parent = null;
                this.state = state;
                this.runtime = runtime;
            }

            public Actor(Runtime runtime, PID pid, PID parent, dynamic state, ActorMeth meth)
            {
                this.pid = pid;
                this.meth = meth;
                this.children = new List<PID>();
                this.mqueue = new Queue<Mail>();
                this.parent = parent;
                this.state = state;
                this.runtime = runtime;
            }


            public void AddMail(Mail m)
            {
                mqueue.Enqueue(m);
            }

            public void AddChild(PID child)
            {
                children.Add(child);
            }
        }


        private Queue<PID> kqueue = new Queue<PID>();
        private Dictionary<PID, Actor> actors = new Dictionary<PID, Actor>();
        private Dictionary<PID, WebSocket> webSockets = new Dictionary<PID, WebSocket>();
        public WebSocket GetWebSocket(PID pid)
        {
            if (webSockets.ContainsKey(pid))
                return webSockets[pid];
            return null;
        }

        long pidcount = 0;
        bool pidoverflow = false;

        public Kernel(dynamic state, ActorMeth meth)
        {
            // TODO clean up this mess and provide a better root actor
            var pid = this.Spawn(state, meth);
            this.Send(pid, new Mail(Symbol.Init, null));
        }

        // return a fresh pid
        private PID NewPID()
        {
            // result is the current unallocated PID
            var result = pidcount;
            if (pidoverflow)
            {
                // in the case where we've run out of fresh PIDs to give we
                // must advance the counter past existing ones.
                while (actors.ContainsKey(new PID(pidcount)))
                {
                    pidcount += 1;
                }
                result = pidcount;
            }
            pidcount += 1;
            if (pidcount == 0) { pidoverflow = true; }
            return new PID(result);
        }

        // Spawn a new actor with the given state and method
        public PID Spawn(dynamic state, ActorMeth meth)
        {
            PID pid = this.NewPID();
            Runtime runtime = new Runtime(this, pid);
            this.actors.Add(pid, new Actor(runtime, pid, state, meth));
            Console.WriteLine($"spawn: {pid}");
            return pid;
        }

        // Spawn a new linked actor with the given state and method
        public PID SpawnLink(PID parent, dynamic state, ActorMeth meth)
        {
            PID pid = this.NewPID();
            Runtime runtime = new Runtime(this, pid);
            this.actors.Add(pid, new Actor(runtime, pid, parent, state, meth));
            this.actors[parent].children.Add(pid);
            Console.WriteLine($"spawn link: {pid}");
            return pid;
        }
        //Add Websocketconnection to Dictionary
        public void AddWebSocketConnection(PID key, WebSocket webSocket)
        {
            webSockets.Add(key, webSocket);
        }
        // Send a message to another actor's mailbox
        public void Send(PID to, Mail msg)
        {
            Console.WriteLine($"{to} <= {msg}");

            if (actors.ContainsKey(to))
            {
                kqueue.Enqueue(to);
                actors[to].AddMail(msg);
            }
            else 
            {
                //log/report back. if something goes mad crazy!!!!!!
            }
            //Step();

        }

        public dynamic Call(PID self, Mail msg)
        {
            // TODO optimize really slow calls such as this one
            var runtime = actors[self].runtime;
            var state = actors[self].state;
            var meth = actors[self].meth;
            var resp = meth(runtime, self, state, msg);
            actors[self].state = resp.Item1;
            return resp.Item2;
        }

        // Step the world by one frame. The return value indicates if the kernel
        // is currently running (there are messages left in kqueue).
        public bool Step()
        {
            if (this.kqueue.Count != 0)
            {
                PID pid = this.kqueue.Dequeue();
                var state = actors[pid].state;
                var meth = actors[pid].meth;
                var runtime = actors[pid].runtime;
                // ASSUMPTION! only actors with mail are in kqueue thus it's
                // safe not to check if their mqueue is empty. This saves a
                // single check but causes confusing errors when the root
                // actor isn't setup properly.
                var mail = actors[pid].mqueue.Dequeue();
                try
                {
                    actors[pid].state = meth(runtime, pid, state, mail);
                }
                catch
                {
                    this.Kill(pid);
                }
                return true;
            }
            else
            {
                Console.WriteLine("Kernel exit without error: This is a server bug.");
                return false;
            }
        }
        public async void Loop()
        {
            while (true)
            {
                await Task.Run(() => Step());

            }
        }

        public void Kill(PID self)
        {
            Mail died = new Mail(Symbol.Killed, self);
            foreach (var child in actors[self].children)
            {
                this.Send(child, died);
            }
            if (actors[self].parent.HasValue) { this.Send(actors[self].parent.Value, died); }
            actors.Remove(self);
        }
    }
}
/*
namespace GamesVonKoch
{
	using GamesVonKoch.Core;
	
	public static class Program
	{
		public static void Main(String[] args) {
			// TODO: make this not painful
			ActorMeth input = (rt, self, state, msg) => {
				var text = Console.ReadLine();
				rt.Send(state, new Mail(Symbol.Text, text));
				return null;
			};

			ActorMeth writer = (rt, self, state, msg) => {
				// configuration
				if (state == null) { return msg.content; }
				Console.WriteLine(msg.content);
				return state;
			};

			ActorMeth game = (rt, self, state, msg) => {
				// ASSUMPTION! The messages won't arrive out of order since
				// the implementation is currently on a single thread
				rt.Send(state, new Mail(Symbol.Init, msg.content == 5 ? "correct" : "wrong"));
				return state;
			};

			var kernel = new Kernel(0, (rt, self, state, msg) => {
				rt.Die();
				var writer_pid = rt.Spawn(null, writer);
				var game_pid = rt.Spawn(writer_pid, game);
				var input_pid = rt.Spawn(game_pid, input);
				rt.Send(writer_pid, new Mail(Symbol.Init, input_pid));
				rt.Send(input_pid, new Mail(Symbol.Init, null));
				return null;
			});
			kernel.Step();
			kernel.Step();
			kernel.Step();
			kernel.Step();
			kernel.Step();
		}
		
	}
	
}*/
