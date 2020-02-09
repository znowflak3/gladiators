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
	ClientRegister: 13,
	ClientLogin: 14,
	ItemListAdd: 15,
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

	const link = (p, b) => {
		const wrapper = (runtime, self, m) => {
			switch (m.type) {
				case Protocol.Terminate:
					runtime.send(p, mail(Protocol.ChildKilled, self));
				default:
					b(runtime, self, m);
					return;
			}
		}
		return wrapper;
	}

	const mkruntime /*: (Kernel, PID) => Runtime */ = (kernel, pid) => ({
		spawn: b => kernel.spawn(b),
		spawnLink: b => kernel.spawn(link(pid, b)),
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
			case Protocol.HideElement:
				return;
			case Protocol.ShowElement:
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

const free = node => {
	for (let i = 0; i < node.children.length; i++) {
		const child = node.children[i];
		node.removeChild(node.children[i]);
		free(child);
	}
}

const hide = node => {
	node.display = 'none';
	for (let i = 0; i < node.children.length; i++) {
		hide(node.children[i]);
	}
}

const show = node => {
	node.display = 'block';
	for (let i = 0; i < node.children.length; i++) {
		hide(node.children[i]);
	}
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
					runtime.send(to , mail(Protocol.WebSocketMessage, JSON.parse(ev.data)));
				socket.onopen = ev =>
					runtime.send(to, mail(Protocol.WebSocketOpen, "socket open"));
				return;
			case Protocol.WebSocketSend:
				const keys = m.content.keys();
				for (let i = 0; i < keys.length; i++) {
					m.content[keys[i]] = typeof keys[i] === "number"  ? keys[i].ToString() : keys[i];
				}
				socket.send(JSON.stringify(m.content));
				return;
			case Protocol.Terminate:
				socket.close();
				return;
		}
	});
	return wrapper;
}

const LoginActor = (parent, recipient) => {
	const wrapper = element(parent, "div", (runtime, self, parent, m) => {
		switch (m.type) {
			case Protocol.Initialize:
				let login = {};
				login.namediv = document.createElement("div");
				login.name = document.createElement("input");
				login.name.onkeyup = _ => {
					let valid = /^[a-zA-Z0-9]*$/g;
					console.log("fuck", login.name.value);
					if (valid.test(login.name.value)) {
						login.name.style.color = color.text.color;
					} else {
						login.name.style.color = color.text.error;
					}
				};

				login.passdiv = document.createElement("div");
				login.pass = document.createElement("input");
				login.pass.onkeyup = _ => {
					let valid = /^[a-zA-Z0-9]*$/g;
					if (valid.test(login.name.value)) {
						login.name.style.color = color.text.color;
					} else {
						login.name.style.color = color.text.error;
					}
				};

				login.buttondiv = document.createElement("div");
				login.register = document.createElement("input");
				login.register.type = "checkbox";

				login.submit = document.createElement("button");
				login.submit.onclick = _ => {
					let m = {};
					let type = Protocol.ClientLogin;
					if (login.register.checked) type = Protocol.ClientRegister;
					let re = new RegExp('/^[a-zA-Z0-9]*$/g');
					if (!re.test(login.pass.value)) return;
					m.pass = login.pass.value;
					let re = new RegExp('/^[a-zA-Z0-9]*$/g');
					if (!re.test(login.name.value)) return;
					m.name = login.name.value;
					runtime.send(recipient, mail(type, m));
				};

				parent.appendChild(login.namediv);
				parent.appendChild(login.passdiv);
				login.namediv.appendChild(document.createTextNode("name"));
				login.passdiv.appendChild(document.createTextNode("pass"));
				login.namediv.appendChild(login.name);
				login.passdiv.appendChild(login.pass);
				parent.appendChild(login.buttondiv);
				login.submit.textContent = "submit";
				login.buttondiv.appendChild(login.submit);
				login.buttondiv.appendChild(document.createTextNode("register"));
				login.buttondiv.appendChild(login.register);
				return;

			case Protocol.Terminate:
				free(parent);
				return;
		}
	});

	return wrapper;
}

const StoreItem = (parent, name, cost, description) => {
	let item = {};
	const wrapper = element(parent, "div", (runtime, self, parent, m) => {
		switch (m.type) {
			case Protocol.Initialize:
				item.button = document.createElement("button");
				item.buttondiv = document.createElement("div");
				item.buttondiv.appendChild(item.button);
				item.button.textContent = "$" + cost.ToString();
				parent.appendChild(document.createTextNode(name + ": " + description));
				parent.appendChild(item.buttondiv);
				return;
			case Protocol.Terminate:
				free(parent)
				return;
		}
	});
	return wrapper;
}

const RequestActor = sock => {
	const wrapper = actor((runtime, self, m) => {
		switch (m.type) {
			Protocol.ClientLogin:
			Protocol.ClientRegister:
				runtime.send(sock, mail(Protocol.WebSocketSend, m));
				return;
			Protocol.Terminate:
				return;
		}
	});
	return wrapper;
}

const ListActor = parent => {
	let actors = {};
	let divs = [];
	const wrapper = element(parent, "div", (runtime, self, parent, m) => {
		switch (m.type) {

			case Protocol.Initialize:
				for (let i = 0; i < m.content.length; i++) {
					const div = document.createElement("div");
					parent.appendChild(div);
					divs.push(div);
					const item = runtime.spawnLink(m.content[i](div));
					runtime.send(item, mail(Protocol.Initialize, unit));
					actors[item] = item;
				}
				return;

			case Protocol.ListAdd:
				const item = runtime.spawnLink(m.content(parent));
				actors[item] = item;
				return;

			case Protocol.ChildKilled:
				actor[m.content] = undefined;
				return;

			case Protocol.Terminate:
				for (let i = 0; i < div.length; i++) {
					parent.removeChild(divs[i]);
				}
				for (let a in actors) {
					runtime.send(a, mail(Protocol.Terminate, "killed"));
				}
				return;

		}
	});
	return wrapper;
}

// TEST
const approot = document.getElementById("app-root");
console.log("initializing kernel");
const kernel = newKernel(1024);
let children = [];
let ws = undefined;
const root = kernel.spawn(actor((r, s, m) => {
	console.log(m);
	switch (m.type) {
		case Protocol.Terminate:
			break;
		case Protocol.Initialize:
			ws = r.spawn(WebSocketActor(s));
			r.send(ws, mail(Protocol.Initialize, "ws://localhost:4433/ws"));
			request = r.spawn(RequestActor(ws));
			const login = r.spawn(LoginActor(approot, request));
			r.send(login, mail(Protocol.Initialize, unit));
			break;
		case Protocol.WebSocketOpen:
			//r.send(ws, mail(Protocol.WebSocketSend, "foo"));
			break;
		default:
			console.log(m);
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
