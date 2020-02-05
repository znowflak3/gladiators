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
};

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
			else console.log("mail to", to, "which is a null address");
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

const element /*: (HTMLElement, string, ElementBehaviour, HTMLElement => void) => Behaviour */ = (parent, elem, b, attr) => {
	let e = document.createElement(elem);
	if (e !== null) {
		attr(e);
		parent.appendChild(e);
		const wrapper = (runtime, self, m) => {
			switch (m.type) {
				case Protocol.ParentKilled:
				case Protocol.ChildKilled:
					runtime.die();
					break;
				case Protocol.Terminate:
					parent.removeChild(e);
					break;
				default:
					b(runtime, self, e, m);
					break;
			}
		};
		return wrapper;
	}
	return (r, s, m) => { r.die(); }
}

// ELEMENTS
const div = (p, b, s) => element(p, "div", b, s);
const input = (p, b, s) => element(p, "input", b, s);
const button = (p, b, s) => element(p, "button", b, s);
// GUI

// TEST
const approot = document.getElementById("app-root");
if (approot !== null) {
	console.log("initializing kernel");
	const kernel = newKernel(1024);
	let children = [];
	const root = kernel.spawn(actor((r, s, m) => {
		console.log("root");
		switch (m.type) {
			case Protocol.Terminate:
				children.forEach(c => r.send(c, mail(Protocol.ParentKilled, "ha")));
				console.log(m.content);
				break;
			case Protocol.Initialize:
				console.log("initialized");
				children.unshift(r.spawn(input(approot, (r, _, e, m) => {
				}, e => {
					e.onclick = k => r.send(s, mail(Protocol.Terminate, k));
				})));
				break;
			default:
				throw "unhandled message";
		}
	}));
	kernel.send(root, mail(Protocol.Initialize, 0));
	const loop = () => {
		kernel.step();
		window.requestAnimationFrame(loop);
	}
	loop();
}
