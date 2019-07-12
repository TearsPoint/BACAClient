using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel;

namespace DomainLibrary
{
    public class Marriage : BaseSelectItem<Marriage>
    {
        public Marriage()
        {

        }

        public Marriage(int id, string name)
        {
            this.Id = id;
            this.DisplayName = name;
        }

        protected override IEnumerable<Marriage> GetInnterList()
        {
            yield return new Marriage(0, "未");
            yield return new Marriage(1, "已");
        }
    }
}
