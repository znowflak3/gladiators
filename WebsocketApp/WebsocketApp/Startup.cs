using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebSocketOptions = Microsoft.AspNetCore.Builder.WebSocketOptions;
using GamesVonKoch.Core;
using GamesVonKoch.DbModels;
using System.Text;
using System.Text.Json;
using WebsocketApp.JsonModels;
using WebsocketApp.Services;
using System.Text.Json.Serialization;

namespace WebsocketApp
{
    public class Startup
    {
        private readonly Kernel _kernel;
        private readonly List<WebSocket> _websockets;

        private PID _sessionManager_pid;
        private PID _echo_pid;
        public Startup(IConfiguration configuration)
        {
            Console.WriteLine("dfgdfhd");
            Configuration = configuration;
            _websockets = new List<WebSocket>();
            _kernel = new Kernel(0, (rt, self, _, msg) =>
            {
                var login_pid = rt.Spawn(null, Login());
                var sessionManager_pid = rt.SpawnLink(login_pid, SessionManager());
                var log_pid = rt.SpawnLink(login_pid, Log());
                var echo_pid = rt.Spawn(null, Echo());

                _sessionManager_pid = sessionManager_pid;
                _echo_pid = echo_pid;

                return null;
            });
            _kernel.Loop();
        }
        ActorMeth ClientProxy()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                return null;
            };
            return behaviour;
        }
        ActorMeth SessionManager()
        {

            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                switch (msg.mtype)
                {
                    case Symbol.StartGame:
                        var pid = rt.SpawnLink(null, GameManager());

                        break;
                    default:
                        break;
                }
                return null;
            };
            return behaviour;
        }
        static ActorMeth SessionRelay()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
                {
                    return null;
                };
            return behaviour;
        }
        static ActorMeth GameManager()
        {
            Gladiator gladiatorOne = new Gladiator();
            Gladiator gladiatorTwo = new Gladiator();

            int turnCount = 0;


            ActorMeth behaviour = (rt, self, _, msg) =>
        {
            switch (msg.mtype)
            {
                case Symbol.AddChild:
                    //add serverReelay
                    break;
                case Symbol.GameAttack:
                    if ((turnCount & 1) == 0)// gladiator a
                    {

                    }
                    else // gladiator b
                    {

                    }
                    break;
                default:
                    break;
            }
            return null;
        };

            return behaviour;
        }

        ActorMeth Log()
        {
            ActorMeth behaviour = (rt, self, state, msg) =>
        {
            return null;
        };
            return behaviour;
        }

        static ActorMeth Database()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                return null;
            };
            return behaviour;
        }
        static ActorMeth Login()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                switch (msg.mtype)
                {
                    case Symbol.Authorize:
                        //check if user exist
                        //check if password matches.
                        //add socket to socketDictionary.
                        break;
                    case Symbol.CreateUser:

                        break;
                    default:
                        break;
                }
                return null;
            };
            return behaviour;
        }
        static ActorMeth Echo()
        {
            ActorMeth behaviour = (rt, self, _, msg) =>
            {
                if (msg.mtype == Symbol.Echo)
                {
                    byte[] buffer;
                    string json = JsonSerializer.Serialize<JsonPID>(msg.content);
                    buffer = Encoding.UTF8.GetBytes(json);
                    WebSocket socket = rt.GetWebSocket(msg.content);
                    socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                return null;
            };
            return behaviour;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4096
            };

            app.UseWebSockets();

            app.Use(async (context, next) =>
            {

                if (context.Request.Path == "/ws")
                {

                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                        while (webSocket.State == WebSocketState.Open)
                        {
                            bool recievedMessage = false;
                            MType mType = new MType();
                            dynamic content = null;

                            PID client_pid = new PID();

                            while (!recievedMessage)
                            {
                                byte[] buffer = new byte[4096];
                                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                                if (!result.CloseStatus.HasValue)
                                {
                                    if (result.MessageType.HasFlag(WebSocketMessageType.Text))
                                    {

                                        string converted = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Replace("\0", string.Empty);
                                        var messages = converted.Split('}', 2, StringSplitOptions.RemoveEmptyEntries);
                                        messages[0] += "}";

                                        try
                                        {
                                            mType = JsonSerializer.Deserialize<MType>(messages[0]);
                                        }
                                        catch (JsonException ex)
                                        {
                                            mType = new MType("error");
                                            //await webSocket.SendAsync();
                                        }
                                        WebSocketMailService webSocketMailService = new WebSocketMailService(mType.MailType, messages[1]);
                                        content = webSocketMailService.HandleMessage();

                                        recievedMessage = true;
                                    }

                                }
                            }
                            switch (mType.MailType)
                            {

                                case "authorize":
                                    if (content.Username == "user" && content.Password == "pass")
                                    {
                                        client_pid = _kernel.Spawn(null, ClientProxy());

                                        _kernel.Send(_sessionManager_pid, new Mail(Symbol.AddChild, client_pid));

                                        _kernel.AddWebSocketConnection(client_pid, webSocket);
                                        _websockets.Add(webSocket);
                                        string json = JsonSerializer.Serialize<JsonPID>(new JsonPID(client_pid));
                                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                    break;
                                case "echo":
                                    //should be client_pid but need to save it for each message sent!
                                    _kernel.Send(_echo_pid, new Mail(Symbol.Echo, new JsonPID(new PID(5))));
                                    break;
                                default:
                                    break;
                            }
                            //Create client proxy
                            //await Echo(context, webSocket);
                        }
                    }
                    else
                    {
                        await next();
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;

                }

            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[4096];

            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

        }
    }
}
