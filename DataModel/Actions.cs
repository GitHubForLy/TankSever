﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class EventActions
    {
        public const string Login = "Login";
        public const string Register = "Register";
        public const string Loginout = "Logout";
        public const string GetPlayerTransforms = "GetPlayerTransforms";
    }

    public class BroadcastActions
    {
        public const string BroadcastMethod = "BroadcastMethod";
        public const string BroadcastField = "BroadcastField";
        public const string UpdateTransform = "UpdateTransform";
        public const string Login = "Login";
        public const string Loginout = "Loginout";
    }
}
