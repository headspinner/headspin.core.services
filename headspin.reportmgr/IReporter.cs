﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace headspin.reportmgr
{
    interface IReporter
    {
        System.IO.StringWriter Download();
    }
}