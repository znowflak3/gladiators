/* @flow */

let debug = {
  spawn: false,
  send: false,
  kill: false,
  all: () => {
    debug.spawn = true;
    debug.send = true;
    debug.kill = true;
  },
  clear: () => {
    debug.spawn = false;
    debug.send = false;
    debug.kill = false;
  }
};

// Error reporting in the browser by indexing the error stack and fetching the
// error location.
const ISNODE = typeof window === "undefined";
const error /*: any */ = document.getElementById("log-root");
const errline = () => {
  try {
    throw Error("");
  } catch (e) {
    const line = e.stack.split("\n")[3];
    const index = line.indexOf("0/");
    return line.slice(index + 2, line.length - 1);
  }
};

const errlog = text => {
  const child = document.createElement("div");
  child.innerText = "[" + errline() + "]: " + text;
  console.log("[", errline(), "]: ", text);
  child.setAttribute("style", "background: #ff6666");
  error.appendChild(child);
};

const warnlog = text => {
  const child = document.createElement("div");
  child.innerText = "[" + errline() + "]: " + text;
  console.log("[", errline(), "]: ", text);
  child.setAttribute("style", "background: #ffff66");
  error.appendChild(child);
};

const conlog = text => {
  const child = document.createElement("div");
  child.innerText = "[" + errline() + "]: " + text;
  console.log("[", errline(), "]: ", text);
  child.setAttribute("style", "background: #999999");
  error.appendChild(child);
};

try {
  const Protocol /*: { [string]: number } */ = {
    Terminate: 0,
    ChildKilled: 1,
    LoopIteration: 2,
    ParentKilled: 3,
    Dynamic: 4,
    LogItem: 5
  };

  const Mail /*: (number, any) => { type: number, content: any } */ = (
    type,
    content
  ) => ({ type: type, content: content });

  // ACTOR ABSTRACTION

  const Actor = behaviour => {
    const wrapper = (
      runtime,
      self,
      mail /*: { type: number, content: any } */
    ) => {
      switch (mail.type) {
        case Protocol.ParentKilled:
        case Protocol.ChildKilled:
        case Protocol.Terminate:
          try {
            runtime.send(self, Protocol.Terminate, mail.content);
          } catch {}
          runtime.die();
          break;
        default:
          behaviour(runtime, self, mail);
          break;
      }
    };
    return wrapper;
  };

  const Future = (
    to /*: number */,
    type /*: number */,
    fun /*: any => { ready: boolean, cont: any } */
  ) => {
    const wrapper = Actor((runtime, self, mail) => {
      switch (mail.type) {
        case Protocol.LoopIteration:
          const x = fun(mail.content);
          if (x.ready) {
            runtime.send(to, type, x.cont);
          } else {
            runtime.loop(x.cont);
          }
          break;
        default:
          break;
      }
    });
    return wrapper;
  };

  const Strategy = {
    // respawn an actor upon it's death
    one_for_one: (runtime, watched, pid) => {
      const fn = watched[pid];
      watched[runtime.spawnLink(fn[0](fn[1]))] = fn;
      watched[pid] = undefined;
    },
    // respawn all actors upon the death of one
    one_for_all: (runtime, watched, pid) => {
      for (let key in watched.keys()) {
        runtime.send(key, Protocol.Terminate, null);
        const fn = watched[pid];
        watched[runtime.spawnLink(fn[0](fn[1]))] = fn;
        watched[pid] = undefined;
      }
    }
  };

  const Supervisor = (
    init /*: [({ [string]: any }) => any, { [string]: any }][] */,
    behaviour /*: (any, number, any, any) => void */,
    strategy = Strategy.one_for_all
  ) => {
    let flag = true;
    let watched = {};
    const wrapper = (runtime, self, mail) => {
      if (flag) {
        flag = false;
        init.forEach(fn => {
          const pid /*: number */ = runtime.spawnLink(fn[0](fn[1]));
          watched[pid] = fn;
        });
      }
      switch (mail.type) {
        case Protocol.ChildKilled:
          strategy(runtime, watched, mail.content);
          break;
        case Protocol.Terminate:
          runtime.die();
          break;
        default:
          behaviour(runtime, self, watched, mail);
          break;
      }
    };
    return wrapper;
  };

  // ACTOR DOM

  const Elem = (
    parent /*: HTMLElement */,
    elem /*: string */,
    behaviour /*: (
      any,
      number,
      HTMLElement,
      { type: number, content: any }
    ) => void */,
    setAttrs /*: (HTMLElement => void) | null */ = null
  ) => {
    let element = document.createElement(elem);
    if (element !== null) {
      if (setAttrs !== null) {
        setAttrs(element);
      }
      parent.appendChild(element);
      const wrapper = Actor((runtime, self, mail) => {
        console.log("mail", mail);
        switch (mail.type) {
          case Protocol.ParentKilled:
          case Protocol.ChildKilled:
          case Protocol.Terminate:
            conlog("remove element");
            parent.removeChild(element);
            break;
          default:
            behaviour(runtime, self, element, mail);
            break;
        }
      });
      return wrapper;
    } else {
      return (runtime, self, mail) => {
        runtime.die();
      };
    }
  };
  const EDiv = (p, b, s) => Elem(p, "div", b, s);
  const EInput = (p, b, s) => Elem(p, "input", b, s);
  const EButton = (p, b, s) => Elem(p, "Button", b, s);

  // KERNEL

  const Kernel = (iterations = 1024) => {
    let store = {};
    let queue = [];
    let pidc = 0;
    let pidv = false;

    const newpid /*: () => number */ = () => {
      let pid = pidc;
      while (pidv && pid in store) pidc = (pidc + 1) & 0xffffffff;
      pid = pidc;
      pidc = (pidc + 1) & 0xffffffff;
      if (pidc == 0) pidv = true;
      return pid;
    };

    const self = {
      // spawn a new actor
      //
      //   spawn: behaviour -> pid
      spawn: behaviour => {
        const pid = newpid();
        if (debug.spawn) conlog("spawning " + pid);
        const runtime = {
          spawn: b => self.spawn(b),
          spawnLink: b => self.spawnLink(pid, b),
          send: (to, type, mail) => self.send(to, Mail(type, mail)),
          call: (to, type, mail) => self.call(to, Mail(type, mail)),
          pid: pid,
          tokens: iterations,
          loop: mail => self.loop(pid, Mail(Protocol.LoopIteration, mail)),
          die: () => self.kill(pid)
        };

        store[pid] = {
          runtime: runtime,
          behaviour: behaviour,
          mailbox: [],
          parent: undefined,
          children: []
        };

        return pid;
      },

      // spawn a new linked actor which is aware of it's children and parent's
      // death.
      //
      //   spawnLink: behaviour -> pid
      spawnLink: (parent /*: number */, behaviour) => {
        const pid = newpid();
        if (debug.spawn) conlog("spawning linked " + pid);
        // the runtime can't be moved out of here thus it's code is duplicated
        // to make up for it
        const runtime = {
          spawn: b => self.spawn(b),
          spawnLink: b => self.spawnLink(pid, b),
          send: (to, type, mail) => self.send(to, Mail(type, mail)),
          call: (to, type, mail) => self.call(to, Mail(type, mail)),
          pid: pid,
          tokens: iterations,
          loop: mail => self.loop(pid, Mail(Protocol.LoopIteration, mail)),
          die: () => self.kill(pid)
        };

        store[pid] = {
          runtime: runtime,
          behaviour: behaviour,
          mailbox: [],
          parent: parent,
          children: []
        };

        return pid;
      },

      // send a message to another actor
      //
      //   send: (pid, mail) -> void
      send: (pid /*: number */, mail) => {
        if (store[pid] !== undefined) {
          if (debug.send) {
            conlog("send to " + pid + " message " + mail.type);
          }
          queue.push(pid);
          store[pid].mailbox.unshift(mail);
        }
      },

      // call upon the behaviour directly without scheduling the actor
      //
      //   call: (pid, mail) -> void
      call: (pid /*: number */, mail) => {
        if (store[pid] !== undefined) {
          const actor = store[pid];
          try {
            actor.behaviour(actor.runtime, pid, mail);
          } catch (e) {
            errlog(e);
          }
        } else errlog("actor " + pid + " doesn't exits");
      },

      // loop
      //
      //   loop: (pid, mail) -> void
      loop: (pid /*: number */, mail) => {
        const actor = store[pid];
        if (actor.tokens > 0) {
          actor.tokens -= 1;
          actor.behaviour(actor.runtime, pid, mail);
        } else {
          actor.tokens = iterations;
          self.send(pid, Mail(Protocol.LoopIteration, mail.content));
        }
      },

      // .
      //
      //   step: () -> bool
      step: () => {
        if (queue.length > 0) {
          const pid = queue.pop();
          if (store[pid] !== undefined) {
            const actor = store[pid];
            const mail = actor.mailbox.pop();
            try {
              actor.behaviour(actor.runtime, pid, mail);
            } catch (e) {
              conlog("behaviour " + pid + " :: " + e);
              self.call(pid, Mail(Protocol.Terminate, e));
            }
          }
          return true;
        } else {
          return false;
        }
      },

      // kill
      //
      //   kill: pid -> void
      kill: (pid /*: number */) => {
        if (store[pid] !== undefined) {
          if (debug.kill) conlog("killing " + pid);
          const actor = store[pid];
          if (actor.hasOwnProperty("parent")) {
            // send the parent a message informing them of their child's death
            // and remove the child from their list of children. This ensures
            // the parent will only announce it's death to the actual children
            // remaining and not children that may possibly not be owned by
            // the parent TODO
            self.send(actor.parent, Mail(Protocol.ChildKilled, pid));
            conlog("notifying parent");
            store[actor.parent].children = store[actor.parent].children.filter(
              x => x !== pid
            );
          }
          //for (let child /*: any */ in actor.children) {
          // self.send(child, Mail(Protocol.ParentKilled, pid));
          //  actor.children = [];
          // }
          store[pid] = undefined;
        }
      }
    };
    return self;
  };

  // TESTING
  conlog("starting tests at " + Date.now());
  let kernel = Kernel();
  debug.all();
  let approot = document.getElementById("app-root");
  if (approot !== null) {
    //debug.all();
    const root = kernel.spawn(
      Actor((runtime, self, mail) => {
        const name = runtime.spawnLink(
          EInput(
            approot,
            (r, s, e, m) => {},
            e => {}
          )
        );
        const pass = runtime.spawnLink(
          EInput(
            approot,
            (r, s, e, m) => {},
            e => {}
          )
        );
        const button = runtime.spawnLink(
          EButton(
            approot,
            (r, s, e, m) => {
              if (m.type === Protocol.Dynamic) {
                r.die();
              }
            },
            e => {
              e.onclick = () => {
                warnlog("stabbing actors");
                runtime.send(button, Protocol.Dynamic, 0);
              };
            }
          )
        );
      })
    );
    kernel.send(root, Mail(Protocol.Dynamic, "start"));

    const loop = () => {
      kernel.step();
      window.requestAnimationFrame(loop);
    };
    loop();
  }
} catch (e) {
  errlog(e);
  warnlog("restarting in 5 seconds");
  setTimeout(() => location.reload(true), 5000);
}
