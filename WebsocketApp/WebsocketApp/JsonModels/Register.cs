﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketApp.JsonModels
{
    public class Register
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}