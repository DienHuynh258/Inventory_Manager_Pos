﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DL;
using System.Runtime.Serialization;
using DTO;
namespace BL
{
    public class LoadUserBL
    {
        public List<Employees> loadUser()
        {
            return new LoadUserDL().LoadUsers();
        }
    }
}
