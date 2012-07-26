using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interquel.DAL.DocumentDb
{
    interface IDocumentDb
    {
        IConnection Connection { get; set; }
        IDatabase GetDatabase();
        IDatabase GetDatabase(IConnection connection);
    }
}
