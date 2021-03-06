﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GamesVonKoch.Core;
using WebsocketApp.JsonModels;

namespace WebsocketApp.Services
{
    public class WebSocketMailService
    {
        private string _type;
        private dynamic _content;
        public WebSocketMailService(string type, string content)
        {
            _type = type;
            _content = content;
        }
        public dynamic HandleMessage()
        {
            dynamic result = null;
            switch (_type)
            {
                case "adminlist":
                    try
                    {
                        JsonPID pid = JsonSerializer.Deserialize<JsonPID>(_content);
                        result = pid;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "deleteuser":
                    try
                    {
                        DeleteUser pid = JsonSerializer.Deserialize<DeleteUser>(_content);
                        result = pid;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "items":
                    try
                    {
                        JsonPID pid = JsonSerializer.Deserialize<JsonPID>(_content);
                        result = pid;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "buy":
                    try
                    {
                        StoreBuy buy = JsonSerializer.Deserialize<StoreBuy>(_content);
                        result = buy;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "clientregister":
                    try
                    {
                        Register register = JsonSerializer.Deserialize<Register>(_content);
                        result = register;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "clientlogin":
                    try
                    {
                        Login login = JsonSerializer.Deserialize<Login>(_content);
                        result = login;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "queuegame":
                    try
                    {
                        JsonPID jsonPID = JsonSerializer.Deserialize<JsonPID>(_content);
                        result = jsonPID;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "gameaction":
                    try
                    {
                        GameAction gameAction = JsonSerializer.Deserialize<GameAction>(_content);
                        result = gameAction;
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                case "echo":
                    try
                    {
                        JsonPID jsonPID = JsonSerializer.Deserialize<JsonPID>(_content);
                        result = jsonPID;
                        //  result = long.Parse(result.pId);
                    }
                    catch (JsonException ex)
                    {
                        result = ex.Message;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

    }
}
