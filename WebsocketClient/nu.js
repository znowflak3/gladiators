/* @flow */
/*::
type PID = number;

type Runtime = {
	spawn: Behaviour => PID,
	send: (PID, Mail) => void,
	loop: any => void,
	die: () => void,
	pid: PID,
	tokens: number,
};

type Kernel = {
	spawn: Behaviour => PID,
	send: (PID, Mail) => void,
	loop: (PID, Mail) => void,
	kill: PID => void,
	step: () => bool,
};

type Mail = {type: number, content: any};

type Behaviour = (Runtime, PID, Mail) => void;
type ElementBehaviour = (Runtime, PID, HTMLElement, Mail) => void;

type Actor = {
	behaviour: Behaviour,
	runtime: Runtime,
	mailbox: Mail[],
};
*/

const Protocol /*: { [string]: number } */ = {
	Terminate: 0,
	ChildKilled: 1,
	LoopIteration: 2,
	ParentKilled: 3,
	Dynamic: 4,
	LogItem: 5,
	Initialize: 6,
	Deinitialize: 7,
	WebSocketMessage: 8,
	WebSocketOpen: 9,
	WebSocketSend: 10,
	WebSocketClose: 11,
	WebSocketError: 12,
};

const unit = "unit";

const mail = (type, content) => ({type: type, content: content});

const newKernel /*: number => Kernel */ = (iterations = 1024) => {
	let pidc /*: number */ = 0;
	let pidv /*: boolean */ = false;
	let store /*: { [number]: (Actor | typeof undefined) } */ = {};
	let queue /*: PID[] */ = [];

	const newpid = () => {
		let pid = pidc;
		while (pidv && pid in store) pidc = (pidc + 1) & 0xffffffff;
		pid = pidc;
		pidc = (pidc + 1) & 0xffffffff;
		if (pidc == 0) pidv = true;
		return pid;
	};

	const mkruntime /*: (Kernel, PID) => Runtime */ = (kernel, pid) => ({
		spawn: b => kernel.spawn(b),
		send: (to, mail) => kernel.send(to, mail),
		loop: item => kernel.loop(pid, mail(Protocol.LoopIteration, item)),
		die: () => kernel.kill(pid),
		pid: pid,
		tokens: iterations,
	});

	const self = {
		spawn: b => {
			const pid = newpid();
			console.log("spawning actor at", pid);
			store[pid] = {
				behaviour: b,
				runtime: mkruntime(self, pid),
				mailbox: [],
			};
			return pid;
		},

		send: (to, m) => {
			if (store[to] !== undefined) {
				console.log("mail for", to, "sent");
				store[to].mailbox.unshift(m);
				queue.unshift(to);
			}
			else {
				console.log("mail to", to, "which is a null address", m);
			}
		},

		loop: (pid, item) => {},

		kill: pid => {
			if (store[pid] !== undefined) {
				store[pid].behaviour(store[pid].runtime, pid, mail(Protocol.Terminate, "killed"));
				store[pid] = undefined;
			}
		},

		step: () => {
			if (queue.length > 0) {
				const pid = queue.pop();
				if (store[pid] !== undefined) {
					try {
						console.log("evaluating behaviour");
						store[pid].behaviour(store[pid].runtime, pid, store[pid].mailbox.pop());
						console.log("behaviour evaluated");
					} catch(e) {
						console.log("exception", e);
						self.kill(pid);
					}
				}
				return true;
			}
			return false;
		}

	};
	return self;
};

const actor /*: Behaviour => Behaviour */ = b => {
	const wrapper /*: Behaviour */ = (runtime, self, m) => {
		switch (m.type) {
			case Protocol.ParentKilled:
			case Protocol.ChildKilled:
				runtime.die();
				break;
			default:
				b(runtime, self, m);
				break;
		}
	}
	return wrapper;
}

const element /*: (HTMLElement, string, ElementBehaviour, HTMLElement => void) => Behaviour */ = (parent, elem, b) => {
	let e = document.createElement(elem);
	parent.appendChild(e);
	const wrapper = (runtime, self, m) => {
		switch (m.type) {
			case Protocol.ParentKilled:
			case Protocol.ChildKilled:
				runtime.die();
				return;
			case Protocol.Terminate:
				parent.removeChild(e);
				return;
			default:
				b(runtime, self, e, m);
				return;
		}
		throw "Unreachable";
	};
	return wrapper;
}

const init = (runtime, item, b) => {
	let pid = runtime.spawn(b);
	runtime.send(pid, mail(Protocol.Initialize, item));
	return pid;
}

// ELEMENTS
const div = (p, b, s) => element(p, "div", b, s);
const input = (p, b, s) => element(p, "input", b, s);
const button = (p, b, s) => element(p, "button", b, s);

// GUI

const DivActor = (parent, ...behaviours) => {
	let children = [];
	const wrapper = element(parent, "div", (runtime, self, elem, m) => {
		switch (m.type) {
			case Protocol.Initialize:
				children = behaviours.map(b => init(runtime, "init", b(elem)));
				return;
			case Protocol.ParentKilled:
			case Protocol.Terminate:
				children.forEach(child => runtime.send(child, mail(Protocol.ParentKilled, "killed")));
				return;
		}
	});
	return wrapper;
}

const color = {
	text: {color: "#000000", error: "#773322", background: "#ffffff"},
};

const WebSocketActor = to => {
	let socket = undefined;
	return wrapper = actor((runtime, self, m) => {
		switch (m.type) {
			case Protocol.Initialize:
				socket = new WebSocket(m.content);
				socket.onmessage = ev =>
					runtime.send(to , mail(Protocol.WebSocketMessage, ev.data));
				socket.onopen = ev =>
					runtime.send(to, mail(Protocol.WebSocketOpen, "socket open"));
				return;
			case Protocol.WebSocketSend:
				socket.send(m.content);
				return;
			case Protocol.Terminate:
				socket.close();
				return;
		}
	});
	return wrapper;
}


const LoginActor = (parent, recipent) => {
	let login = {};
	let message = {};
	const wrapper = element(parent, "div", (runtime, self, parent, m) => {
		switch (m.type) {
			case Protocol.Initialize:

				login.name = init(runtime, "init", (DivActor(parent, p => element(p, "input", (r, s, e, m) => {
					switch (m.type) {
						case Protocol.Initialize:
							return;
					}
				}))));

				login.pass = init(runtime, "init", (DivActor(parent, p => element(p, "input", (r, s, e, m) => {
					switch (m.type) {
						case Protocol.Initialize:
							return;
					}
				}))));

				login.submit = init(runtime, "init", (DivActor(parent,
					p => element(p, "button", (r, s, e, m) => {
						switch (m.type) {
							case Protocol.Initialize:
								return;
						}
					}),
					p => element(p, "input", (r, s, e, m) => {
						switch (m.type) {
							case Protocol.Initialize:
								e.type = "checkbox";
								return;
						}
					}))));
				return;

			case Protocol.Terminate:
				for (let child in login) {
					runtime.send(child, mail(Protocol.ParentKilled, "killed"));
				}
				return;
		}
	});

	return wrapper;
}

// ws://demos.kaazing.com/echo

// TEST
const approot = document.getElementById("app-root");
console.log("initializing kernel");
const kernel = newKernel(1024);
let children = [];
let ws = undefined;
const root = kernel.spawn(actor((r, s, m) => {
	switch (m.type) {
		case Protocol.Terminate:
			break;
		case Protocol.Initialize:
			//const login = r.spawn(LoginActor(approot, 0));
			//r.send(login, mail(Protocol.Initialize, "hi"));
			ws = r.spawn(WebSocketActor(s));
			r.send(ws, mail(Protocol.Initialize, "ws://demos.kaazing.com/echo"));
			break;
		case Protocol.WebSocketOpen:
			r.send(ws, mail(Protocol.WebSocketSend, "foo"));
			break;
		default:
			console.log(m);
			throw "unhandled message";
	}
}));
kernel.send(root, mail(Protocol.Initialize, 0));

let iter = 0;
const loop = () => {
	kernel.step();
	console.log("iteration", iter);
	iter += 1;
	setTimeout(loop, 500);
}

loop();
