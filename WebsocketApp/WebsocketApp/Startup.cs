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


namespace WebsocketApp
{
    public class Startup
    {
        private readonly Kernel _kernel;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _kernel = new Kernel(0, (rt, self, state, msg) => {
                var sessionManager_pid = rt.Spawn(null, sessionManager);
                var log_pid = rt.Spawn(null, log);
                return null;
            });
        }
        ActorMeth clientProxy = (rt, self, state, msg) =>
        {
            return null;
        };
        ActorMeth sessionManager = (rt, self, state, msg) =>
        {


            switch (state) 
            {
                case Symbol.StartGame:
                    var pid = rt.SpawnLink(new gameState(10), gameManager);
                    r
                    break;
                default: 
                    break;
            }
            return null;
        };
        ActorMeth sessionRelay = (rt, self, state, msg) =>
        {
            return null;
        };
        struct gameState
        {
            public Gladiator gladiatorA;
            public Gladiator gladiatorB;
            public int turnCount;

            public gameState(int a)
            {
                gladiatorA = new Gladiator();
                gladiatorB = new Gladiator();
                turnCount = a;
            }
        }
        static ActorMeth GameManager() {
            gameState state = new gameState(4);
            ActorMeth behavior = (rt, self, _, msg) =>
        {
            switch (msg.mtype)
            {
                case Symbol.AddChild:
                    //add serverReelay
                    break;
                case Symbol.GameAttack:
                    if ((state.turnCount & 1) == 0)
                    {

                    }
                    else
                    {

                    }
                    break;
                default:
                    break;
            }
            return null;
        }
            return behavior;
        };

        ActorMeth log = (rt, self, state, msg) =>
        {
            return null;
        };
        ActorMeth database = (rt, self, state, msg) =>
        {
            return null;
        };

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
                        
                        await Echo(context, webSocket);
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
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0 , result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                         
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

        }
    }
}
