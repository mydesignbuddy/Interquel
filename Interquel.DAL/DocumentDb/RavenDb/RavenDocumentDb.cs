using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interquel.DAL.DocumentDb.RavenDb
{
    class RavenDocumentDb : IDocumentDb
    {
        public IConnection Connection
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IDatabase GetDatabase()
        {
            throw new NotImplementedException();
        }

        public IDatabase GetDatabase(IConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
